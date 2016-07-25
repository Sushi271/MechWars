using MechWars.Utils;
using MechWars.MapElements;
using UnityEngine;
using System.Linq;
using System.Text;

namespace MechWars.FogOfWar
{
    public class LOSShape
    {
        public float DeltaXNeg { get { return (1 - Width) * 0.5f; } }
        public float DeltaXPos { get { return (Width - 1) * 0.5f; } }
        public float DeltaYNeg { get { return (1 - Height) * 0.5f; } }
        public float DeltaYPos { get { return (Height - 1) * 0.5f; } }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public IVector2 Size { get { return new IVector2(Width, Height); } }

        public float Radius { get; private set; }
        public MapElementShape MapElementShape { get; private set; }

        public int IRadius { get; private set; }

        bool[,] shape;
        public bool this[int x, int y]
        {
            get { return shape[x, y]; }
        }

        public LOSShape(float radius, MapElementShape mapElementShape)
        {
            Radius = radius;
            MapElementShape = mapElementShape;

            IRadius = Mathf.RoundToInt(Radius);

            Width = mapElementShape.Width + 2 * IRadius;
            Height = mapElementShape.Height + 2 * IRadius;
            shape = new bool[Width, Height];

            var templateSize = 1 + 2 * IRadius;
            var sqRadius = Radius * Radius;
            var template = new bool[templateSize, templateSize];

            for (int x = 0; x < templateSize; x++)
                for (int y = 0; y < templateSize; y++)
                {
                    var dx = x - IRadius;
                    var dy = y - IRadius;

                    var corners = new Vector2[]
                        {
                            new Vector2(dx - 0.5f, dy - 0.5f),
                            new Vector2(dx - 0.5f, dy + 0.5f),
                            new Vector2(dx + 0.5f, dy - 0.5f),
                            new Vector2(dx + 0.5f, dy + 0.5f)
                        };
                    template[x, y] = corners.Any(c => c.sqrMagnitude <= sqRadius);
                }

            for (int i = 0; i < mapElementShape.Width; i++)
                for (int j = 0; j < mapElementShape.Height; j++)
                {
                    if (!mapElementShape[i, j]) continue;
                    for (int x = 0; x < templateSize; x++)
                        for (int y = 0; y < templateSize; y++)
                            shape[x + i, y + j] = shape[x + i, y + j] || template[x, y];
                }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                    sb.Append(shape[x, y] ? 'x' : '.');
                if (y < Height - 1)
                    sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
