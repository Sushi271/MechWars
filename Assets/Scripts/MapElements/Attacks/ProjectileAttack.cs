using MechWars.MapElements.Jobs;
using MechWars.MapElements.Orders;
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
        
        public override Vector2 GetDirection(MapElement attacker, MapElement target, Vector2 aim)
        {
            var data = CalculateStartingVelocity2(attacker.transform.position, target, aim);
            return data.startingVelocity.AsHorizontalVector2();
        }
        
        public override void Execute(MapElement attacker, MapElement target, Vector2 aim)
        {
            if (projectilePrefab == null)
                throw new System.Exception(string.Format("Projectile prefab is null ({0}).", attacker));

            var firepower = attacker.Stats[StatNames.Firepower];
            if (firepower == null)
                throw new System.Exception(string.Format("MapElement {0} has no {1} Stat.", attacker, StatNames.Firepower));

            // it gives different result than in OnInitialize (because RotateJob is Done)
            var worldStartingPosition = attacker.transform.rotation * startingPosition + attacker.transform.position;
            var data = CalculateStartingVelocity2(worldStartingPosition, target, aim);

            var projectile = Instantiate(projectilePrefab);
            projectile.transform.SetParent(attacker.army.transform);
            projectile.transform.position = worldStartingPosition;
            projectile.transform.rotation = attacker.transform.rotation;
            projectile.Velocity = data.startingVelocity;
            projectile.Lifetime = data.timeToHit;
            projectile.Firepower = firepower.Value;
            projectile.Target = target;
        }

        StartingData CalculateStartingVelocity2(Vector3 worldStartingPosition, MapElement target, Vector2 aim)
        {
            var A = worldStartingPosition;
            var B = aim.AsHorizontalVector3() + Vector3.up * target.yToAim;
            var AB = B - A;
            var ABLen = AB.magnitude;

            var data = new StartingData();
            data.startingVelocity = startingSpeed * AB.normalized;
            data.timeToHit = ABLen / startingSpeed;

            var targetUnit = target as Unit;
            if (targetUnit != null)
            {
                var move = targetUnit.Move;
                if (move != null)
                {
                    var ABLenSq = ABLen * ABLen;

                    var VALen = startingSpeed;
                    var VALenSq = VALen * VALen;

                    var VB = move.Velocity.AsHorizontalVector3();
                    var VBLen = VB.magnitude;
                    var VBLenSq = VBLen * VBLen;

                    var VBDotAB = Vector3.Dot(VB, AB);

                    var alpha = ABLenSq;
                    var beta = 2 * VBDotAB;
                    var gamma = VBLenSq - VALenSq;
                    var Delta = beta * beta - 4 * alpha * gamma;

                    if (Delta >= 0)
                    {
                        var sqrtDelta = Mathf.Sqrt(Delta);
                        var reciprocT1 = (-beta - sqrtDelta) / (2 * alpha);
                        var reciprocT2 = (-beta + sqrtDelta) / (2 * alpha);
                        var reciprocT = reciprocT1 >= 0 ? reciprocT1 : reciprocT2;
                        var t = 1 / reciprocT;

                        data.startingVelocity = AB / t + VB;
                        data.timeToHit = t;
                    }
                }
            }
                

            return data;
        }
        
        struct StartingData
        {
            public Vector3 startingVelocity;
            public float timeToHit;
        }
    }
}
