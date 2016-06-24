using UnityEngine;

namespace MechWars.MapElements
{
    public class AttackHead : MonoBehaviour, IRotatable
    {
        public GameObject tip;

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

        public Vector3 TipPosition { get { return tip.transform.position; } }
    }
}
