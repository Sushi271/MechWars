using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI.Agents
{
    public class ReconAgent : Agent
    {
        List<Request> requests;

        public HashSet<UnitAgent> ReconUnits { get; private set; }

        public ReconAgent(AIBrain brain, MainAgent parent)
            : base("Recon", brain, parent)
        {
            requests = new List<Request>();
            ReconUnits = new HashSet<UnitAgent>();
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

        bool waitingForScouts;
        void ProcessRequests()
        {
            var processed = new List<Request>();
            foreach (var r in requests)
            {
                if (r.Name == AIName.FindMeResources)
                {
                    var kind =
                        (from k in Knowledge.UnitAgents.Kinds
                        let p = k.GetPurposeValue(AIName.Harvesting)
                        where p > 0
                        orderby p descending
                        select k).FirstOrDefault();

                    if (kind == null && !waitingForScouts)
                    {
                        SendMessage(Production, AIName.ProduceMeUnits, "0", AIName.Scout);
                        waitingForScouts = true;
                        continue;
                    }
                    else if (waitingForScouts)
                        continue;

                    processed.Add(r);
                }
            }
            foreach (var r in processed)
                requests.Remove(r);
        }
    }
}