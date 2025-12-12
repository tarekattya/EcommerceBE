namespace Ecommerce.Shared;
public class AuthErrors
{
    public static readonly Error InvalidCredentials =
       new("Auth.InvalidCredentials", "The provided email or password is incorrect.", StatusCodes.Status401Unauthorized);

    public static readonly Error LockedOut =
        new("Auth.LockedOut", "The account is locked. Please try again later.", StatusCodes.Status423Locked);

    public static readonly Error EmailAlreadyExists =
        new("Auth.EmailAlreadyExists", "This email is already registered.", StatusCodes.Status409Conflict);

    public static readonly Error UsernameAlreadyExists =
        new("Auth.UsernameAlreadyExists", "This username is already taken.", StatusCodes.Status409Conflict);

    public static readonly Error InvalidToken =
        new("Auth.InvalidToken", "The provided token is invalid or expired.", StatusCodes.Status401Unauthorized);

    public static readonly Error RefreshTokenExpired =
        new("Auth.RefreshTokenExpired", "The refresh token has expired. Please log in again.", StatusCodes.Status401Unauthorized);

    public static readonly Error Unauthorized =
        new("Auth.Unauthorized", "You are not authorized to perform this action.", StatusCodes.Status403Forbidden);

    public static readonly Error CantCreateUser =
        new("Auth.CantCreateUser", "Unable to create user account due to an internal error.", StatusCodes.Status500InternalServerError);
}

