using MechWars.Utils;
using System.Linq;
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
            var size = Globals.MapSettings.Size;
            var pixels = texture.GetPixels();

            var visibleArmies = Globals.Armies.Where(a => a.actionsVisible);
            var noArmies = visibleArmies.Empty();

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    var alpha = 1f;
                    if (!noArmies)
                    {
                        var visibility = visibleArmies
                            .Select(a => a.VisibilityTable[x, y])
                            .Aggregate((v1, v2) =>
                                v1 == Visibility.Visible || v2 == Visibility.Visible ? Visibility.Visible :
                                v1 == Visibility.Fogged || v2 == Visibility.Fogged ? Visibility.Fogged :
                                Visibility.Unknown);
                        alpha =
                           visibility == Visibility.Visible ? 0f :
                           visibility == Visibility.Fogged ? 0.5f : 1f;
                    }
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
