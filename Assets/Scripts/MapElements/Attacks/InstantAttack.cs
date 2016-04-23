using MechWars.MapElements.Statistics;

namespace MechWars.MapElements.Attacks
{
    public class InstantAttack : Attack
    {
        public override void ExecuteStep()
        {
            // TODO: play attack animation or sth

            var firepower = AttackingMapElement.Stats[StatNames.Firepower];
            var hitPoint = Target.Stats[StatNames.HitPoints];

            if (hitPoint != null && firepower != null)
                hitPoint.Value -= firepower.Value;

            Finished = true;
        }
    }
}
