using Aidan.TextAnalysis.Parsing.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aidan.TextAnalysis.Parsing.Core;

/// <summary>
/// Represents a parser that is capable of parsing text based on a given grammar.
/// </summary>
public interface IStringParser
{
    /// <summary>
    /// Parses the given text and returns the concrete syntax tree (CST) based on the grammar specified in the constructor.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    CstRootNode Parse(string text);
}
