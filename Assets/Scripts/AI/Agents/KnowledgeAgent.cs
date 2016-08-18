namespace MechWars.AI.Agents
{
    public class KnowledgeAgent : Agent
    {
        public MapElementKindDictionary MapElementKinds { get; private set; }
        public UnitAgentDictionary UnitAgents { get; private set; }

        public KnowledgeAgent(AIBrain brain, MainAgent parent)
            : base("Knowledge", brain, parent)
        {
            MapElementKinds = new MapElementKindDictionary();
            UnitAgents = new UnitAgentDictionary();
        }

        protected override void OnStart()
        {
            foreach (var u in Army.Units)
                UnitAgents.Add(new UnitAgent(Brain, this, u));
        }

        // TODO:
        // agenci => przerwania jeśli priorytet zadania ważniejszy
    }
}