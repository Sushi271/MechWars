namespace MechWars.MapElements
{
    public class OnTechnologyDevelopArgs
    {
        public Technology Technology { get; private set; }
        public Army Army { get; private set; }

        public OnTechnologyDevelopArgs(Technology technology, Army army)
        {
            Technology = technology;
            Army = army;
        }
    }
}