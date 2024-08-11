using System.Collections;
using System.Text;
using System.Text.Json.Serialization;

namespace Aidan.TextAnalysis.Language.Transformations;

public class SetTransformationCollection : 
    IEnumerable<SetTransformation>
{
    private SetTransformation[] Transformations { get; set; }

    [JsonConstructor]
    public SetTransformationCollection(params SetTransformation[] transformations)
    {
        Transformations = transformations;
    }

    public static implicit operator SetTransformationCollection(SetTransformation[] transformations)
    {
        return new SetTransformationCollection(
            transformations: transformations
        );
    }

    public static implicit operator SetTransformationCollection(List<SetTransformation> transformations)
    {
        return new SetTransformationCollection(
            transformations: transformations.ToArray()
        );
    }

    public static implicit operator SetTransformation[](SetTransformationCollection collection)
    {
        return collection.Transformations;
    }

    public IEnumerator<SetTransformation> GetEnumerator()
    {
        return ((IEnumerable<SetTransformation>)Transformations).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Transformations.GetEnumerator();
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var transformation in Transformations)
        {
            builder.AppendLine(transformation.ToString());
        }

        return builder.ToString();
    }

}
