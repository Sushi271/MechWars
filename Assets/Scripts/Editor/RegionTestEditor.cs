using MechWars.AI.Regions;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    [CustomEditor(typeof(RegionTest))]
    public class RegionTestEditor : Editor
    {
        int tileX;
        int tileY;

        public override void OnInspectorGUI()
        {
            var rt = (RegionTest)target;
            var region = rt.region;

            tileX = EditorGUILayout.IntField("Tile X", tileX);
            tileY = EditorGUILayout.IntField("Tile Y", tileY);
            
            if (GUILayout.Button("Add Tile"))
                region.AddTile(tileX, tileY);
            if (GUILayout.Button("Remove Tile"))
                region.RemoveTile(tileX, tileY);

            rt.regionReadFile = (TextAsset)EditorGUILayout.ObjectField("Region file", rt.regionReadFile, typeof(TextAsset), false);

            if (GUILayout.Button("Read Region from file") && rt.regionReadFile != null)
                ReadRegionFromFile(rt.regionReadFile, ref rt.region);

            if (GUILayout.Button("Print Region"))
                Debug.Log(region);

            if (GUILayout.Button("Create & Print Hull") && rt.region != null)
            {
                Debug.Log(rt.hull = new RegionHull(rt.region));
            }

            if (GUILayout.Button("Create & Print Convex Hull") && rt.hull != null)
            {
                Debug.Log(rt.convexHull = new RegionConvexHull(rt.hull));
            }
        }

        void ReadRegionFromFile(TextAsset regionReadFile, ref Region region)
        {
            var stream = new MemoryStream(regionReadFile.bytes);
            StreamReader sr = null;

            try
            {
                sr = new StreamReader(stream);

                var header = sr.ReadLine();
                var headerData = header.Split(',');

                var offset = int.Parse(headerData[0]);                
                var horizontalStart = int.Parse(headerData[1]);
                var verticalStart = int.Parse(headerData[2]);

                Region r = new Region();
                string line = sr.ReadLine();
                for (int y = verticalStart; line != null; y--)
                {
                    for (int x = horizontalStart, i = 0; i < line.Length; x++, i++)
                        if (line[i] == 'x')
                            r.AddTile(x, y);
                    
                    line = sr.ReadLine();
                }
                r.ChangeOffset(offset);
                r.Normalize();
                region = r;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }
    }
}