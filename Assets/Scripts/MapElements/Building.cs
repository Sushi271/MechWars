using MechWars;

namespace MechWars.MapElements
{
    public class Building : MapElement
    {
        public override bool Interactible { get { return true; } }

        public Building()
        {
            selectable = true;
        }
    }
}
