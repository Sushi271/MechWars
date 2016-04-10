using MechWars.Utils;

namespace Assets.Scripts.MapElements
{
    public class MapElementShape
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public IVector2 Size { get { return new IVector2(Width, Height); } }

        bool[,] shape;
        public bool this[int x, int y]
        {
            get { return shape[x, y]; }
        }
        public bool this[IVector2 coords]
        {
            get { return this[coords.X, coords.Y]; }
        }

        public MapElementShape(int width, int height)
        {
            Width = width;
            Height = height;

            shape = new bool[Width, Height]; 
        }
    }
}
