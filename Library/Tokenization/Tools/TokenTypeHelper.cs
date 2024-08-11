namespace Aidan.TextAnalysis.Tokenization.Tools;

public static class TokenTypeHelper
{
    public static string ToString(TokenType type)
    {
        return type switch
        {
            TokenType.Unknown => "Unknown",
            TokenType.Eoi => "Eoi",
            TokenType.Identifier => "Identifier",
            TokenType.Keyword => "Keyword",
            TokenType.Punctuation => "Punctuation",
            TokenType.Comment => "Comment",
            TokenType.String => "String",
            TokenType.Integer => "Integer",
            TokenType.Float => "Float",
            TokenType.Hexadecimal => "Hexadecimal",
            TokenType.Binary => "Binary",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}