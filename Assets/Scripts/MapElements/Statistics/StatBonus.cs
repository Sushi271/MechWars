using UnityEngine;

namespace MechWars.MapElements
{
    public class StatBonus : MonoBehaviour
    {
        public MapElement receiver;
        public string statName;
        public BonusType type;
        public float value;
    }
}