namespace Aidan.TextAnalysis.Tokenization;

/// <summary>
/// Represents the type of a token. This enumeration is used by the tokenizer to classify tokens.
/// </summary>
public enum TokenType : int
{
    /// <summary>
    /// Represents an unknown token type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Represents the end of input (<c>$</c>).
    /// </summary>
    Eoi,

    /// <summary>
    /// Identifiers must start with a letter or underscore, followed by any number of digits, letters, or underscores.
    /// <br/>
    /// The formation rule in regex notation is: <c>^[a-zA-Z_][a-zA-Z0-9_]*$</c>.
    /// <br/>
    /// Ex: <c>name</c>, <c>Name</c>, <c>_name_</c>, <c>name99</c>. 
    /// </summary>
    Identifier,

    /// <summary>
    /// Special identifier that carries a specific meaning in the context of the language. Ex: <c>if</c>, <c>else</c>, <c>for</c>, <c>while</c> etc...
    /// <br/>
    /// <remarks>
    /// This type exists to enable support for keywords. However, as of the current implementation, the tokenizer does not generate any keywords. All words are considered identifiers instead.
    /// </remarks>
    /// </summary>
    Keyword,

    /// <summary>
    /// Single-character punctuators. Ex: <c>,</c> <c>:</c> <c>(</c> <c>[</c> <c>{</c> 
    /// </summary>
    Punctuation,

    /// <summary>
    /// Represents a comment sequence supported by the tokenizer. Currently, the tokenizer supports the following comment styles:
    /// <list type="bullet">
    /// <item>
    /// <description>C++ style: <c>/* */</c> and <c>// \n</c></description>
    /// </item>
    /// <item>
    /// <description>EBNF style: <c>(* *)</c></description>
    /// </item>
    /// </list>
    /// </summary>
    Comment,

    /*
     * literals.
     */

    /// <summary>
    /// String literal denoted by single or double quotes. Ex: <c>'hello'</c>, <c>"world"</c>.
    /// </summary>
    String,

    /// <summary>
    /// Integer literal. Ex: <c>50</c>, <c>-50</c>.
    /// </summary>
    Integer,

    /// <summary>
    /// Float literal. Ex: <c>0.25</c>, <c>-0.25</c>.
    /// </summary>
    Float,

    /// <summary>
    /// Hexadecimal literal. Ex: <c>0x1A</c>
    /// </summary>
    Hexadecimal,

    /// <summary>
    /// Binary literal. Ex: <c>0b1010</c>
    /// </summary>
    Binary,
}
