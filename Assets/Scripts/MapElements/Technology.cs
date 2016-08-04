using MechWars.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Technology : MonoBehaviour
    {
        public string technologyName;
        public List<StatBonus> bonuses;
        public List<ScriptAction> onTechnologyDevelopingActions;
        public List<ScriptAction> onTechnologyDevelopedActions;
        
        public override string ToString()
        {
            return technologyName ?? "";
        }

        public bool IsTheSameAs(Technology technology)
        {
            return technologyName == technology.technologyName;
        }

        public void OnTechnologyDeveloping(Army developingArmy)
        {
            var args = new OnTechnologyDevelopArgs(this, developingArmy);
            foreach (var a in onTechnologyDevelopingActions)
                a.Invoke(args);
        }

        public void OnTechnologyDeveloped(Army developingArmy)
        {
            var args = new OnTechnologyDevelopArgs(this, developingArmy);
            foreach (var a in onTechnologyDevelopedActions)
                a.Invoke(args);
        }
    }
}