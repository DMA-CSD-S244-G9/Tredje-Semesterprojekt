using bcryptPackage = BCrypt.Net;


namespace InfiniteInfluence.DataAccessLibrary.Tools;


public class BCryptTool
{
    public static string HashPassword(string password)
    {
        return bcryptPackage.BCrypt.HashPassword(password, GetRandomSalt());
    }

    public static bool ValidatePassword(string password, string correctHash)
    {
        return bcryptPackage.BCrypt.Verify(password, correctHash);
    }

    private static string GetRandomSalt()
    {
        return bcryptPackage.BCrypt.GenerateSalt(12);
    }
}
