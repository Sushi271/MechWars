using System.Collections.Generic;

namespace MechWars.AI.Agents
{
    public class ReconAgent : Agent
    {
        List<ReconRequest> requests;

        public ReconAgent(AIBrain brain, MainAgent parent)
            : base("DataAcquirer", brain, parent)
        {
            requests = new List<ReconRequest>();
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
                if (message.Name == MessageName.FindMeResources)
                    requests.Add(new ReconRequest(message.Sender, message.Name, int.Parse(message.Arguments[0])));
            }
            requests.Sort((r1, r2) => r1.Priority.CompareTo(r2.Priority));
        }

        void ProcessRequests()
        {

        }
    }
}