using MechWars.AI;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    [CustomEditor(typeof(ResearchHelper))]
    public class ResearchHelperEditor : Editor
    {
        bool paused = false;
        string timestampText = "";

        bool gameRunning = false;

        public override void OnInspectorGUI()
        {
            var researchHelper = (ResearchHelper)target;

            GUILayout.Label("AI Brain:");
            researchHelper.AI = (AIBrain)EditorGUILayout.ObjectField(researchHelper.AI, typeof(AIBrain), true);

            GUILayout.Label("Resources filename:");
            researchHelper.resourcesFilename = GUILayout.TextField(researchHelper.resourcesFilename);
            GUILayout.Label("Recon filename:");
            researchHelper.reconFilename = GUILayout.TextField(researchHelper.reconFilename);

            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();

            bool gameWasRunning = gameRunning;
            gameRunning = Application.isPlaying;
            if (!gameWasRunning && gameRunning && 
                researchHelper.timestampsFilename != null &&
                researchHelper.timestampsFilename != string.Empty)
            {
                var fs = new FileStream(researchHelper.timestampsFilename, FileMode.OpenOrCreate);
                var sw = new StreamWriter(fs);
                sw.WriteLine("---------------------------------------------------------------------\n");
                fs.Close();
            }

            GUI.enabled = !Application.isPlaying;

            GUILayout.Label("Timestamps filename:");
            researchHelper.timestampsFilename = GUILayout.TextField(researchHelper.timestampsFilename);

            GUI.enabled = Application.isPlaying;

            string pauseButtonText = paused ? "Unpause" : "Pause";
            if (GUILayout.Button(pauseButtonText))
            {
                paused = !paused;
            }
            float pauseTimeScale = paused ? 0 : 1;
            Time.timeScale = pauseTimeScale;

            GUI.enabled = Application.isPlaying && paused &&
                researchHelper.timestampsFilename != null &&
                researchHelper.timestampsFilename != string.Empty;

            GUILayout.Label("Timestamp text:");
            timestampText = GUILayout.TextField(timestampText);

            if (GUILayout.Button("Make TimeStamp!"))
            {
                var now = Time.time;
                var minutes = (int)(now / 60f);
                var seconds = (int)(now - minutes * 60f);
                var message = string.Format("{0}:{1:D2} - {2}", minutes, seconds, timestampText);

                var fs = new FileStream(researchHelper.timestampsFilename, FileMode.Append);
                var sw = new StreamWriter(fs);
                sw.WriteLine(message);
                sw.Close();

                timestampText = "";
            }

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }
    }
}   