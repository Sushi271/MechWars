using MechWars.Effects;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class InstantAttack : Attack
    {
        public override void Execute(MapElement attacker, MapElement target, Vector3 aim)
        {
            if (target.Dying) return;
            if (target.IsGhost)
            {
                if (target.OriginalMapElement == null) return;
                target = target.OriginalMapElement;
            }

            var firepower = attacker.Stats[StatNames.Firepower];
            var hitPoints = target.Stats[StatNames.HitPoints];
            
            if (hitPoints != null && firepower != null)
                hitPoints.Value -= firepower.Value;

            SpawnEffect(attacker, target, aim);
        }

        void SpawnEffect(MapElement attacker, MapElement target, Vector3 aim)
        {
            var origin = attacker.transform.position;
            var dir = aim - origin;
            var ray = new Ray(origin, dir);
            var dist = dir.magnitude;

            var colliders = target.GetComponentsInChildren<Collider>();
            var hits =
                from c in colliders
                let h = RaycastCollider(c, ray, dist)
                where h != null
                select h;
            var hit = hits.HasAtLeast(1) ?
                hits.SelectMin(rh => (rh.Value.point - origin).sqrMagnitude) :
                null;
            var point = hit != null ? hit.Value.point : aim;

            var explosion = Instantiate(Globals.Prefabs.normalHitEffect);
            explosion.transform.position = point;
        }

        RaycastHit? RaycastCollider(Collider collider, Ray ray, float maxDistance)
        {
            RaycastHit rh;
            if (collider.Raycast(ray, out rh, maxDistance))
                return rh;
            return null;
        }
    }
}
