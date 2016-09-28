using MechWars.MapElements;
using MechWars.MapElements.Statistics;
using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    [CustomEditor(typeof(StatsDisplay))]
    public class StatsDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var statsDisplay = (StatsDisplay)target;
            var mapElement = statsDisplay.GetComponent<MapElement>();
            var stats = mapElement.Stats;

            if (stats == null) return;

            GUI.enabled = false;
            foreach (var kv in stats)
            {
                var stat = kv.Value;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(stat.Name);
                EditorGUILayout.TextField(stat.Value.ToString());
                EditorGUILayout.TextField(stat.Limited ? stat.MaxValue.ToString() : "---");
                EditorGUILayout.EndHorizontal();
            }
            GUI.enabled = true;

            EditorUtility.SetDirty(target);
        }
    }
}