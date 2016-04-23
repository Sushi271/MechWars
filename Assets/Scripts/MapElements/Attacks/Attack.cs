using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class Attack : MonoBehaviour
    {
        public MapElement AttackingMapElement { get; private set; }
        public MapElement Target { get; private set; }

        public bool Finished { get; protected set; }

        public void Initialize(MapElement attackingMapElement, MapElement target)
        {
            Finished = false;
            AttackingMapElement = attackingMapElement;
            Target = target;
        }

        public virtual void ExecuteStep()
        {
            Finished = true;
        }
    }
}
