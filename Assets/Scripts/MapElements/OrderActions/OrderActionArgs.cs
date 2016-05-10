using System.Collections.Generic;

namespace MechWars.MapElements.OrderActions
{
    public class OrderActionArgs
    {
        Dictionary<string, object> args;

        public object this[string name]
        {
            get
            {
                object value;
                bool success = args.TryGetValue(name, out value);
                if (!success)
                    throw new System.Exception(string.Format(
                        "Trying to access non-existing argument \"{0}\"", name));
                return value;
            }
            set { args[name] = value; }
        }

        public OrderActionArgs()
        {
            args = new Dictionary<string, object>();
        }

        public bool TryGetArg(string name, out object arg)
        {
            return args.TryGetValue(name, out arg);
        }
    }
}
