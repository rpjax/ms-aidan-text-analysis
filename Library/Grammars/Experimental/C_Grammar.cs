using Aidan.TextAnalysis.Language.Components;
using Aidan.TextAnalysis.Tokenization;

namespace Aidan.TextAnalysis.Language.Grammars;

public class C_Grammar : Grammar
{
    public C_Grammar() : base(GetStart(), GetProductions())
    {
    }

    private static ProductionRule[] GetProductions()
    {
        return new ProductionRule[]
        {
            // translation-unit:
            new ProductionRule("translation-unit", new NonTerminal("external-declaration")),
            
            //external-declaration:
            new ProductionRule("external-declaration", new NonTerminal("function-definition")),

            new ProductionRule("external-declaration", new NonTerminal("declaration")),

            //function-definition:
            new ProductionRule("function-definition", new NonTerminal("declaration-specifiers"), new NonTerminal("declarator"), new NonTerminal("declaration-list"), new NonTerminal("compound-statement")),

            new ProductionRule("function-definition", new NonTerminal("declaration-specifiers"), new NonTerminal("declarator"), new NonTerminal("compound-statement")),

            //declaration:
            new ProductionRule("declaration", new NonTerminal("declaration-specifiers"), new NonTerminal("init-declarator-list"), new Terminal(TokenType.Punctuation, ";")),

            new ProductionRule("declaration", new NonTerminal("declaration-specifiers"), new Terminal(TokenType.Punctuation, ";")),

            new ProductionRule("declaration", new NonTerminal("static-assert-declaration")),

            new ProductionRule("declaration", new Terminal(TokenType.Punctuation, ";")),

            //declaration-specifiers:
            new ProductionRule("declaration-specifiers", new NonTerminal("declaration-specifier"), new NonTerminal("declaration-specifiers")),

            new ProductionRule("declaration-specifiers", new Epsilon()),

            //declaration-specifier:
            new ProductionRule("declaration-specifier", new NonTerminal("storage-class-specifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("type-specifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("type-qualifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("function-specifier")),

            new ProductionRule("declaration-specifier", new NonTerminal("alignment-specifier")),

            //declarator:   
            new ProductionRule(
                "declarator",
                new Sentence(
                    new OptionMacro(
                        new NonTerminal("pointer")
                    ),
                    new NonTerminal("direct-declarator")
                )
            ),

            //declaration-list:
            new ProductionRule(
                "declaration-list",
                new Sentence(
                    new NonTerminal("declaration"),
                    new RepetitionMacro(
                        new NonTerminal("declaration")
                    )
                )
            ),

            // compound-statement:
            new ProductionRule(
                "compound-statement",
                new Sentence(
                    new Terminal(TokenType.Punctuation, "{"),
                    new RepetitionMacro(
                        new NonTerminal("declaration-or-statement")
                    ),
                    new Terminal(TokenType.Punctuation, "}")
                )
            ),

            //declaration-or-statement:
            new ProductionRule(
                "declaration-or-statement",
                new Sentence(
                    new NonTerminal("declaration"),
                    new AlternativeMacro(),
                    new NonTerminal("statement")
                )
            ),

            //init-declarator-list:
            new ProductionRule(
                "init-declarator-list",
                new Sentence(
                    new NonTerminal("init-declarator"),
                    new RepetitionMacro(
                        new Terminal(TokenType.Punctuation, ","),
                        new NonTerminal("init-declarator")
                    )
                )
            ),

            //init-declarator:
            new ProductionRule(
                "init-declarator",
                new Sentence(
                    new NonTerminal("declarator"),
                    new OptionMacro(
                        new Terminal(TokenType.Punctuation, "="),
                        new NonTerminal("initializer")
                    )
                )
            ),

            //static-assert-declaration:
            new ProductionRule(
                "static-assert-declaration",
                new Sentence(
                    new Terminal(TokenType.Identifier, "static_assert"),
                    new Terminal(TokenType.Punctuation, "("),
                    new NonTerminal("constant-expression"),
                    new Terminal(TokenType.Punctuation, ","),
                    new Terminal(TokenType.String),
                    new Terminal(TokenType.Punctuation, ")"),
                    new Terminal(TokenType.Punctuation, ";")
                )
            ),

            //storage-class-specifier:
            new ProductionRule(
                "storage-class-specifier",
                new Sentence(
                    new Terminal(TokenType.Identifier, "typedef"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "extern"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "static"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "Thread_local"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "auto"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "register")
                )
            ),

            //type-specifier:
            new ProductionRule(
                "type-specifier",
                new Sentence(
                    new Terminal(TokenType.Identifier, "void"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "char"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "short"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "int"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "long"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "float"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "double"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "signed"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "unsigned"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "_Bool"),
                    new AlternativeMacro(),
                    new Terminal(TokenType.Identifier, "_Complex"),
                    new AlternativeMacro(),
                    new NonTerminal("_Imaginary")
                )
            ),

            //struct-or-union-specifier:
            new ProductionRule(
                "struct-or-union-specifier",
                new Sentence(
                    new NonTerminal("enum-specifier"),
                    new AlternativeMacro(),
                    new NonTerminal("typedef-name")
                )
            ),

            /*
             *  It seems that the C99 grammar i got from some github repo is not complete, so i decided to leave this out for now.
             *  It also seems that C99 is quite complex, maybe a simpler language would be better for testing purposes.
             */
        };
    }

    private static NonTerminal GetStart()
    {
        return new NonTerminal("translation-unit");
    }
}
