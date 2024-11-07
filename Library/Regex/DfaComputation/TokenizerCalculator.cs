using Aidan.TextAnalysis.Regexes.Ast;
using Aidan.TextAnalysis.Regexes.Ast.Extensions;
using Aidan.TextAnalysis.Tokenization.StateMachine;

namespace Aidan.TextAnalysis.Regexes.DfaComputation;

public class TokenizerCalculator
{
    private RegexLexeme[] Lexemes { get; }
    private RegexDfaCalculator DfaCalculator { get; }

    public TokenizerCalculator(params RegexLexeme[] lexemes)
    {
        Lexemes = lexemes.ToArray();
        DfaCalculator = new RegexDfaCalculator(Lexemes, ' ', '\0');
    }

    public static void Test()
    {
        var fooRegex = new ConcatenationNode(
            new LiteralNode('f'),
            new ConcatenationNode(
                new LiteralNode('o'),
                new LiteralNode('o')
            )
        );

        var foobarRegex = new ConcatenationNode(
            new LiteralNode('f'),
            new ConcatenationNode(
                new LiteralNode('o'),
                new ConcatenationNode(
                    new LiteralNode('o'),
                    new ConcatenationNode(
                        new LiteralNode('b'),
                        new ConcatenationNode(
                            new LiteralNode('a'),
                            new LiteralNode('r')
                        )
                    )
                )
            )
        );

        var useRegex = new ConcatenationNode(
            new LiteralNode('u'),
            new ConcatenationNode(
                new LiteralNode('s'),
                new LiteralNode('e')
            )
        );

        var lexemes = new[]
        {
            new RegexLexeme("foo_statement", fooRegex),
            new RegexLexeme("foobar_statement", foobarRegex),
            new RegexLexeme("use_statement", useRegex)
        };

        var tokenizer = new TokenizerCalculator(lexemes)
            .CreateTokenizer();

        var tokens = tokenizer
            .Tokenize(" foo foo foobar foo foobar use")
            .ToArray();
    }

    public TokenizerMachine CreateTokenizer()
    {
        var dfa = DfaCalculator.ComputeRegexDfa();
        var transitions = dfa.States
            .SelectMany(x => x.Transitions)
            .ToArray();

        var builder = new TokenizerDfaBuilder(dfa.States.First().Name);

        builder.SetCharset(dfa.Alphabet.ToArray());

        builder.FromInitialState()
            .OnWhitespace()
            .OnEoi()
            .Recurse();

        foreach (var state in dfa.States)
        {
            foreach (var transition in state.Transitions)
            {
                var derivativeIsAccepting = transition.Derivative.IsEpsilon();
                var sourceContainsEpsilon = transition.Source.ContainsEpsilon;
                var stateName = transition.Derivative.ToString();

                if (derivativeIsAccepting)
                {
                    var lexeme = transition.Derivative.GetLexeme();

                    if (lexeme is null)
                    {
                        throw new InvalidOperationException("Lexeme is null");
                    }

                    if (sourceContainsEpsilon)
                    {
                        builder
                            .FromState(transition.Source.ToString())
                            .OnCharacter(transition.Character)
                            .Accept(lexeme.Name)
                            ;
                    }
                    else
                    {
                        var intermediateName = Guid.NewGuid().ToString();

                        builder
                            .FromState(transition.Source.ToString())
                            .OnCharacter(transition.Character)
                            .GoTo(intermediateName)
                            ;

                        builder
                            .FromState(intermediateName)
                            .OnAnyCharacter()
                            .Accept(lexeme.Name)
                            ;
                    }
                }
                else
                {
                    builder
                        .FromState(transition.Source.ToString())
                        .OnCharacter(transition.Character)
                        .GoTo(transition.Derivative.ToString());
                }

            }
        }

        return builder.Build();
    }

}

