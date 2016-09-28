using UnityEngine;

namespace MechWars.MapElements
{
    public class HorizontalAttackHead : AttackHead, IRotatable
    {
        public float Rotation
        {
            get { return transform.rotation.eulerAngles.y; }
            set
            {
                var ea = transform.rotation.eulerAngles;
                ea.y = value;
                transform.rotation = Quaternion.Euler(ea);
            }
        }

        public float HeadPitch
        {
            get { return 0; }
            set { throw new System.InvalidOperationException("Cannot set HeadPitch of HorizontalAttackHead."); }
        }
    }
}
