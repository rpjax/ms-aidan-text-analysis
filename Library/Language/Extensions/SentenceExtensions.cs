using Aidan.TextAnalysis.Language.Components;

namespace Aidan.TextAnalysis.Language.Extensions;

public static class SentenceExtensions
{
    /// <summary>
    /// Gets the index of the specified symbol in the sentence. If the symbol is not found, -1 is returned. <br/>
    /// Equality is determined by reference equality. So, two symbols are considered equal if they are the same object.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static int IndexOfSymbol(this Sentence self, Symbol symbol)
    {
        var index = -1;

        foreach (var item in self)
        {
            index++;

            if (ReferenceEquals(item, symbol))
            {
                break;
            }
        }

        return index;
    }

    /*
        read helper methods section.
     */

    public static Symbol GetLeftmostSymbol(this Sentence self)
    {
        return self.First();
    }

    public static Symbol GetRightmostSymbol(this Sentence self)
    {
        return self.Last();
    }

    public static NonTerminal? GetLeftmostNonTerminal(this Sentence self)
    {
        return self
            .OfType<NonTerminal>()
            .FirstOrDefault();
    }

    public static NonTerminal? GetRightmostNonTerminal(this Sentence self)
    {
        return self
            .OfType<NonTerminal>()
            .LastOrDefault();
    }

    public static Terminal? GetLeftmostTerminal(this Sentence self)
    {
        return self
            .OfType<Terminal>()
            .FirstOrDefault();
    }

    public static Terminal? GetRightmostTerminal(this Sentence self)
    {
        return self
            .OfType<Terminal>()
            .LastOrDefault();
    }

    /*
        derivation helper methods section.
     */

    public static Derivation Derive(this Sentence self, int index, ProductionRule production)
    {
        if (index < 0 || index >= self.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The index is out of range.");
        }

        if (self[index] is not NonTerminal nonTerminal)
        {
            throw new InvalidOperationException("The symbol at the specified index is not a non-terminal symbol. Derivations can only be performed on non-terminal symbols.");
        }

        if (nonTerminal != production.Head)
        {
            throw new InvalidOperationException("The non-terminal at the specified index does not match the head of the production rule.");
        }

        var derivedSentence = self;

        derivedSentence = RemoveAt(derivedSentence, index);
        derivedSentence = InsertAt(derivedSentence, index, production.Body);

        return new Derivation(
            production: production,
            originalSentence: self,
            nonTerminal: nonTerminal,
            derivedSentence: derivedSentence
        );
    }

    public static Derivation DeriveLeftmostNonTerminal(this Sentence self, ProductionRule production)
    {
        var nonTerminal = GetLeftmostNonTerminal(self);

        if (nonTerminal is null)
        {
            throw new InvalidOperationException("There are no non-terminals in the sentence.");
        }

        if (nonTerminal != production.Head)
        {
            throw new InvalidOperationException("The leftmost non-terminal in the sentence does not match the head of the production rule.");
        }

        var index = IndexOfSymbol(self, nonTerminal);

        if (index == -1)
        {
            throw new InvalidOperationException("The non-terminal was not found in the sentence.");
        }

        return Derive(self, index, production);
    }

    public static Derivation DeriveRightmostNonTerminal(this Sentence self, ProductionRule production)
    {
        var nonTerminal = GetRightmostNonTerminal(self);

        if (nonTerminal is null)
        {
            throw new InvalidOperationException("There are no non-terminals in the sentence.");
        }

        if (nonTerminal != production.Head)
        {
            throw new InvalidOperationException("The rightmost non-terminal in the sentence does not match the head of the production rule.");
        }

        var index = IndexOfSymbol(self, nonTerminal);

        if (index == -1)
        {
            throw new InvalidOperationException("The non-terminal was not found in the sentence.");
        }

        return Derive(self, index, production);
    }

    public static int[] GetIndexesOfNonTerminal(this Sentence self, NonTerminal nonTerminal)
    {
        var indexes = new List<int>();

        for (var i = 0; i < self.Length; i++)
        {
            if (self[i] == nonTerminal)
            {
                indexes.Add(i);
            }
        }

        return indexes.ToArray();
    }

    /*
        helper methods section.
    */

    public static Sentence Add(this Sentence self, Symbol symbol)
    {
        var list = new List<Symbol>(self.ToArray());
        list.Add(symbol);
        return list;
    }

    public static Sentence Add(this Sentence self, IEnumerable<Symbol> symbols)
    {
        var list = new List<Symbol>(self.ToArray());
        list.AddRange(symbols);
        return list;
    }

    public static Sentence InsertAt(this Sentence self, int index, Symbol symbol)
    {
        var list = new List<Symbol>(self.ToArray());
        list.Insert(index, symbol);
        return list;
    }

    public static Sentence InsertTerminalAt(this Sentence self, int index, string value)
    {
        var list = new List<Symbol>(self.ToArray());
        list.Insert(index, Terminal.From(value));
        return list;
    }

    public static Sentence InsertAt(this Sentence self, int index, IEnumerable<Symbol> symbols)
    {
        var list = new List<Symbol>(self.ToArray());
        list.InsertRange(index, symbols);
        return list;
    }

    public static Sentence RemoveAt(this Sentence self, int index)
    {
        var list = new List<Symbol>(self.ToArray());
        list.RemoveAt(index);
        return list;
    }

    public static Sentence Replace(this Sentence self, Symbol symbol, params Symbol[] replacement)
    {
        var symbols = new List<Symbol>();

        foreach (var item in self)
        {
            if (item == symbol)
            {
                symbols.AddRange(replacement);
            }
            else
            {
                symbols.Add(item);
            }
        }

        return symbols;
    }

    public static Sentence GetRange(this Sentence self, int start, int count)
    {
        if(start < 0 || start >= self.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(start), "The start index is out of range.");
        }
        if(count < 0 || start + count > self.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "The count is out of range.");
        }

        return self
            .Skip(start)
            .Take(count)
            .ToArray();
    }

}

