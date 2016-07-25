using UnityEngine;

namespace MechWars.FogOfWar
{
    public class VisualFog : MonoBehaviour
    {
        Texture2D texture;

        void Start()
        {
            var size = Globals.MapSettings.Size;
            texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;

            GetComponent<Renderer>().material.mainTexture = texture;
        }

        void Update()
        {
            PaintVisiblity();
            AlignWithCamera();
        }

        void PaintVisiblity()
        {
            var army = Globals.HumanArmy;
            if (army == null) return;

            var size = Globals.MapSettings.Size;
            var pixels = texture.GetPixels();

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    var visibility = army.VisibilityTable[x, y];
                    var alpha =
                        visibility == Visibility.Visible ? 0 :
                        visibility == Visibility.Fogged ? 0.5f : 1;
                    Color color = new Color(0, 0, 0, alpha);
                    pixels[y * size + x] = color;
                }

            texture.SetPixels(pixels);
            texture.Apply(false);
        }

        void AlignWithCamera()
        {
            var size = Globals.MapSettings.Size;
            var camPos = Globals.MainCamera.transform.position;
            var terrainCorner = new Vector3(-0.5f, 0, -0.5f);
            var terrainSize = new Vector3(size, 0, size);
            var terrainCenter = terrainCorner + terrainSize / 2;
            var fogY = transform.position.y;

            var terrainCenterToCamPos = camPos - terrainCenter;
            var dir = terrainCenterToCamPos.normalized;
            var t = (fogY - terrainCenter.y) / dir.y;
            var newFogPos = new Vector3(
                terrainCenter.x + dir.x * t, fogY,
                terrainCenter.z + dir.z * t);

            var newFogPosToCamPos = camPos - newFogPos;
            var ratio = newFogPosToCamPos.magnitude / terrainCenterToCamPos.magnitude;
            var newFogScale = ratio * size;

            transform.position = newFogPos;
            transform.localScale = Vector3.one * newFogScale;
        }
    }
}
