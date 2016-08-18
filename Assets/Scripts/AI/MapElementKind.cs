using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI
{
    public class MapElementKind
    {
        public string Name { get; private set; }

        Dictionary<string, MapElementPurpose> abilities;

        public MapElementKind(string name, params MapElementPurpose[] abilities)
        {
            Name = name;
            this.abilities = new Dictionary<string, MapElementPurpose>();
            foreach (var a in abilities)
                this.abilities.Add(a.Name, a);

            NormalizeAbilities();
        }

        void NormalizeAbilities()
        {
            var sum = abilities.Sum(kv => kv.Value.Value);
            foreach (var a in abilities.Values)
                a.Value /= sum;
        }
    }
}