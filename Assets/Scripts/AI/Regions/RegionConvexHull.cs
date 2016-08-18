using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechWars.AI.Regions
{
    public class RegionConvexHull
    {
        double timeTakenMs;

        public List<Vector2> Points { get; private set; }

        public RegionConvexHull(RegionHull hull)
        {
            var t1 = System.DateTime.Now;

            Points = GrahamScan(hull.Points);

            var t2 = System.DateTime.Now;
            var delta = t2 - t1;
            timeTakenMs = delta.TotalMilliseconds;
        }

        List<Vector2> GrahamScan(List<Vector2> points)
        {
            if (points.HasAtLeast(4))
            {
                Vector2 anchor = points[0];
                for (int i = 1; i < points.Count; i++)
                {
                    var p = points[i];
                    if (p.y < anchor.y || p.y == anchor.y && p.x < anchor.x)
                        anchor = p;
                }

                var vx = new Vector2(1, 0);

                var sortedPoints =
                    (from p in points
                     let aToP = anchor - p
                     let d = aToP.magnitude
                     let c = p == anchor ? 2 : Vector2.Dot(aToP / d, vx)
                     orderby c
                     select p).ToArray();

                var convexPoints = new List<Vector2>(sortedPoints.Length);
                convexPoints.Add(sortedPoints[0]);
                convexPoints.Add(sortedPoints[1]);

                for (int i = 2; i < sortedPoints.Length; i++)
                {
                    var p3 = sortedPoints[i];
                    bool leftTurn;
                    do
                    {
                        var p1 = convexPoints[convexPoints.Count - 2];
                        var p2 = convexPoints[convexPoints.Count - 1];

                        var v12 = p2 - p1;
                        var v23 = p3 - p2;
                        leftTurn = v12.x * v23.y - v12.y * v23.x > 0;

                        if (!leftTurn)
                            convexPoints.RemoveAt(convexPoints.Count - 1);
                    }
                    while (!leftTurn && convexPoints.Count > 1);

                    convexPoints.Add(p3);
                }

                return convexPoints;
            }
            else return new List<Vector2>(points);
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
