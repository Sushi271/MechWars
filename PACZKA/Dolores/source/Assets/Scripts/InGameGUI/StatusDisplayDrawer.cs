using MechWars.GLRendering;
using MechWars.MapElements;
using MechWars.MapElements.Statistics;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.InGameGUI
{
    public class StatusDisplayDrawer : MonoBehaviour
    {
        struct DisplayState
        {
            public bool Hovered { get; set; }
            public bool Selected { get; set; }

            public DisplayState(bool hovered, bool selected = false)
                : this()
            {
                Hovered = hovered;
                Selected = selected;
            }
        }

        void OnGUI()
        {
            var inputController = Globals.Spectator.InputController;
            var hovered = inputController.HoverController.HoveredMapElements;
            var selected = inputController.SelectionMonitor.SelectedMapElements;

            var all = new Dictionary<MapElement, DisplayState>();
            foreach (var h in hovered)
                all.Add(h, new DisplayState(true));
            foreach (var s in selected)
            {
                DisplayState dispState;
                bool contains = all.TryGetValue(s, out dispState);
                if (contains) dispState.Selected = true;
                else all.Add(s, new DisplayState(false, true));
            }

            foreach (var kv in all)
            {
                if (kv.Key != null)
                {
                    var statusDisplay = new StatusDisplayInfo(kv.Key, kv.Value.Hovered, kv.Value.Selected);
                    DrawBorder(statusDisplay);
                    DrawHealthBar(statusDisplay);
                }
            }
        }

        void DrawBorder(StatusDisplayInfo statusDisplay)
        {
            var location = new Vector2(statusDisplay.Left, Screen.height - statusDisplay.Top);
            var size = statusDisplay.Size;
            var distance = 0.5f;

            Vector2 v00 = location;
            Vector2 v01 = location + Vector2.up * size.y;
            Vector2 v10 = location + Vector2.right * size.x;
            Vector2 v11 = location + Vector2.right * size.x + Vector2.up * size.y;

            float lineLength = 0.2f;

            Vector2 u = Vector2.up * size.y * lineLength;
            Vector2 d = Vector2.down * size.y * lineLength;
            Vector2 r = Vector2.right * size.x * lineLength;
            Vector2 l = Vector2.left * size.x * lineLength;

            Color color = !statusDisplay.Hovered ? Color.black :
                Globals.Spectator.InputController.FramesColor;

            Globals.GLRenderer.Schedule(new LineRenderTask(color, v00, v00 + r, distance));
            Globals.GLRenderer.Schedule(new LineRenderTask(color, v00, v00 + u, distance));

            Globals.GLRenderer.Schedule(new LineRenderTask(color, v01, v01 + r, distance));
            Globals.GLRenderer.Schedule(new LineRenderTask(color, v01, v01 + d, distance));

            Globals.GLRenderer.Schedule(new LineRenderTask(color, v10, v10 + l, distance));
            Globals.GLRenderer.Schedule(new LineRenderTask(color, v10, v10 + u, distance));

            Globals.GLRenderer.Schedule(new LineRenderTask(color, v11, v11 + l, distance));
            Globals.GLRenderer.Schedule(new LineRenderTask(color, v11, v11 + d, distance));

            if (statusDisplay.Selected)
            {
                Globals.GLRenderer.Schedule(new LineRenderTask(color, v00 + 2 * r, v10 + 2 * l, distance));
                Globals.GLRenderer.Schedule(new LineRenderTask(color, v01 + 2 * r, v11 + 2 * l, distance));
                Globals.GLRenderer.Schedule(new LineRenderTask(color, v00 + 2 * u, v01 + 2 * d, distance));
                Globals.GLRenderer.Schedule(new LineRenderTask(color, v10 + 2 * u, v11 + 2 * d, distance));
            }

        }

        void DrawHealthBar(StatusDisplayInfo statusDisplay)
        {
            var mapElement = statusDisplay.MapElement;
            var hitPoints = mapElement.Stats[StatNames.HitPoints];
            if (hitPoints == null) return;

            GUI.depth = (int)(statusDisplay.Distance * 10);
            GUI.BeginGroup(new Rect(statusDisplay.Location, statusDisplay.Size));

            Texture mainTexture;
            Texture sideTexture;
            Texture topTexture;
            if (mapElement.Army != null)
            {
                mainTexture = mapElement.Army.hpBarMain;
                sideTexture = mapElement.Army.hpBarSide;
                topTexture = mapElement.Army.hpBarTop;
            }
            else
            {
                mainTexture = Globals.Textures.hpBarNoArmy;
                sideTexture = Globals.Textures.hpBarNoArmySide;
                topTexture = Globals.Textures.hpBarNoArmyTop;
            }

            var baseWidth = topTexture.width + 2 * sideTexture.width;
            var baseHeight = sideTexture.height;

            var barAspectRatio = (float)baseHeight / baseWidth;
            var sideAspectRatio = (float)sideTexture.height / sideTexture.width;

            float distFromBounds = 0.1f;

            Vector2 barLocation = distFromBounds * statusDisplay.Size;
            float barWidth = (1 - 2 * distFromBounds) * statusDisplay.Width;
            float barHeight = barWidth * barAspectRatio;
            float borderThickness = barHeight / sideAspectRatio;

            float ratio = hitPoints.Value / hitPoints.MaxValue;
            float contentBarWidth = ratio * barWidth;
            float mainWidth = contentBarWidth - 2 * borderThickness;
            float mainHeight = barHeight - 2 * borderThickness;

            Vector2 topLocation = barLocation + Vector2.right * borderThickness;
            Vector2 mainLocation = topLocation + Vector2.up * borderThickness;
            Vector2 bottomLocation = mainLocation + Vector2.up * mainHeight;
            Vector2 rightSideLocation = topLocation + Vector2.right * mainWidth;

            Vector2 barSize = new Vector2(barWidth, barHeight);
            Vector2 sideSize = new Vector2(borderThickness, barHeight);
            Vector2 topSize = new Vector2(mainWidth, borderThickness);
            Vector2 mainSize = new Vector2(mainWidth, mainHeight);
            Vector2 leftSideSize = sideSize;
            float sideToFullRatio = borderThickness / barWidth;
            if (ratio < sideToFullRatio)
                leftSideSize = new Vector2(ratio * barWidth, barHeight);

            GUI.DrawTexture(new Rect(barLocation, barSize), Globals.Textures.hpBarBackground);
            GUI.DrawTexture(new Rect(barLocation, leftSideSize), sideTexture);
            if (mainWidth > 0)
            {
                GUI.DrawTexture(new Rect(topLocation, topSize), topTexture);
                GUI.DrawTexture(new Rect(mainLocation, mainSize), mainTexture);
                GUI.DrawTexture(new Rect(bottomLocation, topSize), topTexture);
            }
            if (rightSideLocation.x > barLocation.x)
                GUI.DrawTexture(new Rect(rightSideLocation, sideSize), sideTexture);
            GUI.EndGroup();
        }
    }
}