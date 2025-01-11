namespace Aidan.TextAnalysis.RegularExpressions.Parsing;

public class RegexGrammar
{
    public static string RawFile { get; } = @"
regex
    : union
    ;

union
    : concatenation ('|' concatenation)*
    ;

concatenation
    : quantifier+
    ;

quantifier
    : primary ('*' | '+' | '?')?
    ;

primary
    : group
    | class
    | any
    | fragment
    | $char
    | $escaped_char
    ;

group
    : '(' regex ')'
    ;

class
    : '[' '^'? class_child+ ']'
    ;

class_child
    : class_literal
    | class_range
    ;

class_literal
    : $char
    | $escaped_char
    ;

class_range
    : $char '-' $char
    ;

any
    : '.'
    ;

fragment
    : '@' $id
    ;

";

}
