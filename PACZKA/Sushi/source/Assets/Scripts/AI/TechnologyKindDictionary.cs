using MechWars.MapElements;
using System.Collections.Generic;

namespace MechWars.AI
{
    public class TechnologyKindDictionary
    {
        Dictionary<string, TechnologyKind> dict;

        public TechnologyKind this[string technologyName]
        {
            get { return dict[technologyName]; }
        }

        public TechnologyKindDictionary(AIBrain brain)
        {
            dict = new Dictionary<string, TechnologyKind>();

            InitializeDictionary(brain);
        }

        void InitializeDictionary(AIBrain brain)
        {
        }

        void Add(TechnologyKind kind)
        {
            dict.Add(kind.Name, kind);
        }
    }
}