namespace Aidan.TextAnalysis.Tokenization.Machine;

public interface ITokenResult : ITransitionResult
{
    TokenType TokenType { get; }
}
