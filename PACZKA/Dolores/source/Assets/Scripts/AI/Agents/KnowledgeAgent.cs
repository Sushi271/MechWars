using MechWars.FogOfWar;
using MechWars.MapElements;
using MechWars.Utils;

namespace MechWars.AI.Agents
{
    public class KnowledgeAgent : Agent
    {
        public MapElementKindDictionary MapElementKinds { get; private set; }
        public TechnologyKindDictionary TechnologyKinds { get; private set; }
        public CreationMethodDictionary CreationMethods { get; private set; }

        public UnitAgentDictionary UnitAgents { get; private set; }

        public ResourcesKnowledge Resources { get; private set; }
        public AllyBaseKnowledge AllyBase { get; private set; }

        public KnowledgeAgent(AIBrain brain, MainAgent parent)
            : base("Knowledge", brain, parent)
        {
        }

        protected override void OnStart()
        {
            MapElementKinds = new MapElementKindDictionary(Brain);
            TechnologyKinds = new TechnologyKindDictionary(Brain);
            CreationMethods = new CreationMethodDictionary(Brain);

            UnitAgents = new UnitAgentDictionary(this);

            Resources = new ResourcesKnowledge(this);
            AllyBase = new AllyBaseKnowledge(this);

            foreach (var u in Army.Units)
                UnitAgents.Add(new UnitAgent(Brain, this, u));
            
            Army.VisibilityTable.VisibilityChanged += VisibilityTable_VisibilityChanged;
            Army.OnVisibleMapElementCreated += Army_OnVisibleMapElementCreated;
            Army.OnVisibleMapElementDied += Army_OnVisibleMapElementDied;
        }

        void VisibilityTable_VisibilityChanged(IVector2 tile, Visibility from, Visibility to)
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

        void Army_OnVisibleMapElementCreated(MapElement mapElement)
        {
            var tile = mapElement.Coords.Round();
            if (mapElement is Resource)
                Resources[tile] = new ResourceInfo(MapProxy, tile);
            else if (mapElement is Building)
            {
                var b = (Building)mapElement;
                if (b.Army == Army)
                    AllyBase.AddBuilding(new BuildingInfo(MapProxy, b));
            }
            else if (mapElement is Unit)
            {
                var u = (Unit)mapElement;
                if (u.Army == Army)
                    UnitAgents.Add(new UnitAgent(Brain, this, u));
            }
        }

        void Army_OnVisibleMapElementDied(MapElement mapElement)
        {
            var tile = mapElement.Coords.Round();
            if (mapElement is Resource)
                Resources[tile] = null;
            else if (mapElement is Building)
            {
                var b = (Building)mapElement;
                if (b.Army == Army)
                    AllyBase.RemoveBuilding(AllyBase[tile]);
            }
            else if (mapElement is Unit)
            {
                var u = (Unit)mapElement;
                if (u.Army == Army)
                    UnitAgents.Remove(u);
            }
        }
    }
}