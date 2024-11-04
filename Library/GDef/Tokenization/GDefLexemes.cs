using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aidan.TextAnalysis.GDef.Tokenization;

public static class GDefLexemes
{
    public static string Identifier { get; } = "identifier";
    public static string String { get; } = "string";
    public static string Integer { get; } = "int";
    public static string Float { get; } = "float";
    public static string Hexadecimal { get; } = "hex";
}
