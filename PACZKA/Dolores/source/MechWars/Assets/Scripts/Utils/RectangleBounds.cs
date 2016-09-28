namespace MechWars.Utils
{
    public struct RectangleBounds : IRectangleBounds
    {
        public IVector2 Location { get; private set; }
        public IVector2 Size { get; private set; }

        public int X0 { get; private set; }
        public int X1 { get; private set; }
        public int Y0 { get; private set; }
        public int Y1 { get; private set; }

        public RectangleBounds(IVector2 location, IVector2 size)
            : this()
        {
            Location = location;
            Size = size;

            X0 = Location.X;
            X1 = Location.X + Size.X - 1;
            Y0 = Location.Y;
            Y1 = Location.Y + Size.Y - 1;
        }

        public bool ContainsPoint(IVector2 point)
        {
            return
                X0 <= point.X && point.X <= X1 &&
                Y0 <= point.Y && point.Y <= Y1;
        }

        public bool IntersectsOther(IRectangleBounds bounds)
        {
            return
                bounds.X0 <= X1 && X0 <= bounds.X1 &&
                bounds.Y0 <= Y1 && Y0 <= bounds.Y1;
        }
    }
}
