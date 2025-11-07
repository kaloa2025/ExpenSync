using System.Security.Cryptography;

namespace expenseTrackerPOC.Services.Auth
{
    public class PasswordHasher
    {
        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256).GetBytes(32);

            byte[] result = new byte[48];
            Buffer.BlockCopy(salt, 0, result, 0, 16);
            Buffer.BlockCopy(hash, 0, result, 16, 32);

            return Convert.ToBase64String(result);
        }

        public static bool Verify(string password, string hashed)
        {
            byte[] result = Convert.FromBase64String(hashed);
            byte[] salt = new byte[16];
            byte[] hash = new byte[32];

            Buffer.BlockCopy(result, 0, salt, 0, 16);
            Buffer.BlockCopy(result, 16, hash, 0, 32);

            byte[] hashToCompare = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256).GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
        }
    }
}
