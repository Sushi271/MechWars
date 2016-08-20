using MechWars.FogOfWar;
using MechWars.MapElements;
using MechWars.Utils;

namespace MechWars.AI.Agents
{
    public class KnowledgeAgent : Agent
    {
        public MapElementKindDictionary MapElementKinds { get; private set; }
        public UnitAgentDictionary UnitAgents { get; private set; }
        public ResourcesKnowledge Resources { get; private set; }

        public KnowledgeAgent(AIBrain brain, MainAgent parent)
            : base("Knowledge", brain, parent)
        {
        }

        protected override void OnStart()
        {
            MapElementKinds = new MapElementKindDictionary();
            UnitAgents = new UnitAgentDictionary();
            Resources = new ResourcesKnowledge(this);

            foreach (var u in Army.Units)
                UnitAgents.Add(new UnitAgent(Brain, this, u));
            
            Army.VisibilityTable.VisibilityChanged += VisibilityTable_VisibilityChanged;
        }

        private void VisibilityTable_VisibilityChanged(IVector2 tile, Visibility from, Visibility to)
        {
            if (to == Visibility.Visible)
            {
                var mapElement = MapProxy[tile];
                if (from == Visibility.Unknown)
                {
                    if (mapElement is Resource)
                        Resources[tile] = new ResourceInfo(MapProxy, tile);
                }
                else if (from == Visibility.Fogged)
                {
                    var resInfo = Resources[tile];

                    if (mapElement == null)
                    {
                        if (resInfo != null)
                            Resources[tile] = null;
                    }
                    else if (mapElement is Resource)
                    {
                        if (resInfo == null)
                            Resources[tile] = new ResourceInfo(MapProxy, tile);
                    }
                }
            }
        }

        // TODO:
        // agenci => przerwania jeśli priorytet zadania ważniejszy
    }
}