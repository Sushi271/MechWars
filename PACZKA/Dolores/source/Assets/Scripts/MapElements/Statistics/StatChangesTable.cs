using System.Collections.Generic;

namespace MechWars.MapElements.Statistics
{
    public class StatChangesTable
    {
        Dictionary<string, Dictionary<string, HashSet<Stat>>> table;

        public HashSet<Stat> this[string mapElementName, string statName]
        {
            get
            {
                Dictionary<string, HashSet<Stat>> subTable;
                var success = table.TryGetValue(mapElementName, out subTable);
                if (!success)
                {
                    subTable = new Dictionary<string, HashSet<Stat>>();
                    table[mapElementName] = subTable;
                }

                HashSet<Stat> value;
                success = subTable.TryGetValue(statName, out value);
                if (!success)
                {
                    value = new HashSet<Stat>();
                    subTable[statName] = value;
                }

                return value;
            }
        }

        public StatChangesTable()
        {
            table = new Dictionary<string, Dictionary<string, HashSet<Stat>>>();
        }
    }
}