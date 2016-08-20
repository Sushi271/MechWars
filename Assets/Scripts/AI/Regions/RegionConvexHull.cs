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
        public Vector2 Center { get; private set; }
        public Rect Bounds { get; private set; }

        public RegionConvexHull(RegionHull hull)
        {
            var t1 = System.DateTime.Now;

            Points = GrahamScan(hull.Points);
            Center = Points.Average2();
            Bounds = Points.AsBounds();

            timeTakenMs = (System.DateTime.Now - t1).TotalMilliseconds;
        }

        List<Vector2> GrahamScan(List<Vector2> points)
        {
            if (points.Empty()) throw new System.Exception("Cannot apply GrahamScan to empty list of points.");

            Vector2 anchor = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                var p = points[i];
                if (p.y < anchor.y || p.y == anchor.y && p.x < anchor.x)
                    anchor = p;
            }

            var sortedPoints =
                (from p in points
                 let aToP = anchor - p
                 let d = aToP.magnitude
                 let cos = p == anchor ? 2 : aToP.x / d // dot with (1, 0)
                 orderby cos
                 select p).ToArray();

            if (!points.HasAtLeast(4)) return new List<Vector2>(sortedPoints);

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

        public bool Contains(Vector2 point)
        {
            if (!Bounds.ContainsWithBorder(point)) return false;

            if (Points.Count == 1)
                return point == Points.First();
            if (Points.Count == 2)
                return point.LiesOnSegment(Points.First(), Points.Last(), 0.0001f);

            if (point.LiesOnSegment(Points.First(), Points[1], 0.0001f) ||
                point.LiesOnSegment(Points.Last(), Points.First(), 0.0001f))
                return true;

            // here we know for sure that:
            // - there are at least 3 points
            // - point does not lie on segments connected with anchor

            var anchor = Points[0];
            if (point == anchor) return true;
            if (point.y < anchor.y) return false; // anchor is the lowest point

            var toPoint = point - anchor;
            var cosAngleToPoint = toPoint.x / toPoint.magnitude;

            Vector2 previous = Points[0];
            Vector2 next = Points[1];
            bool found = false;
            for (int i = 2; i < Points.Count; i++)
            {
                var current = Points[i];
                var toCurrent = current - anchor;
                var cosAngle = toCurrent.x / toCurrent.magnitude;
                if (cosAngle <= cosAngleToPoint)
                {
                    next = current;
                    previous = Points[i - 1];
                    found = true;
                    break;
                }
            }
            if (!found) return false;

            if (point.LiesOnSegment(previous, next, 0.0001f)) return true;
            if (!UnityExtensions.SegmentsIntersectOrCover(anchor, point, previous, next)) return true;
            return false;
        }

        public Vector2 GetPointClosestTo(Vector2 point)
        {
            if (Contains(point)) return point;

            Vector2 vertexBefore = default(Vector2);
            Vector2 closestVertex = default(Vector2);
            Vector2 vertexAfter = default(Vector2);
            float minDistSqr = float.MaxValue;

            for (int i = 0; i < Points.Count; i++)
            {
                var distSqr = (point - Points[i]).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    minDistSqr = distSqr;
                    vertexBefore = Points[i - 1 < 0 ? Points.Count - 1 : i - 1];
                    closestVertex = Points[i];
                    vertexAfter = Points[(i + 1) % Points.Count];
                }
            }

            var toPoint = point - closestVertex;

            var toBeforeNormalized = (vertexBefore - closestVertex).normalized;
            var castOnSegmentBefore = closestVertex + toBeforeNormalized * Vector2.Dot(toBeforeNormalized, toPoint);
            if (castOnSegmentBefore == closestVertex) return closestVertex;
            if (castOnSegmentBefore.LiesOnSegment(closestVertex, vertexBefore, 0.0001f)) return castOnSegmentBefore;

            var toAfterNormalized = (vertexAfter - closestVertex).normalized;
            var castOnSegmentAfter = closestVertex + toAfterNormalized * Vector2.Dot(toAfterNormalized, toPoint);
            if (castOnSegmentAfter == closestVertex) return closestVertex;
            if (castOnSegmentAfter.LiesOnSegment(closestVertex, vertexAfter, 0.0001f)) return castOnSegmentAfter;

            return closestVertex;
        }

        public override string ToString()
        {
            // DO NOT USE AsBounds() HERE - DISCRETE W & H NEEDED, AsBounds() IS CONTINOUS
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
