using MechWars.MapElements.Statistics;

namespace MechWars.MapElements.Attacks
{
    public class InstantAttack : Attack
    {
        public override void ExecuteStep()
        {
            // TODO: play attack animation or sth

            var firepower = Attacker.Stats[StatNames.Firepower];
            var hitPoints = Target.Stats[StatNames.HitPoints];

            if (hitPoints != null && firepower != null)
                hitPoints.Value -= firepower.Value;

            Finished = true;
        }
    }
}
