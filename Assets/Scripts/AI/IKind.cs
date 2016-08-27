using System.Collections.Generic;

namespace MechWars.AI
{
    public interface IKind
    {
        string Name { get; }
        List<CreationMethod> CreationMethods { get; }
    }
}