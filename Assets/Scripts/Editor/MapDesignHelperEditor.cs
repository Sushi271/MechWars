using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    [CustomEditor(typeof(MapDesignHelper))]
    public class MapDesignHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Mirrorize map"))
            {
                int size = Globals.MapSettings.Size;
                int lastCoord = size - 1;
                var toMirrorize = GameObject.FindGameObjectsWithTag(Tag.MapDesignElement);
                foreach (var mapElement in toMirrorize)
                {
                    var pos = mapElement.transform.position;
                    var sumCoords = pos.x + pos.z;
                    if (sumCoords > lastCoord ||
                        sumCoords == lastCoord && pos.x < pos.z)
                    {
                        DestroyImmediate(mapElement);
                    }
                    else if (!(sumCoords == lastCoord && pos.x == pos.z)) // istotne tylko gdy mapa ma nieparzysty size
                    {
                        var prefab = PrefabUtility.GetPrefabParent(mapElement);
                        GameObject copy;
                        if (prefab == null)
                        {
                            copy = Instantiate(mapElement);
                        }
                        else
                        {
                            copy = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        }
                        copy.name = mapElement.name;
                        copy.transform.SetParent(mapElement.transform.parent);
                        copy.transform.position = new Vector3(lastCoord - pos.x, pos.y, lastCoord - pos.z);
                    }
                }
            }
        }
    }
}