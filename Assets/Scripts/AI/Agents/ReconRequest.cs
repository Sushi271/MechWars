namespace MechWars.AI.Agents
{
    public class ReconRequest
    {
        public Agent RequestingAgent { get; private set; }
        public string Name { get; private set; }
        public int Priority { get; private set; }
        
        public ReconRequest(Agent requestingAgent, string name, int priority)
        {
            RequestingAgent = requestingAgent;
            Name = name;
            Priority = priority;
        }
    }
}