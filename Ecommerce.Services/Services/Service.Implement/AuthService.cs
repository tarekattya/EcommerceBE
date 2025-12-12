namespace Ecommerce.Application;

    public class AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
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


            var response = new AuthResponse(user.Id, user.Email, user.DisplayName, user.UserName, Token, expiresIn, RefreshToken.Token, refreshTokenExpiration);
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
                user.UserName,
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
    }

