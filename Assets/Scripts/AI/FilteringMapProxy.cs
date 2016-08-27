using MechWars.FogOfWar;
using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI
{
    public class FilteringMapProxy
    {
        Army army;

        public FilteringMapProxy(Army army)
        {
            this.army = army;
        }

        public int Size { get; private set; }
        
        public List<IVector2> this[MapElement me]
        {
            get
            {
                if (me.IsGhost) return Globals.Map.GetGhostPositions(me);
                if (!me.VisibleToArmies[army]) return new List<IVector2>();
                return Globals.Map[me];
            }
        }
        
        public MapElement this[int x, int y]
        {
            get
            {
                if (army.VisibilityTable[x, y] == Visibility.Visible)
                    return Globals.Map[x, y];
                if (army.VisibilityTable[x, y] == Visibility.Fogged)
                {
                    var mapElement = Globals.Map[x, y];
                    if (mapElement != null && mapElement.VisibleToArmies[army])
                        return mapElement;
                    return Globals.Map.GetGhosts(x, y).FirstOrDefault(me => me.ObservingArmy == army);
                }
                return null;
            }
        }

        public MapElement this[IVector2 coords]
        {
            get { return this[coords.X, coords.Y]; }
        }
    }
}