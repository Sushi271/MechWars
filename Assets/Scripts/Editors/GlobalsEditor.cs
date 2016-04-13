using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    [CustomEditor(typeof(Globals))]
    public class GlobalsEditor : Editor
    {
        Globals globals;

        public override void OnInspectorGUI()
        {
            globals = (Globals)target;

            globals.groundMaterial = (Material)EditorGUILayout.ObjectField(
                "Ground Material", globals.groundMaterial, typeof(Material), false);
            globals.fieldCollider = (GameObject)EditorGUILayout.ObjectField(
                "Field Collider", globals.fieldCollider, typeof(GameObject), false);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Map parameters:");
            globals.mapWidth = EditorGUILayout.IntField("Width", globals.mapWidth);
            globals.mapHeight = EditorGUILayout.IntField("Height", globals.mapHeight);
            EditorGUILayout.Separator();

            DictionaryGuiLayout();
        }

        GameObject newPlayer;
        GameObject newArmy;

        void DictionaryGuiLayout()
        {
            bool changed = true;
                
            var sp = globals.sortedPlayers;
            var sa = globals.sortedArmies;

            var spCopy = new List<GameObject>(sp);
            var saCopy = new List<GameObject>(sa);
            var controlWidth = GUILayout.Width((Screen.width - 27) / 3.0f);

            EditorGUILayout.LabelField("Player-Army assignment:");
            for (int i = 0; i < spCopy.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var key = (GameObject)EditorGUILayout.ObjectField("", spCopy[i], typeof(GameObject), true, controlWidth);
                var value = (GameObject)EditorGUILayout.ObjectField("", saCopy[i], typeof(GameObject), true, controlWidth);
                bool remove = GUILayout.Button("Remove", controlWidth);
                EditorGUILayout.EndHorizontal();

                if (key != sp[i])
                {
                    if (key == null)
                    {
                        EditorUtility.DisplayDialog("Key is NULL!",
                            string.Format("Given key cannot be NULL.", key), "OK");
                        continue;
                    }
                    if (sp.Contains(key))
                    {
                        EditorUtility.DisplayDialog("Key already exists!",
                            string.Format("Key {0} already exists on this dictionary.", key), "OK");
                        continue;
                    }
                    sp[i] = key;
                    changed = true;
                }
                if (value != sa[i])
                {
                    sa[i] = value;
                    changed = true;
                }
                if (remove)
                {
                    sp.RemoveAt(i);
                    sa.RemoveAt(i);
                    changed = true;
                }
            }

            EditorGUILayout.BeginHorizontal();
            newPlayer = (GameObject)EditorGUILayout.ObjectField("", newPlayer, typeof(GameObject), true, controlWidth);
            newArmy = (GameObject)EditorGUILayout.ObjectField("", newArmy, typeof(GameObject), true, controlWidth);
            bool add = GUILayout.Button("Add", controlWidth);
            EditorGUILayout.EndHorizontal();

            if (add)
            {
                if (newPlayer == null)
                    EditorUtility.DisplayDialog("Key is NULL!",
                        string.Format("Given key cannot be NULL.", newPlayer), "OK");
                else if (sp.Contains(newPlayer))
                    EditorUtility.DisplayDialog("Key already exists!",
                        string.Format("Key {0} already exists on this dictionary.", newPlayer), "OK");
                else
                {
                    sp.Add(newPlayer);
                    sa.Add(newArmy);
                    changed = true;
                }
                
                newPlayer = null;
                newArmy = null;
            }

            if (GUILayout.Button("Clear"))
            {
                globals.sortedPlayers.Clear();
                globals.sortedArmies.Clear();
                changed = true;
            }

            if (changed)
                EditorUtility.SetDirty(globals);
        }
    }
}