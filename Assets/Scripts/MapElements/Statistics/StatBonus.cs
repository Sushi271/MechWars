using UnityEngine;

namespace MechWars.MapElements
{
    public class StatBonus : MonoBehaviour
    {
        public MapElement receiver;
        public string statName;
        public BonusType type;
        public float value;
        public int order;

        public float ApplyTo(float value)
        {
            if (type == BonusType.Multiply)
                return this.value * value;
            else if (type == BonusType.Add)
                return this.value + value;
            else return this.value;
        }
    }
}