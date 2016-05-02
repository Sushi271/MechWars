using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class Attack : MonoBehaviour
    {
        public MapElement Attacker { get; private set; }
        public MapElement Target { get; private set; }

        public bool Finished { get; protected set; }

        public void Initialize(MapElement attacker, MapElement target)
        {
            Finished = false;
            Attacker = attacker;
            Target = target;
        }

        public virtual void ExecuteStep()
        {
            Finished = true;
        }
    }
}
