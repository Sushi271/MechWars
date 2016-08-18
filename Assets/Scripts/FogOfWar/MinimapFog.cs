using UnityEngine;

namespace MechWars.FogOfWar
{
    public class MinimapFog : MonoBehaviour
    {
        public VisualFog visualFog;

        void Update()
        {
            GetComponent<Renderer>().material.mainTexture =
                visualFog.GetComponent<Renderer>().material.mainTexture;
        }
    }
}      