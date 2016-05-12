using System.Collections.Generic;

namespace MechWars.MapElements.Orders.Actions.Args
{
    public class OrderActionArgs
    {
        Dictionary<string, OrderActionArg> args;

        public OrderActionArg this[string name]
        {
            get
            {
                OrderActionArg value;
                bool success = args.TryGetValue(name, out value);
                if (!success)
                    throw new System.Exception(string.Format(
                        "Trying to access non-existing argument \"{0}\"", name));
                return value;
            }
            set { args[name] = value; }
        }

        public OrderActionArgs(params OrderActionArg[] args)
        {
            this.args = new Dictionary<string, OrderActionArg>();
            foreach (var arg in args)
                this.args.Add(arg.Name, arg);
        }

        public bool TryGetArg(string name, out OrderActionArg arg)
        {
            return args.TryGetValue(name, out arg);
        }
    }
}
