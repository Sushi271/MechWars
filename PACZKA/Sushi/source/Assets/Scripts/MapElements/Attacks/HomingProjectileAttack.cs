using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class HomingProjectileAttack : Attack
    {
        public HomingProjectile projectilePrefab;
        public float startingSpeed;
        public float accuracy;
        
        public override bool PitchAdjustable { get { return true; } }

        public override float GetHeadPitch(MapElement mapElement, MapElement target, Vector3 aim)
        {
            float longDistancePitch = 30;

            float shortDistance = 3;
            float shortDistancePitch = 70;

            float distance = (mapElement.Coords - aim.AsHorizontalVector2()).magnitude;
            float pitch;
            if (distance <= shortDistance)
                pitch = -shortDistancePitch;
            else
            {
                float expBase = 1.8f;
                float xOffset = Mathf.Log(shortDistancePitch - longDistancePitch, expBase) + shortDistance;

                pitch = -(Mathf.Pow(1.8f, (-distance + xOffset)) + longDistancePitch);
            }
            return pitch;
        }

        public override void Execute(MapElement attacker, MapElement target, Vector3 aim)
        {
            if (projectilePrefab == null)
                throw new System.Exception(string.Format("HomingProjectile prefab is null ({0}).", attacker));

            var firepower = attacker.Stats[StatNames.Firepower];
            if (firepower == null)
                throw new System.Exception(string.Format("MapElement {0} has no {1} Stat.", attacker, StatNames.Firepower));
            
            IEnumerable<Vector3> startingPositions;
            Quaternion startingRotation;
            if (attacker.attackHead != null)
            {
                startingPositions = attacker.attackHead.TipsPositions;
                startingRotation = attacker.attackHead.transform.rotation;
            }
            else
            {
                startingPositions = attacker.transform.position.AsEnumerable();
                startingRotation = attacker.transform.rotation;
            }
            int count = startingPositions.Count();

            foreach (var p in startingPositions)
            {
                var projectile = Instantiate(projectilePrefab);
                projectile.transform.SetParent(attacker.Army.transform);
                projectile.transform.position = p;
                projectile.transform.rotation = startingRotation;
                projectile.Firepower = firepower.Value / count;
                projectile.Aim = aim;
                projectile.Velocity = (startingRotation * Vector3.forward).normalized * startingSpeed;
            }
        }
    }
}
