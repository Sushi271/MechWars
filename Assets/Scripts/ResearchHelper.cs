using MechWars.AI;
using MechWars.FogOfWar;
using System.IO;
using UnityEngine;

namespace MechWars
{
    public class ResearchHelper : MonoBehaviour
    {
        public string timestampsFilename = "";
        public string resourcesFilename = "";
        public string reconFilename = "";

        public AIBrain AI;

        int lastTime = -1;

        void Start()
        {
            if (resourcesFilename != null &&
                resourcesFilename != "")
            {
                FileStream fs = new FileStream(resourcesFilename, FileMode.Create);
                fs.Close();
            }
            if (reconFilename != null &&
                reconFilename != "")
            {
                FileStream fs = new FileStream(reconFilename, FileMode.Create);
                fs.Close();
            }
        }

        void Update()
        {
            var time = (int)Time.time;
            if (time > lastTime)
            {
                lastTime = time;
                MakeMeasurements(time);
            }
        }

        void MakeMeasurements(int time)
        {
            if (AI == null) return;

            if (resourcesFilename != null &&
                resourcesFilename != "")
            {
                FileStream fs = new FileStream(resourcesFilename, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(string.Format("{0} {1}", time, AI.MainAgent.Army.resources));

                sw.Close();
            }

            if (reconFilename != null &&
                reconFilename != "")
            {
                FileStream fs = new FileStream(reconFilename, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);

                var visibilityTable = AI.MainAgent.Army.VisibilityTable;
                var exploredTiles = CalcExploredTiles(visibilityTable);
                var explorationPercentage = exploredTiles / (float)(visibilityTable.Size * visibilityTable.Size);
                sw.WriteLine(string.Format("{0} {1} {2}", time, exploredTiles, explorationPercentage));

                sw.Close();
            }
        }

        int CalcExploredTiles(VisibilityTable visibilityTable)
        {
            int sum = 0;
            var size = visibilityTable.Size;
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    if (visibilityTable[x, y] != Visibility.Unknown)
                        sum++;
            return sum;
        }
    }
}
