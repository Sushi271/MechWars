using MechWars.Utils;
using System.Linq;

namespace MechWars.MapElements
{
    public class DecreaseVisibilityTableScriptAction : ScriptAction
    {
        public override void Invoke(object arg = null)
        {
            var args = arg as OnTechnologyDevelopArgs;
            if (args == null) return;

            foreach (var b in args.Technology.bonuses)
            {
                var receivers = args.Army.Units.Where(unt => unt.mapElementName == b.receiver.mapElementName).Cast<MapElement>();
                if (receivers.Empty())
                    receivers = args.Army.Buildings.Where(bld => bld.mapElementName == b.receiver.mapElementName).Cast<MapElement>();
                if (receivers.Empty())
                    continue;

                foreach (var r in receivers)
                    args.Army.VisibilityTable.DecreaseVisibility(r);
            }
        }
    }
}