using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class SymbolExtensions 
{
    public static Terminal AsTerminal(this Symbol symbol)
    {
        if (symbol is not Terminal terminal)
        {
            throw new InvalidCastException("The production symbol is not a terminal symbol.");
        }

        return terminal;
    }

    public static NonTerminal AsNonTerminal(this Symbol symbol)
    {
        if (symbol is not NonTerminal nonTerminal)
        {
            throw new InvalidCastException("The production symbol is not a non-terminal symbol.");
        }

        return nonTerminal;
    }

    public static MacroSymbol AsMacro(this Symbol symbol)
    {
        if (symbol is not MacroSymbol productionMacro)
        {
            throw new InvalidCastException("The production symbol is not a macro.");
        }

        return productionMacro;
    }

}

public static class ISymbolExtensions
{
    public static Terminal AsTerminal(this ISymbol symbol)
    {
        if (symbol is not Terminal terminal)
        {
            throw new InvalidCastException("The production symbol is not a terminal symbol.");
        }

        return terminal;
    }

    public static NonTerminal AsNonTerminal(this ISymbol symbol)
    {
        if (symbol is not NonTerminal nonTerminal)
        {
            throw new InvalidCastException("The production symbol is not a non-terminal symbol.");
        }

        return nonTerminal;
    }

    public static MacroSymbol AsMacro(this ISymbol symbol)
    {
        if (symbol is not MacroSymbol productionMacro)
        {
            throw new InvalidCastException("The production symbol is not a macro.");
        }

        return productionMacro;
    }

}
