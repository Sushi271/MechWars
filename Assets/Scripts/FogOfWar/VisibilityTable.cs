using MechWars.MapElements;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Text;

namespace MechWars.FogOfWar
{
    public class VisibilityTable
    {
        public Army Army { get; private set; }
        public int Size { get; private set; }

        bool[,] fieldsUncovered;
        int[,] fieldsSeenByUnits;

        public event VisibilityChangedEventHandler VisibilityChanged;

        public Visibility this[int x, int y]
        {
            get
            {
                try
                {
                    return
                        !fieldsUncovered[x, y] ? Visibility.Unknown :
                        fieldsSeenByUnits[x, y] == 0 ? Visibility.Fogged :
                        Visibility.Visible;
                }
                catch
                {
                    throw new System.Exception(string.Format("OutOfRange: [{0}, {1}]", x, y));
                }
            }
        }

        public VisibilityTable(Army army)
        {
            Army = army;
            Size = Globals.MapSettings.Size;

            fieldsUncovered = new bool[Size, Size];
            fieldsSeenByUnits = new int[Size, Size];
        }

        void IncreaseVisibilityOfTile(int x, int y)
        {
            if (x < 0 || Size <= x ||
                y < 0 || Size <= y) return;

            bool justUncovered = !fieldsUncovered[x, y];
            fieldsUncovered[x, y] = true;
            fieldsSeenByUnits[x, y]++;

            if (VisibilityChanged != null)
                if (justUncovered) VisibilityChanged(new IVector2(x, y), Visibility.Unknown, Visibility.Visible);
                else VisibilityChanged(new IVector2(x, y), Visibility.Fogged, Visibility.Visible);
        }

        void IncreaseVisibility(float x, float y, MapElementSurroundingShape shape)
        {
            for (int rx = shape.GetXMin(x), i = 0; rx <= shape.GetXMax(x); rx++, i++)
                for (int ry = shape.GetYMin(y), j = 0; ry <= shape.GetYMax(y); ry++, j++)
                    if (shape[i, j]) IncreaseVisibilityOfTile(rx, ry);
        }

        public void IncreaseVisibility(MapElement mapElement)
        {
            var coords = mapElement.Coords;
            IncreaseVisibility(mapElement, coords.x, coords.y);
        }

        public void IncreaseVisibility(MapElement mapElement, float x, float y)
        {
            var meShape = mapElement.Shape;
            var radiusStat = mapElement.Stats[StatNames.ViewRange];
            if (radiusStat == null) return;
            var radius = radiusStat.Value;

            var losShape = Globals.LOSShapeDatabase[radius, meShape];
            IncreaseVisibility(x, y, losShape);
        }

        void DecreaseVisibilityOfTile(int x, int y)
        {
            if (x < 0 || Size <= x ||
                y < 0 || Size <= y) return;

            fieldsSeenByUnits[x, y]--;

            if (VisibilityChanged != null && fieldsSeenByUnits[x, y] == 0)
                VisibilityChanged(new IVector2(x, y), Visibility.Visible, Visibility.Fogged);
        }

        void DecreaseVisibility(float x, float y, MapElementSurroundingShape shape)
        {
            for (int rx = shape.GetXMin(x), i = 0; rx <= shape.GetXMax(x); rx++, i++)
                for (int ry = shape.GetYMin(y), j = 0; ry <= shape.GetYMax(y); ry++, j++)
                    if (shape[i, j]) DecreaseVisibilityOfTile(rx, ry);
        }

        public void DecreaseVisibility(MapElement mapElement)
        {
            var coords = mapElement.Coords;
            DecreaseVisibility(mapElement, coords.x, coords.y);
        }

        public void DecreaseVisibility(MapElement mapElement, float x, float y)
        {
            var meShape = mapElement.Shape;
            var radiusStat = mapElement.Stats[StatNames.ViewRange];
            if (radiusStat == null) return;
            var radius = radiusStat.Value;

            var losShape = Globals.LOSShapeDatabase[radius, meShape];
            DecreaseVisibility(x, y, losShape);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int y = Size - 1; y >= 0; y--)
            {
                for (int x = 0; x < Size; x++)
                    sb.Append(
                        this[x, y] == Visibility.Unknown ? 'X' :
                        this[x, y] == Visibility.Fogged ? 'o' : '.');
                if (y > 0)
                    sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
