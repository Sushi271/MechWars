using MechWars.Human;
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

            globals.isGameplay = EditorGUILayout.Toggle(
                "Is Gameplay?", globals.isGameplay);
            globals.humanPlayer = (HumanPlayer)EditorGUILayout.ObjectField(
                "Human Player", globals.humanPlayer, typeof(HumanPlayer), true);
            globals.dayAndNightCycleTime = EditorGUILayout.FloatField(
                "Day & night cycle time (minutes)", globals.dayAndNightCycleTime);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Map parameters:");
            globals.mapWidth = EditorGUILayout.IntField("Width", globals.mapWidth);
            globals.mapHeight = EditorGUILayout.IntField("Height", globals.mapHeight);
            EditorGUILayout.Separator();

            DictionaryGuiLayout(
                globals.sortedPlayers, ref newPlayer,
                globals.sortedArmies, ref newArmy);

            EditorUtility.SetDirty(globals);
        }

        Player newPlayer;
        Army newArmy;

        void DictionaryGuiLayout<TKey, TValue>(
            List<TKey> keys, ref TKey newKey, 
            List<TValue> values, ref TValue newValue)
            where TKey : Object
            where TValue : Object
        {
            var keysCopy = new List<TKey>(keys);
            var valuesCopy = new List<TValue>(values);
            var controlWidth = GUILayout.Width((Screen.width - 27) / 3.0f);

            EditorGUILayout.LabelField("Player-Army assignment:");
            for (int i = 0; i < keysCopy.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var key = (TKey)EditorGUILayout.ObjectField("", keysCopy[i], typeof(TKey), true, controlWidth);
                var value = (TValue)EditorGUILayout.ObjectField("", valuesCopy[i], typeof(TValue), true, controlWidth);
                bool remove = GUILayout.Button("Remove", controlWidth);
                EditorGUILayout.EndHorizontal();

                if (key != keys[i])
                {
                    if (key == null)
                    {
                        EditorUtility.DisplayDialog("Key is NULL!",
                            string.Format("Given key cannot be NULL."), "OK");
                        continue;
                    }
                    if (keys.Contains(key))
                    {
                        EditorUtility.DisplayDialog("Key already exists!",
                            string.Format("Key {0} already exists on this dictionary.", key), "OK");
                        continue;
                    }
                    keys[i] = key;
                }
                if (value != values[i])
                    values[i] = value;
                if (remove)
                {
                    keys.RemoveAt(i);
                    values.RemoveAt(i);
                }
            }

            EditorGUILayout.BeginHorizontal();
            newKey = (TKey)EditorGUILayout.ObjectField("", newKey, typeof(TKey), true, controlWidth);
            newValue = (TValue)EditorGUILayout.ObjectField("", newValue, typeof(TValue), true, controlWidth);
            bool add = GUILayout.Button("Add", controlWidth);
            EditorGUILayout.EndHorizontal();

            if (add)
            {
                if (newKey == null)
                    EditorUtility.DisplayDialog("Key is NULL!",
                        string.Format("Given key cannot be NULL.", newKey), "OK");
                else if (keys.Contains(newKey))
                    EditorUtility.DisplayDialog("Key already exists!",
                        string.Format("Key {0} already exists on this dictionary.", newKey), "OK");
                else
                {
                    keys.Add(newKey);
                    values.Add(newValue);
                }

                newKey = null;
                newValue = null;
            }

            if (GUILayout.Button("Clear"))
            {
                keys.Clear();
                values.Clear();
            }
        }
    }
}