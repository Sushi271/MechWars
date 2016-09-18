using MechWars.AI.Agents.Goals;
using MechWars.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MechWars.AI.Agents
{
    public abstract class Agent
    {
        Queue<Message> messages;
        protected List<Goal> Goals { get; private set; }
        Dictionary<System.Action, ActionToPerform> actionsToPerform;
        Dictionary<System.Action<object>, ArgActionToPerform> argActionsToPerform;

        public string Name { get; private set; }
        public AIBrain Brain { get; private set; }
        public Agent Parent { get; private set; }

        public FilteringMapProxy MapProxy { get { return Brain.MapProxy; } }

        public MainAgent MainAgent { get { return Brain.MainAgent; } }
        public KnowledgeAgent Knowledge { get { return MainAgent.Knowledge; } }
        public ReconAgent Recon { get { return MainAgent.Recon; } }
        public ConstructionAgent Construction { get { return MainAgent.Construction; } }
        public ProductionAgent Production { get { return MainAgent.Production; } }

        public bool Started { get; private set; }
        public bool Finished { get; private set; }

        public Player Player { get { return Brain.player; } }
        public Army Army { get { return Player == null ? null : Player.army; } }

        public Goal CurrentGoal { get { return Goals.Count == 0 ? null : Goals[0]; } }
        public float CurrentGoalImportance { get { return CurrentGoal == null ? 0 : CurrentGoal.Importance; } }

        public Agent(string name, AIBrain brain, Agent parent)
        {
            Name = name;
            Brain = brain;
            Parent = parent;

            messages = new Queue<Message>();
            Goals = new List<Goal>();
            actionsToPerform = new Dictionary<System.Action, ActionToPerform>();
            argActionsToPerform = new Dictionary<System.Action<object>, ArgActionToPerform>();

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
            ResetActionsIncrements();
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

        void ResetActionsIncrements()
        {
            foreach (var a in actionsToPerform.Values)
                a.alreadyIncremented = false;
            foreach (var a in argActionsToPerform.Values)
                a.alreadyIncremented = false;
        }

        protected void Finish()
        {
            Finished = true;
        }

        public void GiveGoal(Goal goal, float importance)
        {
            Goals.Add(goal);
            goal.Importance = importance;
        }

        protected void SendMessage(Agent receiver, string messageName, params string[] args)
        {
            var message = new Message(this, receiver, messageName, args);
            receiver.messages.Enqueue(message);
            LogMessage(message);
        }

        protected void SendMessage(Agent receiver, string messageName, Message innerMessage, params string[] args)
        {
            var message = new Message(this, receiver, messageName, innerMessage, args);
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

        protected void PerformEvery(float seconds, System.Action func)
        {
            ActionToPerform a2p;
            bool success = actionsToPerform.TryGetValue(func, out a2p);
            if (!success)
            {
                a2p = new ActionToPerform { timeInterval = seconds, func = func };
                actionsToPerform[func] = a2p;
                func();
            }
            if (a2p.alreadyIncremented) return;

            bool skipFirst = !success;

            a2p.timer += Time.deltaTime;
            a2p.alreadyIncremented = true;
            while (a2p.timer >= a2p.timeInterval)
            {
                if (skipFirst) skipFirst = false;
                else func();
                a2p.timer -= a2p.timeInterval;
            }
        }

        protected void PerformEvery(float seconds, System.Action<object> func, object arg)
        {
            ArgActionToPerform a2p;
            bool success = argActionsToPerform.TryGetValue(func, out a2p);
            if (!success)
            {
                a2p = new ArgActionToPerform { timeInterval = seconds, func = func };
                argActionsToPerform[func] = a2p;
                func(arg);
            }
            if (a2p.alreadyIncremented) return;

            bool skipFirst = !success;

            a2p.timer += Time.deltaTime;
            a2p.alreadyIncremented = true;
            while (a2p.timer >= a2p.timeInterval)
            {
                if (skipFirst) skipFirst = false;
                else func(arg);
                a2p.timer -= a2p.timeInterval;
            }
        }

        protected void StopPerform(System.Action func)
        {
            actionsToPerform.Remove(func);
        }

        protected void StopPerform(System.Action<object> func)
        {
            argActionsToPerform.Remove(func);
        }

        public override string ToString()
        {
            return string.Format("Agent \"{0}\"", Name);
        }

        class ActionToPerform
        {
            public float timeInterval;
            public float timer;
            public System.Action func;
            public bool alreadyIncremented;
        }

        class ArgActionToPerform
        {
            public float timeInterval;
            public float timer;
            public System.Action<object> func;
            public bool alreadyIncremented;
        }
    }
}