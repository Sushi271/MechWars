using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class Projectile : MonoBehaviour
    {
        bool hit;
        float currentLifetime;
        
        public MapElement Target { get; set; }
        public Vector3 Velocity { get; set; }
        public float Firepower { get; set; }
        public float Lifetime { get; set; }

        void Update()
        {
            var dr = Time.deltaTime * Velocity;
            var oldPos = transform.position;
            transform.position += dr;

            currentLifetime += Time.deltaTime;
            if (currentLifetime > Lifetime)
                Hit();
        }

        void Hit()
        {
            if (hit) return;
            hit = true;

            if (Target.Alive)
            {
                var hitPoints = Target.Stats[StatNames.HitPoints];
                if (hitPoints != null)
                    hitPoints.Value -= Firepower;
            }

            Destroy(gameObject);
        }
    }
}