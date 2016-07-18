using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class InstantAttack : Attack
    {
        public override void Execute(MapElement attacker, MapElement target, Vector3 aim)
        {
            var firepower = attacker.Stats[StatNames.Firepower];
            var hitPoints = target.Stats[StatNames.HitPoints];
            
            if (hitPoints != null && firepower != null)
                hitPoints.Value -= firepower.Value;
        }
    }
}
