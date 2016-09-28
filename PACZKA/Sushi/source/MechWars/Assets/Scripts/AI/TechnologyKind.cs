using MechWars.MapElements;
using System.Collections.Generic;

namespace MechWars.AI
{
    public class TechnologyKind : IKind
    {
        public string Name { get; private set; }
        public Technology Technology { get; private set; }

        public List<CreationMethod> CreationMethods { get; private set; }

        public TechnologyKind(Technology technology)
        {
            Name = technology.technologyName;
            Technology = technology;
            CreationMethods = new List<CreationMethod>();
        }
    }
}