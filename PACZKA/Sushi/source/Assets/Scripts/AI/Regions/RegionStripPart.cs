namespace MechWars.AI.Regions
{
    public class RegionStripPart
    {
        int start;
        public int Start
        {
            get { return start; }
            set
            {
                start = value;
                Normalize();
            }
        }

        int end;
        public int End
        {
            get { return end; }
            set
            {
                end = value;
                Normalize();
            }
        }

        public RegionStripPart(int start, int end)
        {
            this.start = start;
            this.end = end;

            Normalize();
        }

        public bool IsInside(int y)
        {
            return Start <= y && y <= End;
        }

        void Normalize()
        {
            if (start > end)
            {
                var a = start;
                start = end;
                end = a;
            }
        }
    }
}
