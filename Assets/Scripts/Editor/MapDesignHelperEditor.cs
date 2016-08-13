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
                int size = Globals.MapSettings.Size; // rozmiar mapy
                int lastCoord = size - 1; // ostatni wiersz/kolumna mapy
                var toMirrorize = GameObject.FindGameObjectsWithTag(Tag.MapDesignElement); // wyszukujemy wszystkie obiekty które chcemy odbijac wzdluz przekatnej
                // i dla kazdego z nich:
                foreach (var mapElement in toMirrorize)
                {
                    var pos = mapElement.transform.position; // pobieramy jego pozycje
                    var sumCoords = pos.x + pos.z; 
                    if (sumCoords > lastCoord || // jezeli suma wspolrzednych jest wieksza niz ostatni wiersz/kolumna, to jestesmy za przekatna
                        sumCoords == lastCoord && pos.x < pos.z) // lub jezeli jest rowna ostatniemu wierszowi kolumnie, ale doli < zeppe, to jestesmy na lewej-gornej czesci przekatnej
                    {
                        // tych obiektow (za przekatna albo w top-left przekatnej) nie chcemy i je wywalamy
                        DestroyImmediate(mapElement);
                    }
                    // ale jednak jesli obiekt jest w lewej dolnej czesci mapy
                    else if (!(sumCoords == lastCoord && pos.x == pos.z)) // uwzgledniamy punkt dokladnie na srodku mapy (istotne tylko gdy mapa ma nieparzysty size) - z nim nie musielibysmy nic robic
                    {
                        var prefab = PrefabUtility.GetPrefabParent(mapElement); // pobieramy prefab z ktorego zostal zrobiony obiekt ktory chcemy skopiowac
                        GameObject copy;
                        if (prefab == null) // jesli obiekt nie ma prefabu
                        {
                            // robimy kopie na bazie obiektu, a nie na bazie prefabu
                            copy = Instantiate(mapElement);
                        }
                        else // ale jesli ma
                        {
                            // to robimy kopie na bazie prefaba - w ten sposob zostaje utrzymane powiazanie nowego obiektu z prefabem
                            copy = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        }
                        copy.name = mapElement.name; // przepisujemy nazwe, by debil nie pisal przy kazdym do nazwy "(Clone)"
                        copy.transform.SetParent(mapElement.transform.parent); // chcemy by byl pod tym samym parentem (glownie NoArmy)
                        copy.transform.position = new Vector3(lastCoord - pos.x, pos.y, lastCoord - pos.z); // i ustawiamy mu pozycje na po przeciwnej stronie planszy
                    }
                }
            }
        }
    }
}