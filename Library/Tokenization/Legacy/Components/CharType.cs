namespace Aidan.TextAnalysis.Tokenization.Components;

/// <summary>
/// Represents the type of a character in the source text. It caracterizes <see cref="char"/> into a specific category.
/// </summary>
public enum CharType
{
    Digit,
    Letter,
    Punctuation,
    StringDelimiter,
    Whitespace,
    Control,
    Unknown
}

public enum CharSpecificType
{
    /*
     * digits
     */
    Zero,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,

    /*
     * letters (uppercase)
     */
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,

    /*
     * letters (lowercase)
     */
    a,
    b,
    c,
    d,
    e,
    f,
    g,
    h,
    i,
    j,
    k,
    l,
    m,
    n,
    o,
    p,  
    q,
    r,
    s,
    t,
    u,
    v,
    w,
    x,
    y,
    z,

}
