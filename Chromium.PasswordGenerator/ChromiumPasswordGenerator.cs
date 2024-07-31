using System;
using System.Buffers;
using System.Linq;
using System.Text;

namespace Chromium.PasswordGenerator
{
    public class ChromiumPasswordGenerator : IPasswordGenerator
    {
        private static bool IsDifficultToRead(ReadOnlySpan<byte> password)
        {
            for (var i = 1; i < password.Length; i++)
            {
                if (password[i] == password[i - 1] && (password[i] == (byte)'-' || password[i] == (byte)'_'))
                {
                    return true;
                }
            }

            return false;
        }

        private static byte[] GenerateMaxEntropyPassword(PasswordGenerateOptions spec)
        {
            var targetLength = Math.Min(Math.Max(PasswordGenerateOptions.DefaultLength,
                    spec.MinLength ?? PasswordGenerateOptions.DefaultLength),
                spec.MaxLength ?? 200);

            var passwordBytes = ArrayPool<byte>.Shared.Rent(targetLength);
            var passwordSpan = passwordBytes.AsSpan(0, targetLength);
#if NET6_0_OR_GREATER
            var random = Random.Shared;
#else
            var random = new Random();
#endif

            try
            {
                var classes = new ReadOnlySpan<CharacterClassOptions>(new[]
                {
                    spec.LowerCase, spec.UpperCase, spec.Alphabetic, spec.Numeric, spec.Symbols
                });

                var currentIndex = 0;

                // Fill minimum requirements
                foreach (var characterClass in classes)
                {
                    if (characterClass.Max <= 0) continue;

                    var classChars = Encoding.UTF8.GetBytes(characterClass.CharacterSet);
                    for (var i = 0; i < characterClass.Min && currentIndex < targetLength; i++)
                    {
                        passwordSpan[currentIndex++] = classChars[random.Next(classChars.Length)];
                    }
                }

                // Fill remaining characters
                while (currentIndex < targetLength)
                {
                    var validClasses = classes.ToArray().Where(c => c.Max > currentIndex).ToArray();
                    if (validClasses.Length == 0) break;

                    var selectedClass = validClasses[random.Next(validClasses.Length)];
                    var classChars = Encoding.UTF8.GetBytes(selectedClass.CharacterSet);
                    passwordSpan[currentIndex++] = classChars[random.Next(classChars.Length)];
                }

                // Shuffle the password
                for (var i = currentIndex - 1; i > 0; i--)
                {
                    var j = random.Next(i + 1);
                    (passwordSpan[i], passwordSpan[j]) = (passwordSpan[j], passwordSpan[i]);
                }

                // Check if password is difficult to read and regenerate if necessary
                var attempts = 5;
                while (IsDifficultToRead(passwordSpan) && --attempts > 0)
                {
                    for (var i = currentIndex - 1; i > 0; i--)
                    {
                        int j = random.Next(i + 1);
                        (passwordSpan[i], passwordSpan[j]) = (passwordSpan[j], passwordSpan[i]);
                    }
                }

                return passwordSpan[..currentIndex].ToArray();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(passwordBytes);
            }
        }

        public string GeneratePassword()
        {
            return GeneratePassword(PasswordGenerateOptions.Default);
        }

        public string GeneratePassword(PasswordGenerateOptions options)
        {
            var passwordBytes = GenerateMaxEntropyPassword(options);

            if (passwordBytes.Length == 0)
                passwordBytes = GenerateMaxEntropyPassword(PasswordGenerateOptions.Default);

            return Encoding.UTF8.GetString(passwordBytes);
        }
    }
}