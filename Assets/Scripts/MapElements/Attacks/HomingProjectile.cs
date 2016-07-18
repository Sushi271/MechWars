using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class HomingProjectile : MonoBehaviour
    {
        public float damageRadius;
        public GameObject explosionPrefab;

        bool hit;
        
        public Vector3 Aim { get; set; }
        public Vector3 Velocity { get; set; }
        public float Firepower { get; set; }

        Vector3 direction;
        Vector2 forward2;
        float currentVertAngle;
        float currentHorAngle;

        float armTime = 0.5f;

        void Start()
        {
            UpdateAngles();
        }

        void UpdateAngles()
        {
            direction = Velocity.normalized;
            var rgt = Vector3.Cross(Vector3.up, direction);
            var fwd = Vector3.Cross(rgt, Vector3.up);
            var sign = Mathf.Sign(Vector3.Dot(Vector3.Cross(fwd, direction), rgt));
            currentVertAngle = Vector3.Angle(fwd, direction) * sign;
            currentHorAngle = UnityExtensions.AngleFromToXZ(Vector2.up, fwd.AsHorizontalVector2());

            forward2 = fwd.AsHorizontalVector2();
        }

        void Update()
        {
            float horAnglePerSec = 270;
            float vertAnglePerSec = 120;

            var toAim = Aim - transform.position;
            var horToAim = toAim.AsHorizontalVector2();
            var horAngleToAim = UnityExtensions.AngleFromToXZ(forward2, horToAim);
            var horDAngle = Time.deltaTime * horAnglePerSec * Mathf.Sign(horAngleToAim);
            if (Mathf.Abs(horDAngle) > Mathf.Abs(horAngleToAim))
                horDAngle = horAngleToAim;
            currentHorAngle += horDAngle;

            var vertDirToForwardAngle = -currentVertAngle;
            var horToAim3 = horToAim.AsHorizontalVector3();
            var rgt = Vector3.Cross(Vector3.up, toAim);
            var sign = Mathf.Sign(Vector3.Dot(Vector3.Cross(horToAim3, toAim), rgt));
            var vertToAimAngle = Vector3.Angle(horToAim3, toAim) * sign;
            var vertAngleToAim = vertDirToForwardAngle + vertToAimAngle;
            var vertDAngle = Time.deltaTime * vertAnglePerSec * Mathf.Sign(vertAngleToAim);
            if (Mathf.Abs(vertDAngle) > Mathf.Abs(vertAngleToAim))
                vertDAngle = vertAngleToAim;
            if (vertDAngle > 0)
                currentVertAngle += vertDAngle;
            
            Velocity = Velocity.magnitude * (Quaternion.Euler(currentVertAngle, currentHorAngle, 0) * Vector3.forward);
            transform.rotation = Quaternion.LookRotation(Velocity);

            var dr = Time.deltaTime * Velocity;
            transform.position += dr;

            UpdateAngles();

            if (transform.position.y <= 0)
                Hit();

            if (armTime > 0)
            {
                armTime -= Time.deltaTime;
                if (armTime < 0) armTime = 0;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (armTime == 0)
                Hit();
        }

        void Hit()
        {
            if (hit) return;
            hit = true;
            
            var coords = transform.position.AsHorizontalVector2();
            var xFrom = Mathf.RoundToInt(coords.x - damageRadius);
            var xTo = Mathf.RoundToInt(coords.x + damageRadius);
            var yFrom = Mathf.RoundToInt(coords.y - damageRadius);
            var yTo = Mathf.RoundToInt(coords.y + damageRadius);

            var damageCandidates = new List<MapElement>();
            for (int x = xFrom; x <= xTo; x++)
                for (int y = yFrom; y <= yTo; y++)
                {
                    var map = Globals.FieldReservationMap;
                    if (!map.CoordsInside(x, y))
                        continue;
                    var candidate = map[x, y];
                    if (candidate != null)
                        damageCandidates.Add(candidate);
                }
            damageCandidates = damageCandidates.Distinct().ToList();

            foreach (var dc in damageCandidates)
            {
                if (dc.Dying) continue;

                var hitPoints = dc.Stats[StatNames.HitPoints];
                if (hitPoints == null) continue;

                var pos = dc.GetClosestAimTo(coords).AsHorizontalVector2();
                var dist = Vector2.Distance(pos, coords);
                if (dist >= damageRadius)
                    continue;

                float damage = Firepower;
                float r = dist / damageRadius;
                if (r > 0.2f)
                    damage *= -1.25f * r + 1.25f;

                hitPoints.Value -= damage;
            }

            var explosion = Instantiate(explosionPrefab);
            explosion.transform.SetParent(transform.parent);
            explosion.transform.position = coords.AsHorizontalVector3();
            explosion.transform.localScale =
                new Vector3(1, 0, 1) * damageRadius * 2 +
                new Vector3(0, 1, 0) * explosion.transform.localScale.y;

            Destroy(gameObject);
        }
    }
}