using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Text;

namespace MechWars.Mapping
{
    public class QuadTree
    {
        QuadTreeMapElement QuadTreeMapElement;

        SquareBounds bounds;

        QuadTree x0y0;
        QuadTree x0y1;
        QuadTree x1y0;
        QuadTree x1y1;

        bool HasChildren { get { return x0y0 != null; } }
        bool IsEmpty { get { return QuadTreeMapElement == null && !HasChildren; } }

        public QuadTree(SquareBounds bounds)
        {
            if (!bounds.SquareSize.IsPowerOfTwo())
                throw new System.Exception("QuadTree Bounds size must be power of two.");
            this.bounds = bounds;
        }

        public void Insert(MapElement mapElement)
        {
            var coordsList = Globals.Map[mapElement];
            foreach (var c in coordsList)
                InsertCore(new QuadTreeMapElement(mapElement, c));
        }

        bool InsertCore(QuadTreeMapElement quadTreeMapElement)
        {
            var mapElement = quadTreeMapElement.MapElement;
            var coords = quadTreeMapElement.Coords;

            if (!bounds.ContainsPoint(coords))
                return false;

            if (!HasChildren)
            {
                if (QuadTreeMapElement == null)
                {
                    QuadTreeMapElement = quadTreeMapElement;
                    return true;
                }
                else if (
                    QuadTreeMapElement.MapElement == mapElement &&
                    QuadTreeMapElement.Coords == coords)
                    throw new System.Exception("Cannot insert MapElement twice into the same coords.");
                else Subdivide();
            }

            if (x0y0.InsertCore(quadTreeMapElement)) return true;
            if (x0y1.InsertCore(quadTreeMapElement)) return true;
            if (x1y0.InsertCore(quadTreeMapElement)) return true;
            if (x1y1.InsertCore(quadTreeMapElement)) return true;

            throw new System.Exception("Cannot insert MapElement for unknown reason (this should never happen.)");
        }

        void Subdivide()
        {
            if (bounds.SquareSize == 1)
                throw new System.Exception("Cannot subdivide, because Bounds size is 1 (undividable).");

            var dividedSize = bounds.SquareSize / 2;
            var halfwayX = bounds.Location.X + dividedSize;
            var halfwayY = bounds.Location.Y + dividedSize;

            x0y0 = new QuadTree(new SquareBounds(new IVector2(bounds.X0, bounds.Y0), dividedSize));
            x0y1 = new QuadTree(new SquareBounds(new IVector2(bounds.X0, halfwayY), dividedSize));
            x1y0 = new QuadTree(new SquareBounds(new IVector2(halfwayX, bounds.Y0), dividedSize));
            x1y1 = new QuadTree(new SquareBounds(new IVector2(halfwayX, halfwayY), dividedSize));

            var qtme = QuadTreeMapElement;
            QuadTreeMapElement = null;

            if (x0y0.InsertCore(qtme)) return;
            if (x0y1.InsertCore(qtme)) return;
            if (x1y0.InsertCore(qtme)) return;
            if (x1y1.InsertCore(qtme)) return;
        }

        public void Remove(MapElement mapElement)
        {
            var coordsList = Globals.Map[mapElement];
            foreach (var c in coordsList)
                RemoveCore(c);
        }

        RemoveCoreResult RemoveCore(IVector2 coords)
        {
            if (!bounds.ContainsPoint(coords))
                return RemoveCoreResult.OutOfBounds;

            if (!HasChildren)
            {
                if (QuadTreeMapElement == null ||
                    QuadTreeMapElement.Coords != coords)
                    return RemoveCoreResult.WontFind;

                QuadTreeMapElement = null;
                return RemoveCoreResult.Adjusting;
            }

            List<QuadTree> children = new List<QuadTree> { x0y0, x0y1, x1y0, x1y1 };
            for (int i = 0; i < 4; i++)
            {
                var result = children[i].RemoveCore(coords);
                if (result == RemoveCoreResult.Adjusting)
                    break;
                if (result == RemoveCoreResult.Done ||
                    result == RemoveCoreResult.WontFind)
                    return result;
                if (i == 3 && result == RemoveCoreResult.OutOfBounds)
                    throw new System.Exception("Coords out of bounds for all of 4 children (this should never happen).");
            }

            if (TryUnsubdivide()) return RemoveCoreResult.Adjusting;
            else return RemoveCoreResult.Done;
        }

        bool TryUnsubdivide()
        {
            List<QuadTree> children = new List<QuadTree> { x0y0, x0y1, x1y0, x1y1 };

            int emptyCount = 0;
            foreach (var child in children)
                if (child.IsEmpty) emptyCount++;
            if (emptyCount == 4)
                throw new System.Exception("All of 4 children are empty (this should never happen).");
            if (emptyCount < 3)
                return false;

            var nonEmptyChild = children.Find(c => !c.IsEmpty);
            if (nonEmptyChild.HasChildren)
                return false;

            QuadTreeMapElement = nonEmptyChild.QuadTreeMapElement;
            nonEmptyChild.QuadTreeMapElement = null;

            x0y0 = null;
            x0y1 = null;
            x1y0 = null;
            x1y1 = null;

            return true;
        }

        public List<QuadTreeMapElement> QueryRange(IRectangleBounds range)
        {
            var mapElements = new List<QuadTreeMapElement>();

            if (!bounds.IntersectsOther(range))
                return mapElements;

            if (QuadTreeMapElement != null &&
                range.ContainsPoint(QuadTreeMapElement.Coords))
                mapElements.Add(QuadTreeMapElement);

            if (!HasChildren)
                return mapElements;

            mapElements.AddRange(x0y0.QueryRange(range));
            mapElements.AddRange(x0y1.QueryRange(range));
            mapElements.AddRange(x1y0.QueryRange(range));
            mapElements.AddRange(x1y1.QueryRange(range));

            return mapElements;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            return ToStringCore(sb, 0).ToString();
        }

        StringBuilder ToStringCore(StringBuilder sb, int level)
        {
            if (QuadTreeMapElement != null)
            {
                AppendTabs(sb, level);
                sb.AppendFormat("{0}: {1}\n", QuadTreeMapElement.MapElement, QuadTreeMapElement.Coords);
            }
            if (HasChildren)
            {
                AppendTabs(sb, level);
                sb.AppendLine("00");
                x0y0.ToStringCore(sb, level + 1);

                AppendTabs(sb, level);
                sb.AppendLine("01");
                x0y1.ToStringCore(sb, level + 1);

                AppendTabs(sb, level);
                sb.AppendLine("10");
                x1y0.ToStringCore(sb, level + 1);

                AppendTabs(sb, level);
                sb.AppendLine("11");
                x1y1.ToStringCore(sb, level + 1);
            }

            return sb;
        }

        void AppendTabs(StringBuilder sb, int tabCount)
        {
            for (int i = 0; i < tabCount; i++) sb.Append("    ");
        }

        enum RemoveCoreResult
        {
            OutOfBounds,
            Adjusting,
            Done,
            WontFind
        }
    }
}