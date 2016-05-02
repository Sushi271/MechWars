using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class Projectile : MonoBehaviour
    {
        public float gravity;
        public float lifetime;

        bool hit;
        float currentLifetime;
        
        public Army Army { get; set; }
        public Vector3 Velocity { get; set; }
        public float Firepower { get; set; }

        void Update()
        {
            var dv = Vector3.up * Time.deltaTime * gravity;
            Velocity += dv;
            var dr = Time.deltaTime * Velocity;
            transform.position += dr;

            currentLifetime += Time.deltaTime;
            if (currentLifetime > lifetime)
                Destroy(gameObject);
        }

        void OnTriggerEnter(Collider collider)
        {
            if (hit) return;
            hit = true;

            var mapElement = collider.gameObject.GetComponentInParent<MapElement>();
            if (mapElement != null && mapElement.army != Army)
            {
                var hitPoints = mapElement.Stats[StatNames.HitPoints];
                Debug.Log(Firepower);
                if (hitPoints != null)
                    hitPoints.Value -= Firepower;
            }

            Destroy(gameObject);
        }
    }
}