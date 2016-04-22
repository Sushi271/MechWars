using System.Collections;
using System.Collections.Generic;

namespace MechWars.MapElements.Statistics
{
    public class Stats : IEnumerable<KeyValuePair<string, Stat>>
    {
        Dictionary<string, Stat> attributes;

        public int Count { get { return attributes.Count; } }

        public Stat this[string name]
        {
            get
            {
                Stat a;
                bool success = attributes.TryGetValue(name, out a);
                if (!success) return null;
                return a;
            }
            set
            {
                if (value == null)
                {
                    if (attributes.ContainsKey(name))
                        attributes.Remove(name);
                }
                else attributes[name] = value;
            }
        }

        public Stats()
        {
            attributes = new Dictionary<string, Stat>();
        }

        public Stats(Stats stats)
        {
            attributes = new Dictionary<string, Stat>(stats.attributes);
        }

        public void Add(Stat attribute)
        {
            attributes.Add(attribute.Name, attribute);
        }

        public void Remove(string name)
        {
            attributes.Remove(name);
        }

        public bool ContainsKey(string newName)
        {
            return attributes.ContainsKey(newName);
        }

        public void Clear()
        {
            attributes.Clear();
        }

        public IEnumerator<KeyValuePair<string, Stat>> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
