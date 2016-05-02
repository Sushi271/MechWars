using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class ProjectileAttack : Attack
    {
        public Projectile projectilePrefab;
        public float startingSpeed;
        public Vector3 startingPosition;

        public override void ExecuteStep()
        {
            if (projectilePrefab == null)
                throw new System.Exception(string.Format("Projectile prefab is null ({0}).", Attacker));

            var firepower = Attacker.Stats[StatNames.Firepower];
            if (firepower == null)
                throw new System.Exception(string.Format("MapElement {0} has no {1} Stat.", Attacker, StatNames.Firepower));
            
            var projectile = Instantiate(projectilePrefab);
            projectile.transform.SetParent(Attacker.army.transform);
            projectile.transform.position = transform.rotation * startingPosition + transform.position;
            projectile.transform.rotation = transform.rotation;
            projectile.Army = Attacker.army;
            projectile.Velocity = CalculateStartingVelocity(projectile);
            projectile.Firepower = firepower.Value;

            Finished = true;
        }

        Vector3 CalculateStartingVelocity(Projectile projectile)
        {
            var startPos = projectile.transform.position;
            var targetPos = Target.Coords.AsHorizontalVector3() + Target.yToAim * Vector3.up;
            
            var startPos2 = startPos.AsHorizontalVector2();
            var targetPos2 = targetPos.AsHorizontalVector2();

            var h0 = startPos.y;
            var h1 = targetPos.y;
            var dist = Vector2.Distance(startPos2, targetPos2);
            var g = projectile.gravity;
            var r = startingSpeed;

            var A = (g * dist * dist) / (2 * r * r);
            var B = dist;
            var C = h0 - h1 + A;
            var delta = B * B - 4 * A * C;
            if (delta < 0)
                throw new System.Exception(string.Format("Target {0} out of attack range for MapElement {1}.", Target, Attacker));
            float tan;
            if (delta == 0)
                tan = -B / (2 * A);
            else
            {
                var tan1 = (-B - Mathf.Sqrt(delta)) / (2 * A);
                var tan2 = (-B + Mathf.Sqrt(delta)) / (2 * A);
                tan = Math2.AbsMin(tan1, tan2);
            }
            var x = Mathf.Sqrt((r * r) / (1 + tan * tan));
            var y = x * tan;

            var horizontal = (x * (targetPos2 - startPos2).normalized).AsHorizontalVector3();
            var vertical = y * Vector3.up;
            var startingVelocity = horizontal + vertical;

            return startingVelocity;
        }
    }
}
