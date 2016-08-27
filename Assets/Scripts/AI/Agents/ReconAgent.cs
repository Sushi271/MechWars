using System.Collections.Generic;

namespace MechWars.AI.Agents
{
    public class ReconAgent : Agent
    {
        List<Request> requests;

        public ReconAgent(AIBrain brain, MainAgent parent)
            : base("Recon", brain, parent)
        {
            requests = new List<Request>();
        }

        protected override void OnUpdate()
        {
            ProcessMessages();
            ProcessRequests();
        }

        void ProcessMessages()
        {
            Message message;
            while ((message = ReceiveMessage()) != null)
            {
                if (message.Name == AIName.FindMeResources)
                {
                    SendMessage(message.Sender, AIName.Ok, message);
                    requests.Add(new Request(message.Sender, message.Name, int.Parse(message.Arguments[0]), message));
                }
            }
            requests.Sort((r1, r2) => r1.Priority.CompareTo(r2.Priority));
        }

        void ProcessRequests()
        {

        }
    }
}