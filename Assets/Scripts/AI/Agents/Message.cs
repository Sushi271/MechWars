using System.Text;
using UnityEngine;

namespace MechWars.AI.Agents
{
    public class Message
    {
        public Agent Sender { get; private set; }
        public Agent Receiver { get; private set; }
        public string Name { get; private set; }
        public string[] Arguments { get; private set; }

        public Message InnerMessage { get; private set; }

        public Message(Agent sender, Agent receiver, string name, params string[] arguments)
        {
            Sender = sender;
            Receiver = receiver;
            Name = name;
            Arguments = arguments;
        }
        public Message(Agent sender, Agent receiver, string name, Message innerMessage, params string[] arguments)
            : this(sender, receiver, name, arguments)
        {
            InnerMessage = innerMessage;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}: {1} ==> {2}, Name: \"{3}\", Args: ",
                Time.time, Sender, Receiver, Name);
            for (int i = 0; i < Arguments.Length; i++)
            {
                sb.AppendFormat("\"{0}\"", Arguments[i]);
                if (i < Arguments.Length - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }
    }
}