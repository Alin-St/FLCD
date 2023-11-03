using System.Text.RegularExpressions;

namespace Interpreter;

internal class Scanner
{
    public SymbolTable SymbolTable { get; set; } = new();
    public ProgramInternalForm PIF { get; set; } = new();

    readonly string[] OPERATORS = new[]{ "<-", "+", "-", "*", "/", "%", "<=", "<", "=", ">=", ">", "and", "or", ".add", ".get", };
    readonly string[] SEPARATORS = new[] { "{", "}", "(", ")", ";", " ", "\t", "\n", "\r" };
    readonly string[] KEYWORDS = new[] { "int", "bool", "int_list", "if", "while", "read", "write" };

    readonly string IDENTIFIER_REGEX = @"^[a-zA-Z][a-zA-Z0-9_]*";
    readonly string INTEGER_REGEX = @"^[0-9]+";
    readonly string BOOLEAN_REGEX = @"^(true|false)";

    readonly List<(string pattern, TokenType tokenType)> REGX_PATTERNS = new();

    public Scanner()
    {
        ComputeRegexPatterns();
    }

    void ComputeRegexPatterns()
    {
        // Convert the operators, separators and keywords into regex patterns
        foreach (var @operator in OPERATORS)
            REGX_PATTERNS.Add((@operator, TokenType.Operator));

        foreach (var separator in SEPARATORS)
            REGX_PATTERNS.Add((separator, TokenType.Separator));

        foreach (var keyword in KEYWORDS)
            REGX_PATTERNS.Add((keyword, TokenType.Keyword));

        // For the tokens that end with a letter, add the END_KW pattern
        const string END_KW = "(?=[^a-zA-Z0-9_]|$)"; // The token must be followed by a non-alphanumeric character or the end of the string
        for (int i = 0; i < REGX_PATTERNS.Count; i++)
        {
            var (pattern, tokenType) = REGX_PATTERNS[i];
            pattern = "^" + Regex.Escape(pattern);

            // If the token ends with a letter, add the END_KW pattern. (careful not to confuse \n and \r as ending with letter)
            if (char.IsLetter(REGX_PATTERNS[i].pattern[^1]))
                pattern += END_KW;

            REGX_PATTERNS[i] = (pattern, tokenType);
        }

        // Add the identifier pattern
        REGX_PATTERNS.Add((IDENTIFIER_REGEX, TokenType.Identifier));

        // Add the constant patterns
        REGX_PATTERNS.Add((INTEGER_REGEX, TokenType.Constant));
        REGX_PATTERNS.Add((BOOLEAN_REGEX, TokenType.Constant));
    }

    public void Scan(string fileContent)
    {
        string initialFile = fileContent;

        // While there are still characters in the file, try to extract a token from the beginning
        while (fileContent != "")
        {
            // Try to match the beginning of the file with each pattern
            bool foundMatch = false;

            foreach (var (pattern, tokenType) in REGX_PATTERNS)
            {
                var match = Regex.Match(fileContent, pattern);

                if (match.Success)
                {
                    // If a match was found, add the token to the ST and PIF
                    var token = match.Value;
                    AddToken(token, tokenType, pattern);

                    foundMatch = true;
                    fileContent = fileContent[match.Length..];
                    break;
                }
            }

            int currentTotalPosition = initialFile.Length - fileContent.Length;
            int currentLine = initialFile[..currentTotalPosition].Split('\n').Length;
            int linePosition = currentTotalPosition - initialFile[..currentTotalPosition].LastIndexOf('\n') - 1;
            if (!foundMatch)
                throw new Exception($"Invalid token at line {currentLine}, position {linePosition}: {fileContent[0]}");
        }    
    }

    public void AddToken(string token, TokenType tokenType, string pattern)
    {
        if (tokenType is TokenType.Operator or TokenType.Separator or TokenType.Keyword)
            PIF.Add(token, -1);
        else if (tokenType == TokenType.Identifier)
            PIF.Add("identifier", SymbolTable.Add(token));
        else if (tokenType == TokenType.Constant && pattern == INTEGER_REGEX)
            PIF.Add("integer", SymbolTable.Add(token));
        else if (tokenType == TokenType.Constant && pattern == BOOLEAN_REGEX)
            PIF.Add("boolean", SymbolTable.Add(token));
        else
            throw new Exception($"Invalid token type: {tokenType}.");
    }
}
