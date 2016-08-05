using MechWars.MapElements;
using MechWars.MapElements.Statistics;
using System.Text;

namespace MechWars.FogOfWar
{
    public class VisibilityTable
    {
        public Army Army { get; private set; }
        public int Size { get; private set; }

        bool[,] fieldsUncovered;
        int[,] fieldsSeenByUnits;

        public Visibility this[int x, int y]
        {
            get
            {
                return
                    !fieldsUncovered[x, y] ? Visibility.Unknown :
                    fieldsSeenByUnits[x, y] == 0 ? Visibility.Fogged :
                    Visibility.Visible;
            }
        }

        public VisibilityTable(Army army)
        {
            Army = army;
            Size = Globals.MapSettings.Size;

            fieldsUncovered = new bool[Size, Size];
            fieldsSeenByUnits = new int[Size, Size];
        }

        void IncreaseVisibility(int x, int y)
        {
            if (x < 0 || Size <= x ||
                y < 0 || Size <= y) return;

            fieldsUncovered[x, y] = true;
            fieldsSeenByUnits[x, y]++;
        }

        void IncreaseVisibility(float x, float y, MapElementSurroundingShape shape)
        {
            for (int rx = (int)(x + shape.DeltaXNeg), i = 0; rx <= (int)(x + shape.DeltaXPos); rx++, i++)
                for (int ry = (int)(y + shape.DeltaYNeg), j = 0; ry <= (int)(y + shape.DeltaYPos); ry++, j++)
                    if (shape[i, j]) IncreaseVisibility(rx, ry);
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

        void DecreaseVisibility(int x, int y)
        {
            if (x < 0 || Size <= x ||
                y < 0 || Size <= y) return;

            fieldsSeenByUnits[x, y]--;
        }

        void DecreaseVisibility(float x, float y, MapElementSurroundingShape shape)
        {
            for (int rx = (int)(x + shape.DeltaXNeg), i = 0; rx <= (int)(x + shape.DeltaXPos); rx++, i++)
                for (int ry = (int)(y + shape.DeltaYNeg), j = 0; ry <= (int)(y + shape.DeltaYPos); ry++, j++)
                    if (shape[i, j]) DecreaseVisibility(rx, ry);
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
