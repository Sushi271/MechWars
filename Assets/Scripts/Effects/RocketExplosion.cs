using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class RocketExplosion : MonoBehaviour
    {
        public float fadeTime = 1.5f;

        float alpha = 1;
        float currentFadeTime = 0;

        void Update()
        {
            var dAlpha = Time.deltaTime / fadeTime;
            alpha -= dAlpha;
            var mat = GetComponent<Renderer>().material;
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);

            currentFadeTime += Time.deltaTime;
            if (currentFadeTime >= fadeTime)
                Destroy(gameObject);
        }
    }
}