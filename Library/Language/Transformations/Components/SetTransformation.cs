using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

namespace Aidan.TextAnalysis.Language.Transformations;

public class SetTransformation :
    IEquatable<SetTransformation>,
    IEqualityComparer<SetTransformation>,
    IEnumerable<SetOperation>
{
    public string Name { get; }
    internal SetOperation[] Operations { get; set; }

    [JsonConstructor]
    public SetTransformation(string explanation, params SetOperation[] operations)
    {
        Name = explanation;
        Operations = operations;
    }

    public SetTransformation(params SetOperation[] operations)
    {
        Operations = operations;
        Name = GetDefaultExplanation();
    }

    public static bool operator ==(SetTransformation left, SetTransformation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SetTransformation left, SetTransformation right)
    {
        return !left.Equals(right);
    }

    public static implicit operator SetTransformation(SetOperation[] transformations)
    {
        return new SetTransformation(transformations.ToArray());
    }

    public static implicit operator SetTransformation(List<SetOperation> transformations)
    {
        return new SetTransformation(transformations.ToArray());
    }

    public static implicit operator SetOperation[](SetTransformation transformation)
    {
        return transformation.Operations;
    }

    public bool Equals(SetTransformation? other)
    {
        if (other is null)
        {
            return false;
        }

        return Operations.SequenceEqual(other.Operations);
    }

    public bool Equals(SetTransformation? x, SetTransformation? y)
    {
        if (x is null || y is null)
        {
            return false;
        }

        return x.Equals(y);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetTransformation);
    }

    public int GetHashCode([DisallowNull] SetTransformation obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;

            foreach (var operation in Operations)
            {
                hash = hash * 23 + operation.GetHashCode();
            }

            return hash;
        }
    }

    public IEnumerator<SetOperation> GetEnumerator()
    {
        return ((IEnumerable<SetOperation>)Operations).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return $"{Name}:\n{string.Join("\n", Operations.Select(x => x.ToString()))}";
    }

    private string GetDefaultExplanation()
    {
        var builder = new StringBuilder();

        foreach (var operation in Operations)
        {
            builder.AppendLine(operation.ToString());
        }

        return builder.ToString();
    }

}
