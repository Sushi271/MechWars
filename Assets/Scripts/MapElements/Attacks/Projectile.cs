using System;
using MechWars.MapElements.Statistics;
using UnityEngine;
using System.Linq;
using MechWars.Utils;
using MechWars.FogOfWar;

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
        
        bool visibleToSpectator;
        bool VisibleToSpectator
        {
            get { return visibleToSpectator; }
            set
            {
                if (visibleToSpectator == value) return;

                visibleToSpectator = value;

                var renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers) r.enabled = value;
            }
        }

        void Start()
        {
            visibleToSpectator = true;
        }

        void Update()
        {
            UpdateVisibilityToSpectator();

            var dr = Time.deltaTime * Velocity;
            transform.position += dr;

            currentLifetime += Time.deltaTime;
            if (currentLifetime > Lifetime)
                Hit();
        }

        void UpdateVisibilityToSpectator()
        {
            var coords = transform.position.AsHorizontalVector2().Round();
            VisibleToSpectator = Globals.Map.IsInBounds(coords) && Globals.Armies
                .Where(a => a.actionsVisible)
                .Any(a => a.VisibilityTable[coords.X, coords.Y] == Visibility.Visible);
        }

        void Hit()
        {
            if (hit) return;
            hit = true;

            if (!Target.Dying)
            {
                if (Target.IsGhost && Target.OriginalMapElement != null)
                    Target = Target.OriginalMapElement;
                else Target = null;

                if (Target != null)
                {
                    var hitPoints = Target.Stats[StatNames.HitPoints];
                    if (hitPoints != null)
                        hitPoints.Value -= Firepower;
                }
            }

            Destroy(gameObject);
        }
    }
}