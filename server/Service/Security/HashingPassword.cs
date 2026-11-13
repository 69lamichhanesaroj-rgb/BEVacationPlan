using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace Service.Security;

public record PasswordHashResult(byte[] Hash, byte[] Salt); // two values returned together

public class HashingPassword
{
    private byte[] HashPassword(string password, byte[] salt)
    {
        Argon2id argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Salt = salt;
        argon2.MemorySize = 32 * 1024;
        argon2.Iterations = 4;
        argon2.DegreeOfParallelism = 1;

        return argon2.GetBytes(32);
    }

    public PasswordHashResult HashAndSaltPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = HashPassword(password, salt);
        return new PasswordHashResult(hash, salt);
    }

    public bool VerifyPassword(string password, byte[] hashedPassword, byte[] storedSalt)
    {
        var newHashedPassword = HashPassword(password, storedSalt);

        return CryptographicOperations.FixedTimeEquals(newHashedPassword, hashedPassword);
    }
}