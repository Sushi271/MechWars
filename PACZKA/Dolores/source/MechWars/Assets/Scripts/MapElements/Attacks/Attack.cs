using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class Attack : MonoBehaviour
    {
        public float animationDuration;
        public float animationExecuteTime;

        public virtual Vector2 GetDirection(MapElement attacker, MapElement target, Vector3 aim)
        {
            return aim.AsHorizontalVector2() - attacker.Coords;
        }

        public virtual void Execute(MapElement attacker, MapElement target, Vector3 aim)
        {
        }

        public virtual bool PitchAdjustable { get { return false; } }

        public virtual float GetHeadPitch(MapElement mapElement, MapElement target, Vector3 aim)
        {
            return 0;
        }
    }
}
