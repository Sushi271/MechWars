using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechWars.AI.Regions
{
    public class RegionHull
    {
        IVector2[] directionsStraight =
        {
            new IVector2(1, 0),
            new IVector2(0, 1),
            new IVector2(-1, 0),
            new IVector2(0, -1)
        };

        IVector2[] directionsWithDiagonal =
        {
            new IVector2(1, 0),
            new IVector2(1, 1),
            new IVector2(0, 1),
            new IVector2(-1, 1)
        };

        double timeTakenMs;

        public List<Vector2> Points { get; private set; }

        public RegionHull(Region region)
        {
            var t1 = System.DateTime.Now;

            Points = new List<Vector2>();

            if (region.Width == 0) return;

            var stripsTable = region.ToStripsTable();
            var nonEmptyStrips = stripsTable.Where(rs => rs.Count > 0);
            if (nonEmptyStrips.Empty()) return;

            int minStart = nonEmptyStrips.Min(rs => rs.Start);
            int maxEnd = nonEmptyStrips.Max(rs => rs.End);

            Points = GetBorderPoints(region, minStart, maxEnd);

            timeTakenMs = (System.DateTime.Now - t1).TotalMilliseconds;
        }

        List<Vector2> GetBorderPoints(Region region, int minStart, int maxEnd)
        {
            var borderPoints = new List<Vector2>();

            for (int y = minStart; y <= maxEnd; y++)
                for (int x = region.Left; x <= region.Right; x++)
                {
                    var point = new IVector2(x, y);
                    if (IsBorderPoint(region, point))
                        borderPoints.Add(point);
                }

            return borderPoints;
        }

        bool IsBorderPoint(Region region, IVector2 point)
        {
            if (!region.IsInside(point)) return false;
            return directionsStraight.Any(d => !region.IsInside(point + d));
        }

        public override string ToString()
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            foreach (var p in Points)
            {
                if (p.x < minX) minX = p.x;
                if (p.y < minY) minY = p.y;
                if (p.x > maxX) maxX = p.x;
                if (p.y > maxY) maxY = p.y;
            }
            int w = (int)(maxX - minX + 1);
            int h = (int)(maxY - minY + 1);
            bool[,] pts = new bool[w, h];
            foreach (var p in Points)
                pts[(int)(p.x - minX), (int)(p.y - minY)] = true;

            var sb = new StringBuilder();
            sb.AppendFormatLine("X: {0}..{1}", (int)minX, (int)maxX);
            sb.AppendFormatLine("Y: {0}..{1}", (int)minY, (int)maxY);
            sb.AppendLine();
            sb.AppendFormatLine("Points: {0}", Points.ToDebugMessage());
            sb.AppendLine();

            for (int j = h - 1; j >= 0; j--)
            {
                for (int i = 0; i < w; i++)
                    if (pts[i, j]) sb.Append('x');
                    else sb.Append('.');
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendFormatLine("Time taken: {0} ms", timeTakenMs);

            return sb.ToString();
        }
    }
}
