using System.Collections;

namespace Aidan.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents a collection of LR(1) states.
/// </summary>
public class LR1StateCollection : IEnumerable<LR1State>
{
    private LR1State[] States { get; }

    public LR1StateCollection(IEnumerable<LR1State> states)
    {
        States = states.ToArray();
    }

    public int Length => States.Length;

    public IEnumerator<LR1State> GetEnumerator()
    {
        return ((IEnumerable<LR1State>)States).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<LR1State>)States).GetEnumerator();
    }

    /// <summary>
    /// Gets the state id.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public int GetStateId(LR1State state)
    {
        var index = -1;

        for (int i = 0; i < States.Length; i++)
        {
            if (States[i].Equals(state))
            {
                if(index != -1)
                {
                    throw new InvalidOperationException("The state is not unique. This error represents a bug in the LR1 state collection computation.");
                }

                index = i;
                break;
            }
        }

        return index;
    }

    /// <summary>
    /// Gets the state id by kernel.
    /// </summary>
    /// <param name="kernel"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public int GetStateIdByKernel(LR1Kernel kernel)
    {
        var index = -1;

        for (int i = 0; i < States.Length; i++)
        {
            if (States[i].Kernel.Equals(kernel))
            {
                if (index != -1)
                {
                    throw new InvalidOperationException("The kernel is not unique. This error represents a bug in the LR1 state collection computation.");
                }

                index = i;
                break;
            }
        }

        return index;
    }

}
