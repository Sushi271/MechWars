using System.Collections.Generic;
using System.IO;
using MechWars.Utils;

namespace MechWars.MapElements
{
    public class MapElementShape
    {
        static MapElementShape defaultShape;
        public static MapElementShape DefaultShape
        {
            get
            {
                if (defaultShape == null)
                {
                    defaultShape = new MapElementShape();
                    defaultShape.Define(new bool[,] { { true } });
                }
                return defaultShape;
            }
        }

        public float DeltaXNeg { get { return (1 - Width) * 0.5f; } }
        public float DeltaXPos { get { return (Width - 1) * 0.5f; } }
        public float DeltaYNeg { get { return (1 - Height) * 0.5f; } }
        public float DeltaYPos { get { return (Height - 1) * 0.5f; } }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public IVector2 Size { get { return new IVector2(Width, Height); } }
        
        public bool Defined { get; private set; }

        bool[,] shape;
        public bool this[int x, int y]
        {
            get { return shape[x, y]; }
        }
        public bool this[IVector2 coords]
        {
            get { return this[coords.X, coords.Y]; }
        }

        public MapElementShape()
        {
            Defined = false;
        }

        public void Define(bool[,] shapeDefinition)
        {
            if (Defined)
                throw new System.Exception("Cannot define the same MapElementShape twice.");
            
            Defined = true;
            Width = shapeDefinition.GetLength(0);
            Height = shapeDefinition.GetLength(1);
            shape = new bool[Width, Height];

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    shape[i, j] = shapeDefinition[i, j];
        }

        public static MapElementShape FromString(string shapeDefinition, char occupiedChar = 'x')
        {
            var sr = new StringReader(shapeDefinition);
            var lines = new List<string>();
            int width = 1;
            while (true)
            {
                var line = sr.ReadLine();
                if (line == null) break;
                lines.Add(line);
                width = System.Math.Max(width, line.Length);
            }
            sr.Close();

            var shapeDefinitionArray = new bool[width, lines.Count];
            for (int j = 0; j < lines.Count; j++)
                for (int i = 0; i < lines[j].Length; i++)
                    shapeDefinitionArray[i, lines.Count - j - 1] = lines[j][i] == occupiedChar;

            var shape = new MapElementShape();
            shape.Define(shapeDefinitionArray);
            return shape;
        }
    }
}
