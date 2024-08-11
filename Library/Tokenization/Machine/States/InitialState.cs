using Aidan.TextAnalysis.Tokenization.Components;
using System.Runtime.CompilerServices;

namespace Aidan.TextAnalysis.Tokenization.Machine;

public class InitialState : IState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c == null)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.End);
        }

        switch (TokenizerAlphabet.LookupCharType(c.Value))
        {
            case CharType.Digit:
                switch (c)
                {
                    case '0':
                        return new TransitionResult(TokenizerState.NumberZero, TokenizerAction.Read);

                    default:
                        return new TransitionResult(TokenizerState.IntegerNumber, TokenizerAction.Read);
                }

            case CharType.Letter:
                return new TransitionResult(TokenizerState.Identifier, TokenizerAction.Read);

            case CharType.Punctuation:
                switch (c)
                {
                    //* identifiers can start with underline.
                    case '_':
                        return new TransitionResult(TokenizerState.Identifier, TokenizerAction.Read);

                    case '+':
                    case '-':
                        return new TransitionResult(TokenizerState.Sign, TokenizerAction.Read);

                    /*
                     * comments support.
                     */

                    //* cpp-style comments.
                    case '/':
                        return new TransitionResult(TokenizerState.CppStyleCommentStart, TokenizerAction.Read);

                    //* EBNF-style comments.
                    case '(':
                        return new TransitionResult(TokenizerState.EbnfStyleMultiLineCommentStart, TokenizerAction.Read);

                    //* defaults to punctuation.
                    default:
                        return new TransitionResult(TokenizerState.Punctuation, TokenizerAction.Read);
                }

            case CharType.StringDelimiter:
                switch (c)
                {
                    case '\'':
                        return new TransitionResult(TokenizerState.SingleQuoteString, TokenizerAction.Read);

                    case '\"':
                        return new TransitionResult(TokenizerState.DoubleQuoteString, TokenizerAction.Read);

                    /*
                     * TODO: add support for backticks strings (javascript template strings).
                     */
                    // case '`':
                    //     throw new NotImplementedException();

                    default:
                        throw new InvalidOperationException();
                }

            case CharType.Whitespace:
            case CharType.Control:
                return new TransitionResult(TokenizerState.None, TokenizerAction.Skip);

            case CharType.Unknown:
            default:
                return new TransitionResult(TokenizerState.None, TokenizerAction.Error);
        }
    }

}
