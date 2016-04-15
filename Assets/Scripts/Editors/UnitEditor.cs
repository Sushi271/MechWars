using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapElements;
using MechWars.MapElements;
using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    //[CustomEditor(typeof(Unit))]
    public class UnitEditor : Editor
    {
        Unit unit;

        SerializedObject serObj;
        SerializedProperty armyProp;

        void OnEnable()
        {
            serObj = new SerializedObject(target);
            armyProp = serObj.FindProperty("army");
        }

        public override void OnInspectorGUI()
        {
            unit = (Unit)target;

            unit.mapElementName = EditorGUILayout.TextField("Map Element name", unit.mapElementName);
            GUI.enabled = false;
            unit.id = EditorGUILayout.IntField("Id", unit.id);
            GUI.enabled = true;
            unit.selectable = EditorGUILayout.Toggle("Selectable", unit.selectable);
            EditorGUILayout.PropertyField(armyProp, new GUIContent("Army"));
            serObj.ApplyModifiedProperties();

            EditorGUILayout.Separator();

            DictionaryGuiLayout();

            EditorUtility.SetDirty(target);
        }

        string newName = string.Empty;
        Dictionary<string, FoldoutData> foldouts = new Dictionary<string, FoldoutData>();
        class FoldoutData
        {
            public bool Expanded { get; set; }
            public string NewName { get; set; }
            public float Value { get; set; }
            public bool Limited { get; set; }
            public float MaxValue { get; set; }
        }

        void DictionaryGuiLayout()
        {
            var stats = unit.Stats;

            var statsCopy = new Stats(stats);

            EditorGUILayout.LabelField("Statistics:");
            foreach (var sa in statsCopy)
            {
                if (!foldouts.ContainsKey(sa.Key))
                {
                    foldouts[sa.Key] = new FoldoutData
                    {
                        Expanded = true,
                        NewName = sa.Key,
                        Value = sa.Value.Value,
                        Limited = sa.Value.Limited,
                        MaxValue = sa.Value.MaxValue
                    };
                }
                var foldoutData = foldouts[sa.Key];

                EditorGUILayout.BeginHorizontal();
                foldoutData.Expanded = EditorGUILayout.Foldout(foldoutData.Expanded, string.Format("{0} [{1}]", sa.Key, foldoutData.Value));
                bool remove = GUILayout.Button("Remove");
                EditorGUILayout.EndHorizontal();

                if (foldoutData.Expanded)
                {
                    EditorGUILayout.BeginHorizontal();
                    foldoutData.NewName = EditorGUILayout.TextField("Name", foldoutData.NewName);
                    bool change = GUILayout.Button("Change");
                    EditorGUILayout.EndHorizontal();

                    foldoutData.Value = EditorGUILayout.FloatField("Value", foldoutData.Value);
                    if (!foldoutData.Limited) foldoutData.MaxValue = foldoutData.Value;
                    foldoutData.Limited = EditorGUILayout.Toggle("Limited", foldoutData.Limited);
                    if (!foldoutData.Limited) GUI.enabled = false;
                    foldoutData.MaxValue = EditorGUILayout.FloatField("Value", foldoutData.MaxValue);
                    if (!foldoutData.Limited) GUI.enabled = true;
                    
                    sa.Value.MaxValue = foldoutData.MaxValue;
                    sa.Value.Limited = foldoutData.Limited;
                    sa.Value.Value = foldoutData.Value;

                    foldoutData.Value = sa.Value.Value;
                    foldoutData.MaxValue = sa.Value.MaxValue;

                    if (remove)
                    {
                        stats.Remove(sa.Key);
                        foldouts.Remove(sa.Key);
                    }
                    else if (change && foldoutData.NewName != sa.Key && CheckNameValid(foldoutData.NewName, stats))
                    {
                        var attribute = sa.Value;
                        attribute.Name = foldoutData.NewName;
                        stats.Remove(sa.Key);
                        stats.Add(attribute);
                        
                        foldouts.Remove(sa.Key);
                        foldouts.Add(foldoutData.NewName, foldoutData);
                    }
                }
            }

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            newName = EditorGUILayout.TextField("Name", newName);
            bool add = GUILayout.Button("Add");
            EditorGUILayout.EndHorizontal();

            if (add && CheckNameValid(newName, stats))
            {
                var attr = new Attribute(newName);
                stats.Add(attr);
                foldouts[newName] = new FoldoutData
                {
                    Expanded = true,
                    NewName = newName,
                    Value = attr.Value,
                    Limited = attr.Limited,
                    MaxValue = attr.MaxValue
                };
                newName = string.Empty;
            }

            if (GUILayout.Button("Clear"))
            {
                stats.Clear();
                foldouts.Clear();
            }

            if (GUILayout.Button("TEST"))
            {
                EditorUtility.DisplayDialog("TEST RESULT",
                    unit.Stats.Count == 0 ? "EMPTY" :
                    unit.Stats.Select(sa => sa.Key.ToString() + " --- " + sa.Value.ToString())
                    .Aggregate((s1, s2) => s1 + "\n" + s2), "OK");
            }
        }

        bool CheckNameValid(string name, Stats stats)
        {
            if (name == string.Empty)
                EditorUtility.DisplayDialog("Name is an empty string!",
                    string.Format("Given name cannot be empty string."), "OK");
            else if (stats.ContainsKey(name))
                EditorUtility.DisplayDialog("Name already exists!",
                    string.Format("Name {0} already exists on this dictionary.", name), "OK");
            else return true;
            return false;
        }
    }
}