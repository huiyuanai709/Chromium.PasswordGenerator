namespace Chromium.PasswordGenerator
{
    public interface IPasswordGenerator
    {
        string GeneratePassword();

        string GeneratePassword(PasswordGenerateOptions options);
    }
}