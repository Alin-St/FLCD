using DS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Parser.Scanner;

public class Scanner
{
    private string program = "";
    private List<Tuple<int, int>> PIF = new List<Tuple<int, int>>();
    private int index = 0;
    private int currentLine = 1;
    private Dictionary<string, int> tokenPositions = new Dictionary<string, int>();
    private SymbolTable symbolTable = new SymbolTable(500);
    private List<string> reservedWords = new List<string>();
    private List<string> tokens = new List<string>();

    public void ReadTokens()
    {
        string tokenFilePath = Path.Combine(Directory.GetCurrentDirectory(), "language", "token.in");

        try
        {
            string[] lines = File.ReadAllLines(tokenFilePath);

            foreach (var line in lines)
            {
                string[] parts = line.Split(' ');
                string token = parts[0];

                if (new List<string> { "prog", "int", "real", "str", "char", "arr", "bool", "read", "if", "else", "write", "begin", "end",
                    "while", "const", "sys", "and", "or", "rad", "endl"}.Contains(token))
                {
                    reservedWords.Add(token);
                }
                else
                {
                    tokens.Add(token);
                }

                tokenPositions[token] = tokenPositions.Count + 1;
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: 'token.in' file not found in the 'language' directory");
        }
    }

    public void SetProgram(string program)
    {
        this.program = program;
    }

    public void SkipSpaces()
    {
        while (index < program.Length && char.IsWhiteSpace(program[index]))
        {
            if (program[index] == '\n')
            {
                currentLine++;
            }
            index++;
        }
    }

    public void SkipComments()
    {
        SkipSpaces();

        while (index < program.Length - 1 && program.Substring(index, 2) == "//")
        {
            while (index < program.Length && program[index] != '\n')
            {
                index++;
            }
            SkipSpaces();
        }
    }

    public bool TreatStringConstant()
    {
        Regex regexForStringConstant = new Regex(@"^""[a-zA-z0-9_ ?:*^+=.!]*""");

        Match match = regexForStringConstant.Match(program.Substring(index));

        if (!match.Success)
        {
            return false;
        }

        string stringConstant = match.Value;

        if (!symbolTable.HasHash(stringConstant))
        {
            int position = symbolTable.AddHash(stringConstant);
            int hashValue = symbolTable.GetPositionHash(stringConstant);
            PIF.Add(new Tuple<int, int>(tokenPositions["constant"], hashValue));
        }
        else
        {
            int position = symbolTable.GetPositionHash(stringConstant);
            int hashValue = symbolTable.GetPositionHash(stringConstant);
            PIF.Add(new Tuple<int, int>(tokenPositions["constant"], hashValue));
        }

        index += stringConstant.Length;

        return true;
    }

    public bool TreatIntConstant()
    {
        FA fa = new FA("fa_input/int_constant.in");
        string intConstant = fa.GetNextAccepted(program.Substring(index));

        if (intConstant == null)
        {
            return false;
        }

        int nextIndex = index + intConstant.Length;

        if (nextIndex < program.Length && char.IsLetter(program[nextIndex]))
        {
            return false;
        }

        if (Regex.IsMatch(intConstant, @"^[-+]?(\d+)[a-zA-Z]"))
        {
            return false;
        }

        index += intConstant.Length;

        if (!symbolTable.HasHash(intConstant))
        {
            int position = symbolTable.AddHash(intConstant);
            int hashValue = symbolTable.GetPositionHash(intConstant);
            PIF.Add(new Tuple<int, int>(tokenPositions["constant"], hashValue));
        }
        else
        {
            int position = symbolTable.GetPositionHash(intConstant);
            int hashValue = symbolTable.GetPositionHash(intConstant);
            PIF.Add(new Tuple<int, int>(tokenPositions["constant"], hashValue));
        }

        return true;
    }

    public bool CheckIfValid(string possibleIdentifier, string programSubstring)
    {
        if (reservedWords.Contains(possibleIdentifier))
        {
            return false;
        }
        if (Regex.IsMatch(programSubstring, @"^[#]?[A-Za-z_][A-Za-z0-9_]*: (int|char|str|real|arr)"))
        {
            return true;
        }
        return symbolTable.HasHash(possibleIdentifier);
    }

    public bool TreatIdentifier()
    {
        FA fa = new FA("fa_input/identifier.in");

        if (char.IsDigit(program[index]))
        {
            return false;
        }

        string identifier = fa.GetNextAccepted(program.Substring(index));

        if (identifier == null)
        {
            return false;
        }

        if (!CheckIfValid(identifier, program.Substring(index)))
        {
            return false;
        }

        index += identifier.Length;

        if (!symbolTable.HasHash(identifier))
        {
            int position = symbolTable.AddHash(identifier);
            int hashValue = symbolTable.GetPositionHash(identifier);
            PIF.Add(new Tuple<int, int>(tokenPositions["identifier"], hashValue));
        }
        else
        {
            int position = symbolTable.GetPositionHash(identifier);
            int hashValue = symbolTable.GetPositionHash(identifier);
            PIF.Add(new Tuple<int, int>(tokenPositions["identifier"], hashValue));
        }

        return true;
    }

    public bool TreatFromTokenList()
    {
        string possibleToken = program.Substring(index).Split(" ")[0];

        foreach (string reservedToken in reservedWords)
        {
            if (possibleToken.StartsWith(reservedToken))
            {
                string regex = $"^[#]?[a-zA-Z0-9_]*{reservedToken}[a-zA-Z0-9_]+";

                if (Regex.IsMatch(possibleToken, regex))
                {
                    return false;
                }

                index += reservedToken.Length;
                int position = tokenPositions[reservedToken];
                PIF.Add(new Tuple<int, int>(position, -1));

                return true;
            }
        }

        foreach (string token in tokens)
        {
            if (token == possibleToken)
            {
                index += token.Length;
                int position = tokenPositions[token];
                PIF.Add(new Tuple<int, int>(position, -1));

                return true;
            }
            else if (possibleToken.StartsWith(token))
            {
                index += token.Length;
                int position = tokenPositions[token];
                PIF.Add(new Tuple<int, int>(position, -1));

                return true;
            }
        }

        return false;
    }

    public void NextToken()
    {
        SkipSpaces();
        SkipComments();

        if (index == program.Length)
        {
            return;
        }

        if (TreatIdentifier())
        {
            return;
        }

        if (TreatStringConstant())
        {
            return;
        }

        if (TreatIntConstant())
        {
            return;
        }

        if (TreatFromTokenList())
        {
            return;
        }

        throw new Exception($"Lexical error: invalid token '{program[index]}' at line {currentLine}, index {index}");
    }

    public void Scan(string programFileName, string outputFolder)
    {
        try
        {
            string filePath = Path.Combine("language", programFileName);
            string programContent = File.ReadAllText(filePath);

            SetProgram(programContent);

            while (index < program.Length)
            {
                NextToken();
            }

            string pifFilePath = Path.Combine(outputFolder, "PIF.out");

            using (StreamWriter pifFile = new StreamWriter(pifFilePath))
            {
                foreach (var tuple in PIF)
                {
                    pifFile.WriteLine($"{tuple.Item1} -> {tuple.Item2}");
                }
            }

            Console.WriteLine("Lexically correct");
        }
        catch (IOException e)
        {
            throw new Exception(e.Message);
        }
    }
}
