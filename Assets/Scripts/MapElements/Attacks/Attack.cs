using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class Attack : MonoBehaviour
    {
        public Unit AttackingUnit { get; private set; }
        public MapElement Target { get; private set; }

        public bool Finished { get; protected set; }

        public void Initialize(Unit attackingUnit, MapElement target)
        {
            Finished = false;
            AttackingUnit = attackingUnit;
            Target = target;
        }

        public virtual void ExecuteStep()
        {
            Finished = true;
        }
    }
}
