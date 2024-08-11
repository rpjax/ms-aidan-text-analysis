namespace Aidan.TextAnalysis.Tokenization.Machine;

public enum TokenizerState
{
    /// <summary>
    /// Denotes no state transition. The machine should remain in the current state.
    /// </summary>
    None,

    /// <summary>
    /// The initial state of the tokenizer.
    /// </summary>
    Initial,

    /// <summary>
    /// TargetState when the tokenizer sees the number zero. This can be the start of an integer, hexadecimal, or floating point number.
    /// </summary>
    NumberZero,

    /// <summary>
    /// TargetState when the tokenizer is processing an integer number.
    /// </summary>
    IntegerNumber,

    /// <summary>
    /// TargetState when the tokenizer is processing a floating point number.
    /// </summary>
    FloatNumber,

    /// <summary>
    /// A digit is expected to start a hexadecimal number. At this point 0x or 0X has been read.
    /// </summary>
    HexadecimalNumberStart,
    HexadecimalNumber,

    /// <summary>
    /// TargetState when the tokenizer is processing a sign. This can be a positive or negative sign. E.g. + or -.
    /// </summary>
    Sign,

    /// <summary>
    /// TargetState when the tokenizer is processing an identifier.
    /// </summary>
    Identifier,

    /// <summary>
    /// TargetState when the tokenizer is processing a punctuation.
    /// </summary>
    Punctuation,

    /*
     * string states
     */

    /// <summary>
    /// TargetState when the tokenizer is processing a string enclosed in single quotes.
    /// </summary>
    SingleQuoteString,

    /// <summary>
    /// TargetState when the tokenizer is processing an escape sequence within a string enclosed in single quotes.
    /// </summary>
    SingleQuoteStringEscape,

    /// <summary>
    /// TargetState when the tokenizer is processing a string enclosed in double quotes.
    /// </summary>
    DoubleQuoteString,

    /// <summary>
    /// TargetState when the tokenizer is processing an escape sequence within a string enclosed in double quotes.
    /// </summary>
    DoubleQuoteStringEscape,

    /// <summary>
    /// TargetState when the tokenizer has reached the end of a string.
    /// </summary>
    StringEnd,

    /*
    * comments states.
    */
    
    CppStyleCommentStart,
    CppStyleSingleLineComment,
    CppStyleMultiLineComment,
    CppStyleMultiLineCommentEndConfirm,

    EbnfStyleMultiLineCommentStart,
    EbnfStyleMultiLineComment,
    EbnfStyleMultiLineCommentEndConfirm,

    CommentEnd,
}
