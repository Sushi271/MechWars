using MechWars.AI;
using MechWars.FogOfWar;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    [CustomEditor(typeof(AIBrainTest))]
    public class AIBrainTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var bt = (AIBrainTest)target;
            GUILayout.Label("Brain:");
            bt.brain = (AIBrain)EditorGUILayout.ObjectField(bt.brain, typeof(AIBrain), true);
            var brain = bt.brain;

            GUI.enabled = Application.isPlaying && brain != null;

            if (GUILayout.Button("Reveal all"))
            {
                var mi = typeof(VisibilityTable).GetMethod("IncreaseVisibilityOfTile",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                var vtable = brain.MainAgent.Army.VisibilityTable;
                for (int x = 0; x < Globals.MapSettings.Size; x++)
                    for (int y = 0; y < Globals.MapSettings.Size; y++)
                        if (vtable[x, y] == Visibility.Unknown)
                            mi.Invoke(vtable, new object[] { x, y });
            }

            if (GUILayout.Button("Test ResourceRegions"))
            {
                var regions = brain.MainAgent.Knowledge.Resources.Regions;
                int total = 0;
                int totalRes = 0;
                foreach (var r in regions)
                {
                    Debug.LogFormat("Value = {0}\n\n{1}", r.TotalResourceValue, r.Region.ToString());
                    total++;
                    totalRes += r.TotalResourceValue;
                }
                Debug.LogFormat("Total Regions: {0}", total);
                Debug.LogFormat("Total Resources: {0}", totalRes);
            }

            GUI.enabled = true;
        }
    }
}