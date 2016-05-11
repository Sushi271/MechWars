namespace MechWars.MapElements.WallNeighbourhoods
{
    public struct WallNeighbourhood
    {
        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Right { get; private set; }
        public bool Left { get; private set; }

        public WallNeighbourhood(bool up, bool down, bool right, bool left)
            : this()
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;
        }

        public override int GetHashCode()
        {
            // wyciagamy bity z booli i potem posumujemy, to da nam liczbe od 0000 do 1111 (czyli 0 - 15)
            int up = Up ? 1 : 0;       // 0001
            int down = Down ? 2 : 0;   // 0010
            int right = Right ? 4 : 0; // 0100
            int left = Left ? 8 : 0;   // 1000

            return (up + down + left + right).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WallNeighbourhood))
                return false;
            var n = (WallNeighbourhood)obj;
            return Up == n.Up && Down == n.Down && Left == n.Left && Right == n.Right;
        }
    }
}
