using MechWars.AI.Regions;
using MechWars.Utils;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MechWars.Editors
{
    [CustomEditor(typeof(RegionTest))]
    public class RegionTestEditor : Editor
    {
        int tileX;
        int tileY;

        int dpt = 1;
        Vector2 otherPoint;

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

            dpt = EditorGUILayout.IntField("Grid's DPT", dpt);
            if (GUILayout.Button("Test ConvexHull.Contains with grid") && rt.convexHull != null)
            {
                TestConvexHullContains(rt.convexHull, dpt);
            }

            otherPoint = EditorGUILayout.Vector2Field("Other point", otherPoint);
            if (GUILayout.Button("Test ConvexHull.GetClosestPointTo") && rt.convexHull != null)
            {
                Debug.Log(rt.convexHull.GetPointClosestTo(otherPoint));
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

        void TestConvexHullContains(RegionConvexHull convexHull, int dpt)
        {
            if (dpt < 1) return;

            float delta = dpt.Reciproc();
            var bds = convexHull.Bounds;
            float xEnd = bds.xMax + 0.5f * (1 - delta);
            float yEnd = bds.yMin - 0.5f * (1 - delta);

            var sb = new StringBuilder();
            float y = bds.yMax + 0.5f * (1 - delta);
            do
            {
                float x = bds.xMin - 0.5f * (1 - delta);
                
                do sb.Append(convexHull.Contains(new Vector2(x, y)) ? 'x' : '.');
                while ((x += delta) < xEnd);
                sb.AppendLine();
            }
            while ((y -= delta) > yEnd);
            Debug.Log(sb.ToString());
        }
    }
}