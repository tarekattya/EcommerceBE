namespace Ecommerce.Application;

    public class AuthService(UserManager<ApplicationUser> userManager, 
        ApplicationDbContext dbContext, 
        IJwtProvider jwtProvider,
        IEmailService emailService,
        IOptions<BaseUrl> baseUrl) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly IEmailService _emailService = emailService;
        private readonly BaseUrl _baseUrl = baseUrl.Value;
        readonly int _refreshtokenExpirey = 14;
        public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is null)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidCredentials);
            var IsValidPassword = await _userManager.CheckPasswordAsync(user, Password);
            if (!IsValidPassword)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidCredentials);



            var (Token, expiresIn) = _jwtProvider.GenerateToken(user);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshtokenExpirey);


            var RefreshToken = new RefreshToken()
            {
                Token = GenerateRefreshToken(),
                Expires = refreshTokenExpiration,
                UserId = user.Id
            };


            _dbContext.Add<RefreshToken>(RefreshToken);
            await _dbContext.SaveChangesAsync(cancellationToken);


            var response = new AuthResponse(user.Id, user.Email, user.DisplayName, user.UserName ?? string.Empty, Token, expiresIn, RefreshToken.Token, refreshTokenExpiration);
            return Result<AuthResponse>.Success(response);

        }


        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userid = _jwtProvider.ValidateToken(token);
            if (userid is null)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidToken);

            var user = await _userManager.FindByIdAsync(userid);
            if (user is null)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidCredentials);

            var userRefreshToken = await _dbContext.RefreshTokens
    .FirstOrDefaultAsync(t => t.Token == refreshToken && t.UserId == user.Id && t.Revoked == null && t.Expires > DateTime.UtcNow, cancellationToken);

            if (userRefreshToken is null)
                return Result<AuthResponse>.Failure(AuthErrors.RefreshTokenExpired);

            userRefreshToken.Revoked = DateTime.UtcNow;

            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);
            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                Expires = DateTime.UtcNow.AddDays(_refreshtokenExpirey),
                UserId = user.Id,
            };

            _dbContext.Add<RefreshToken>(newRefreshToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = new AuthResponse(
                user.Id,
                user.Email,
                user.DisplayName,
                user.UserName ?? string.Empty,
                newToken,
                expiresIn,
                newRefreshToken.Token,
                newRefreshToken.Expires
            );

            return Result<AuthResponse>.Success(response);
        }

        public async Task<Result> RevokeRefreshTokenAsync( string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userid = _jwtProvider.ValidateToken(token);


            if(userid is null)
                return Result.Failure(AuthErrors.InvalidCredentials);


            var userRefreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(t => t.UserId == userid
                                          && t.Token == refreshToken
                                          && t.Revoked == null
                                          && t.Expires > DateTime.UtcNow,
                                     cancellationToken);

            if (userRefreshToken == null)
                return Result.Failure(AuthErrors.InvalidToken);

            userRefreshToken.Revoked = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return Result<AuthResponse>.Failure(AuthErrors.EmailAlreadyExists);

        if (await _userManager.FindByNameAsync(request.UserName) != null)
            return Result<AuthResponse>.Failure(AuthErrors.UsernameAlreadyExists);

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            DisplayName = request.DisplayName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return Result<AuthResponse>.Failure(new Error("Auth.RegisterFailed", error?.Description ?? "Registration failed.", StatusCodes.Status400BadRequest));
        }

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);
        var refreshToken = new RefreshToken
        {
            Token = GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(_refreshtokenExpirey),
            UserId = user.Id
        };

        _dbContext.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        // Send Confirmation OTP
        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var otp = new Random().Next(100000, 999999).ToString();
        
        user.OTPCode = otp;
        user.OTPToken = confirmationToken;
        user.OTPExpiry = DateTime.UtcNow.AddMinutes(10);
        await _userManager.UpdateAsync(user);
        
        var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "EmailConfirmation.html");
        var emailBody = await File.ReadAllTextAsync(templatePath);

        emailBody = emailBody.Replace("{{DisplayName}}", user.DisplayName)
                             .Replace("{{OTP}}", otp)
                             .Replace("{{Year}}", DateTime.Now.Year.ToString());

        await _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.Id, 
            user.Email!, 
            user.DisplayName, 
            user.UserName, 
            token, 
            expiresIn, 
            refreshToken.Token, 
            refreshToken.Expires
        ));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result.Failure(AuthErrors.InvalidCredentials);

        if (user.OTPCode != request.Code || user.OTPExpiry < DateTime.UtcNow)
            return Result.Failure(new Error("Auth.InvalidOTP", "Invalid or expired code.", StatusCodes.Status400BadRequest));

        var result = await _userManager.ConfirmEmailAsync(user, user.OTPToken!);
        if (!result.Succeeded)
            return Result.Failure(new Error("Auth.EmailConfirmationFailed", "Email confirmation failed.", StatusCodes.Status400BadRequest));

        user.OTPCode = null;
        user.OTPToken = null;
        user.OTPExpiry = null;
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Success(); 

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var otp = new Random().Next(100000, 999999).ToString();

        user.OTPCode = otp;
        user.OTPToken = token;
        user.OTPExpiry = DateTime.UtcNow.AddMinutes(10);
        await _userManager.UpdateAsync(user);

        var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "ForgotPassword.html");
        var emailBody = await File.ReadAllTextAsync(templatePath);

        emailBody = emailBody.Replace("{{DisplayName}}", user.DisplayName)
                             .Replace("{{OTP}}", otp)
                             .Replace("{{Year}}", DateTime.Now.Year.ToString());

        await _emailService.SendEmailAsync(user.Email!, "Reset Your Password", emailBody);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result.Failure(AuthErrors.InvalidCredentials);

        if (user.OTPCode != request.Code || user.OTPExpiry < DateTime.UtcNow)
            return Result.Failure(new Error("Auth.InvalidOTP", "Invalid or expired code.", StatusCodes.Status400BadRequest));

        var result = await _userManager.ResetPasswordAsync(user, user.OTPToken!, request.NewPassword);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(new Error("Auth.ResetFailed", error?.Description ?? "Reset failed.", StatusCodes.Status400BadRequest));
        }

        user.OTPCode = null;
        user.OTPToken = null;
        user.OTPExpiry = null;
        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> ResendConfirmationEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.EmailConfirmed) return Result.Success();

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var otp = new Random().Next(100000, 999999).ToString();

        user.OTPCode = otp;
        user.OTPToken = confirmationToken;
        user.OTPExpiry = DateTime.UtcNow.AddMinutes(10);
        await _userManager.UpdateAsync(user);
        
        var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "EmailConfirmation.html");
        var emailBody = await File.ReadAllTextAsync(templatePath);

        emailBody = emailBody.Replace("{{DisplayName}}", user.DisplayName)
                             .Replace("{{OTP}}", otp)
                             .Replace("{{Year}}", DateTime.Now.Year.ToString());

        await _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure(AuthErrors.InvalidCredentials);

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return Result.Failure(new Error("Auth.ChangeFailed", error?.Description ?? "Change failed.", StatusCodes.Status400BadRequest));
        }

        return Result.Success();
    }
}

