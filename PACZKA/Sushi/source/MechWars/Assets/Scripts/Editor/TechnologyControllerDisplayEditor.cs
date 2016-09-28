using MechWars.MapElements;
using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    [CustomEditor(typeof(TechnologyControllerDisplay))]
    public class TechnologyControllerDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var statsDisplay = (TechnologyControllerDisplay)target;
            var army = statsDisplay.GetComponent<Army>();
            var technologies = army.Technologies;

            if (technologies == null) return;

            GUI.enabled = false;
            EditorGUILayout.LabelField("Technologies:");
            foreach (var t in technologies.DevelopedTechnologies)
                EditorGUILayout.LabelField(" - " + t.technologyName);
            EditorGUILayout.LabelField("In progress:");
            foreach (var t in technologies.DevelopingTechnologies)
                EditorGUILayout.LabelField(" - " + t.technologyName);
            GUI.enabled = true;

            EditorUtility.SetDirty(target);
        }
    }
}