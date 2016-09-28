using UnityEngine;

namespace MechWars.MapElements
{
    public class VerticalAttackHead : AttackHead
    {
        public float Pitch
        {
            get { return transform.rotation.eulerAngles.x; }
            set
            {
                var ea = transform.rotation.eulerAngles;
                ea.x = value;
                transform.rotation = Quaternion.Euler(ea);
            }
        }
    }
}
