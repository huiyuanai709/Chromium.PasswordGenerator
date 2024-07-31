using Chromium.PasswordGenerator;

namespace Test
{
    public class ChromiumPasswordGeneratorTests
    {
        private readonly ChromiumPasswordGenerator _generator = new();

        [Fact]
        public void GeneratePassword_WithDefaultOptions_ReturnsValidPassword()
        {
            // Act
            var password = _generator.GeneratePassword();

            // Assert
            Assert.Equal(PasswordGenerateOptions.DefaultLength, password.Length);
            Assert.Contains(password, char.IsLower);
            Assert.Contains(password, char.IsUpper);
            Assert.Contains(password, char.IsDigit);
        }

        [Fact]
        public void GeneratePassword_WithCustomLength_ReturnsPasswordOfSpecifiedLength()
        {
            // Arrange
            var options = PasswordGenerateOptions.Default with
            {
                MinLength = 20,
                MaxLength = 20
            };

            // Act
            var password = _generator.GeneratePassword(options);

            // Assert
            Assert.Equal(20, password.Length);
        }

        [Fact]
        public void GeneratePassword_WithOnlyLowerCase_ReturnsOnlyLowerCaseLetters()
        {
            // Arrange
            var options = new PasswordGenerateOptions
            {
                LowerCase = new CharacterClassOptions { Min = 10, Max = 10, CharacterSet = "abcdefghijklmnopqrstuvwxyz" },
                UpperCase = new CharacterClassOptions { Min = 0, Max = 0 },
                Numeric = new CharacterClassOptions { Min = 0, Max = 0 },
                Symbols = new CharacterClassOptions { Min = 0, Max = 0 }
            };

            // Act
            var password = _generator.GeneratePassword(options);

            // Assert
            Assert.Equal(10, password.Length);
            Assert.True(password.All(char.IsLower));
        }

        [Fact]
        public void GeneratePassword_WithSymbolsOnly_ReturnsOnlySymbols()
        {
            // Arrange
            var options = new PasswordGenerateOptions
            {
                LowerCase = new CharacterClassOptions { Min = 0, Max = 0 },
                UpperCase = new CharacterClassOptions { Min = 0, Max = 0 },
                Numeric = new CharacterClassOptions { Min = 0, Max = 0 },
                Symbols = new CharacterClassOptions { Min = 8, Max = 8, CharacterSet = "-_.:!" }
            };

            // Act
            var password = _generator.GeneratePassword(options);

            // Assert
            Assert.Equal(8, password.Length);
            Assert.True(password.All(c => "-_.:!".Contains(c)));
        }

        [Fact]
        public void GeneratePassword_WithConflictingRequirements_ReturnsPasswordMeetingMinimumRequirements()
        {
            // Arrange
            var options = new PasswordGenerateOptions
            {
                MinLength = 5,
                MaxLength = 10,
                LowerCase = new CharacterClassOptions { Min = 3, Max = 5, CharacterSet = "abcdefghijklmnopqrstuvwxyz" },
                UpperCase = new CharacterClassOptions { Min = 3, Max = 5, CharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" },
                Numeric = new CharacterClassOptions { Min = 3, Max = 5, CharacterSet = "0123456789" }
            };

            // Act
            var password = _generator.GeneratePassword(options);

            // Assert
            Assert.InRange(password.Length, 9, 10);
            Assert.True(password.Count(char.IsLower) >= 3);
            Assert.True(password.Count(char.IsUpper) >= 3);
            Assert.True(password.Count(char.IsDigit) >= 3);
        }
    }
}