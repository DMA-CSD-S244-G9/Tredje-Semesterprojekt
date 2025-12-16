using bcryptPackage = BCrypt.Net; // Bcrypt NuGet package with a namespace alias

namespace InfiniteInfluence.DataAccessLibrary.Tools;

/// <summary>
/// Utility class that provides helper methods for hashing and validating passwords
/// using the BCrypt hashing algorithm.
///
/// BCrypt is a secure, adaptive hashing algorithm designed specifically for password storage.
/// It automatically incorporates salting and supports configurable work factors to increase
/// resistance against brute-force and rainbow table attacks.
/// </summary>
public class BCryptTool
{
    /// <summary>
    /// Hashes a plain-text password using the BCrypt algorithm with a randomly generated salt.
    /// The resulting hash includes the salt and can be safely stored in the database.
    /// </summary>
    /// 
    /// <remarks>
    /// Password is the plain-text password to be hashed.
    /// </remarks>
    /// 
    /// <returns>
    /// A BCrypt hash representing the securely hashed password.
    /// </returns>
    public static string HashPassword(string password)
    {
        return bcryptPackage.BCrypt.HashPassword(password, GetRandomSalt());
    }


    /// <summary>
    /// Validates a plain-text password against a previously stored BCrypt hash.
    ///
    /// The method extracts the salt and cost factor from the stored hash and
    /// performs a secure comparison to determine whether the password matches.
    /// </summary>
    /// 
    /// <remarks>
    /// Password is the plain-text password provided for validation.
    /// CorrectHash is the stored BCrypt hash to validate the password against.
    /// </remarks>
    /// 
    /// <returns>
    /// True if the password matches the hash; otherwise, false.
    /// </returns>
    public static bool ValidatePassword(string password, string correctHash)
    {
        return bcryptPackage.BCrypt.Verify(password, correctHash);
    }

    /// <summary>
    /// Generates a random BCrypt salt using a predefined work factor.
    ///
    /// The work factor controls the computational cost of the hashing process,
    /// increasing security by making brute-force attacks more time-consuming.
    /// </summary>
    /// 
    /// <returns>
    /// A randomly generated BCrypt salt string.
    /// </returns>
    private static string GetRandomSalt()
    {
        return bcryptPackage.BCrypt.GenerateSalt(12);
    }
}
