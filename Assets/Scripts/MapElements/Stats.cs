using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.MapElements;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Stats : IEnumerable<KeyValuePair<string, Attribute>>
    {
        Dictionary<string, Attribute> attributes;

        public int Count { get { return attributes.Count; } }

        public Attribute this[string name]
        {
            get
            {
                Attribute a;
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
            attributes = new Dictionary<string, Attribute>();
        }

        public Stats(Stats stats)
        {
            attributes = new Dictionary<string, Attribute>(stats.attributes);
        }

        public void Add(Attribute attribute)
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

        public IEnumerator<KeyValuePair<string, Attribute>> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
