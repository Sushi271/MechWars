using System.Collections;
using System.Collections.Generic;

namespace MechWars.MapElements.Statistics
{
    public class Stats : IEnumerable<KeyValuePair<string, Stat>>
    {
        Dictionary<string, Stat> statistics;
        
        public int Count { get { return statistics.Count; } }

        public Stat this[string name]
        {
            get
            {
                Stat a;
                bool success = statistics.TryGetValue(name, out a);
                if (!success) return null;
                return a;
            }
            set
            {
                if (value == null)
                {
                    if (statistics.ContainsKey(name))
                        statistics.Remove(name);
                }
                else statistics[name] = value;
            }
        }

        public Stats()
        {
            statistics = new Dictionary<string, Stat>();
        }

        public void Add(Stat stat)
        {
            statistics.Add(stat.Name, stat);
        }

        public void Remove(string name)
        {
            statistics.Remove(name);
        }

        public bool ContainsKey(string newName)
        {
            return statistics.ContainsKey(newName);
        }

        public void Clear()
        {
            statistics.Clear();
        }

        public Stats Clone(MapElement newOwner)
        {
            var newStats = new Stats();
            foreach (var kv in statistics)
            {
                newStats[kv.Key] = this[kv.Key].Clone(newOwner);
            }
            return newStats;
        }

        public IEnumerator<KeyValuePair<string, Stat>> GetEnumerator()
        {
            return statistics.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
