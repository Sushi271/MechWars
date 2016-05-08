using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class Attack : MonoBehaviour
    {
        public float animationDuration;
        public float animationExecuteTime;

        public virtual Vector2 GetDirection(MapElement attacker, MapElement target, Vector2 aim)
        {
            return aim - attacker.Coords;
        }

        public virtual void Execute(MapElement attacker, MapElement target, Vector2 aim)
        {
        }
    }
}
