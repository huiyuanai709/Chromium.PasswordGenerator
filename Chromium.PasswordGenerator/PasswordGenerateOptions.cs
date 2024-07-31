namespace Chromium.PasswordGenerator
{
    public record PasswordGenerateOptions
    {
        // 默认字符集
        private const string LowerCaseChars = "abcdefghijkmnpqrstuvwxyz";
        private const string UpperCaseChars = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string AlphabeticChars = LowerCaseChars + UpperCaseChars;
        private const string Digits = "23456789";
        private const string SymbolChars = "-_.:!";
        public const int DefaultLength = 15;

        public int Priority { get; set; }
        public int SpecVersion { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }

        public CharacterClassOptions LowerCase { get; set; } = new CharacterClassOptions();
        public CharacterClassOptions UpperCase { get; set; } = new CharacterClassOptions();
        public CharacterClassOptions Alphabetic { get; set; } = new CharacterClassOptions();
        public CharacterClassOptions Numeric { get; set; } = new CharacterClassOptions();
        public CharacterClassOptions Symbols { get; set; } = new CharacterClassOptions();

        public static PasswordGenerateOptions Default = new PasswordGenerateOptions
        {
            Priority = 0,
            SpecVersion = 1,
            LowerCase = new CharacterClassOptions
            {
                CharacterSet = LowerCaseChars,
                Min = 1,
                Max = int.MaxValue
            },
            UpperCase = new CharacterClassOptions
            {
                CharacterSet = UpperCaseChars,
                Min = 1,
                Max = int.MaxValue
            },
            Alphabetic = new CharacterClassOptions
            {
                CharacterSet = AlphabeticChars,
                Min = 0,
                Max = 0
            },
            Numeric = new CharacterClassOptions
            {
                CharacterSet = Digits,
                Min = 1,
                Max = int.MaxValue
            },
            Symbols = new CharacterClassOptions
            {
                CharacterSet = SymbolChars,
                Min = 0,
                Max = 0
            }
        };
    }

    public record CharacterClassOptions
    {
        public string CharacterSet { get; set; } = null!;
        public int Min { get; set; }
        public int Max { get; set; }
    }
}