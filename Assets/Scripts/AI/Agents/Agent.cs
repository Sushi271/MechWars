using MechWars.AI.Agents.Goals;
using MechWars.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MechWars.AI.Agents
{
    public abstract class Agent
    {
        Queue<Message> messages;
        protected List<Goal> Goals { get; private set; }

        public string Name { get; private set; }
        public AIBrain Brain { get; private set; }
        public Agent Parent { get; private set; }

        public FilteringMapProxy MapProxy { get { return Brain.MapProxy; } }

        public MainAgent MainAgent { get { return Brain.MainAgent; } }
        public ReconAgent Recon { get { return MainAgent.Recon; } }
        public KnowledgeAgent Knowledge { get { return MainAgent.Knowledge; } }

        public bool Started { get; private set; }
        public bool Finished { get; private set; }

        public Player Player { get { return Brain.player; } }
        public Army Army { get { return Player == null ? null : Player.army; } }

        public Agent(string name, AIBrain brain, Agent parent)
        {
            Name = name;
            Brain = brain;
            Parent = parent;

            messages = new Queue<Message>();
            Goals = new List<Goal>();

            brain.AddAgent(this);
        }

        public void Start()
        {
            CheckForArmy();
            OnStart();
            Started = true;
        }

        protected virtual void OnStart()
        {
        }

        public void Update()
        {
            CheckForArmy();
            OnUpdate();
            if (!Goals.Empty())
            {
                var goal = Goals.First();
                if (goal.State == GoalState.BrandNew)
                    goal.Start();
                if (goal.State == GoalState.Started)
                    goal.Update();
                if (goal.InFinalState)
                    Goals.RemoveFirst();
            }
        }

        protected virtual void OnUpdate()
        {
        }

        void CheckForArmy()
        {
            if (Army == null)
                throw new System.Exception(
                    "Agent.Army cannot return NULL (a Player with an Army must be assigned to AIBrain).");
        }

        protected void Finish()
        {
            Finished = true;
        }

        protected void SendMessage(Agent receiver, string messageName, params string[] args)
        {
            var message = new Message(this, receiver, messageName, args);
            receiver.messages.Enqueue(message);
            LogMessage(message);
        }

        void LogMessage(Message message)
        {
            var fileName = Globals.Instance.aiMessageLogFileName;
            if (fileName == null)
                return;

            var fs = new FileStream(fileName, FileMode.Append);
            var sw = new StreamWriter(fs);
            sw.WriteLine(message);
            sw.Close();
        }

        protected Message ReceiveMessage()
        {
            if (messages.Count == 0) return null;
            return messages.Dequeue();
        }

        public override string ToString()
        {
            return string.Format("Agent \"{0}\"", Name);
        }
    }
}