using MechWars.FogOfWar;
using MechWars.Utils;
using System.Linq;
using UnityEngine;

namespace MechWars.Effects
{
    public class NormalHit : MonoBehaviour
    {
        public float fadeTime = 0.5f;

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
            var coords = transform.position.AsHorizontalVector2().Round();
            VisibleToSpectator = Globals.Armies
                .Where(a => a.actionsVisible)
                .Any(a => !Globals.Map.IsInBounds(coords.X, coords.Y) ||
                     a.VisibilityTable[coords.X, coords.Y] == Visibility.Visible);
        }
    }
}