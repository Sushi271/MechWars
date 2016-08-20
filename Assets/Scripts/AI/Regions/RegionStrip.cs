using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechWars.AI.Regions
{
    public class RegionStrip
    {
        List<RegionStripPart> parts;

        public int Count { get { return parts.Count; } }
        public bool Empty { get { return Count == 0; } }

        public int Start { get { AssertNotEmpty(); return parts[0].Start; } }
        public int End { get { AssertNotEmpty(); return parts[Count - 1].End; } }

        public RegionStripPart this[int idx]
        {
            get { return parts[idx]; }
        }

        public RegionStrip()
        {
            parts = new List<RegionStripPart>();
        }

        public bool IsInside(int y)
        {
            return parts.Any(p => p.IsInside(y));
        }

        public void AddTile(int y)
        {
            var adjacentParts = parts.Where(p =>
                Mathf.Abs(p.Start - y) <= 1 || Mathf.Abs(p.End - y) <= 1);
            if (adjacentParts.Any(p => p.IsInside(y)))
                throw new System.Exception("Tile at {0} is already inside RegionStrip.");

            int count = adjacentParts.Count();
            if (count == 0)
            {
                int idx;
                for (idx = 0; idx < parts.Count; idx++)
                    if (y < parts[idx].Start) break;
                parts.Insert(idx, new RegionStripPart(y, y));
            }
            else
            {
                var part = adjacentParts.First();
                if (y == part.Start - 1)
                    part.Start -= 1;
                else if (y == part.End + 1)
                    part.End += 1;

                if (count == 2)
                {
                    var start = adjacentParts.Min(p => p.Start);
                    var end = adjacentParts.Min(p => p.End);
                    parts.Remove(adjacentParts.Last());
                    part.Start = start;
                    part.End = end;
                }
            }

        }

        public void RemoveTile(int y)
        {
            AssertNotEmpty();

            int index;
            RegionStripPart part = null;
            for (index = 0; index < parts.Count; index++)
                if (parts[index].IsInside(y))
                {
                    part = parts[index];
                    break;
                }
            if (part == null)
                throw new System.Exception("Tile at {0} is not inside RegionStrip.");

            if (part.Start == part.End)
                parts.Remove(part);
            else if (y == part.Start)
                part.Start += 1;
            else if (y == part.End)
                part.End -= 1;
            else
            {
                var start2 = y + 1;
                var end2 = part.End;
                part.End = y - 1;
                parts.Insert(index + 1, new RegionStripPart(start2, end2));
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder()
                .AppendFormat("Count({0}), ", Count)
                .AppendFormat("Empty({0}), ", Empty);
            if (!Empty) sb
                .AppendFormat("Start({0}), ", Start)
                .AppendFormat("End({0}), ", End);

            sb.Append("Parts: ");
            for (int i = 0; i < Count; i++)
                sb.AppendFormat("[{0}..{1}]", parts[i].Start, parts[i].End);

            return sb.ToString();
        }

        void AssertNotEmpty()
        {
            if (Empty) throw new System.Exception("RegionStrip is empty.");
        }
    }
}
