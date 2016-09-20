using System.IO;
using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    [CustomEditor(typeof(ResearchHelper))]
    public class ResearchHelperEditor : Editor
    {
        bool paused = true;
        string timestampText = "";

        bool gameRunning = false;

        public override void OnInspectorGUI()
        {
            var researchHelper = (ResearchHelper)target;

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
            float pauseTimeScale = paused ? 1 : 0;

            if (GUILayout.Button(pauseButtonText))
            {
                paused = !paused;
                Time.timeScale = pauseTimeScale;
            }

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
        }
    }
}   