using MechWars.FogOfWar;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.Effects
{
    public class RocketExplosion : MonoBehaviour
    {
        public float fadeTime = 1.5f;
        public float radius;

        float alpha = 1;
        float currentFadeTime = 0;

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

            var dAlpha = Time.deltaTime / fadeTime;
            alpha -= dAlpha;
            var mat = GetComponent<Renderer>().material;
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);

            currentFadeTime += Time.deltaTime;
            if (currentFadeTime >= fadeTime)
                Destroy(gameObject);
        }

        void UpdateVisibilityToSpectator()
        {
            var coords = transform.position.AsHorizontalVector2();
            var x0 = Mathf.Clamp(Mathf.Round(coords.x - radius), 0, Globals.Map.Size);
            var x1 = Mathf.Clamp(Mathf.Round(coords.x + radius), 0, Globals.Map.Size);
            var y0 = Mathf.Clamp(Mathf.Round(coords.y - radius), 0, Globals.Map.Size);
            var y1 = Mathf.Clamp(Mathf.Round(coords.y + radius), 0, Globals.Map.Size);
            var fieldsInCircle = new List<IVector2>();
            for (float x = x0 + 0.5f; x <= x1 - 0.5f; x++)
                for (float y = y0 + 0.5f; y <= y1 - 0.5f; y++)
                {
                    var fromCenter = new Vector2(x, y) - coords;
                    if (fromCenter.sqrMagnitude < radius * radius)
                        for (float i = -0.5f; i <= 0.5f; i++)
                            for (float j = -0.5f; j <= 0.5f; j++)
                                fieldsInCircle.Add(new IVector2((int)(x + i), (int)(y + j)));
                }
            var fieldsInCircleDistinct = fieldsInCircle.Distinct();

            VisibleToSpectator = Globals.Armies
                .Where(a => a.actionsVisible)
                .Any(a => fieldsInCircleDistinct
                    .Any(f => !Globals.Map.IsInBounds(f.X, f.Y) ||
                        a.VisibilityTable[f.X, f.Y] == Visibility.Visible));
        }
    }
}