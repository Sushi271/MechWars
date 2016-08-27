using UnityEngine;

namespace MechWars.AI.Agents
{
    public class Request
    {
        public Agent RequestingAgent { get; private set; }
        public string Name { get; private set; }
        public int Priority { get; private set; }
        public Message InnerMessage { get; private set; }
        public Vector2? Position { get; private set; }

        public Request(Agent requestingAgent, string name, int priority, Message innerMessage, Vector2? position = null)
        {
            RequestingAgent = requestingAgent;
            Name = name;
            Priority = priority;
            InnerMessage = innerMessage;
            Position = position;
        }
    }
}