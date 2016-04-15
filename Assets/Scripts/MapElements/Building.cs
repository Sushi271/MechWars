using MechWars;

namespace MechWars.MapElements
{
    public class Building : MapElement
    {
        public Army army;
        
        public Stats Stats { get; private set; }

        public Building()
        {
            selectable = true;
            Stats = new Stats();
        }
    }
}
