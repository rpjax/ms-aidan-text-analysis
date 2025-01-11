namespace Aidan.TextAnalysis.Parsing.Tree;

public interface ICstReducer
{
    CstNode Reduce(CstNode[] children);
}
