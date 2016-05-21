//#define DEBUG_HOVER_BOX

using System.Collections.Generic;
using System.Linq;
using MechWars.GLRendering;
using MechWars.MapElements;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class HoverBox
    {
        InputController inputController;

        public Vector2 Start { get; private set; }
        public Vector2 Size { get; private set; }

        public HashSet<MapElement> MapElementsInside { get; private set; }

        public HoverBox(InputController inputController, Vector2 start)
        {
            this.inputController = inputController;

            Start = start;
            UpdateSize();

            MapElementsInside = new HashSet<MapElement>();
        }
        
        public void Update()
        {
            UpdateSize();
            Draw();
            
            var mapElements = Globals.MapElementsDatabase.All;

            var mapElementsScreenPos = mapElements.Select(me => new
                {
                    MapElement = me,
                    ScreenPosition = Globals.MainCamera.WorldToScreenPoint(me.transform.position)
                });

            var x0 = Mathf.Min(Start.x, Start.x + Size.x);
            var x1 = Mathf.Max(Start.x, Start.x + Size.x);
            var y0 = Mathf.Min(Start.y, Start.y + Size.y);
            var y1 = Mathf.Max(Start.y, Start.y + Size.y);

            var newMapElementsInside = new HashSet<MapElement>(mapElementsScreenPos.Where(mesp =>
                {
                    var pos = mesp.ScreenPosition;
                    return
                        x0 <= pos.x && pos.x <= x1 &&
                        y0 <= pos.y && pos.y <= y1;
                }).Select(mesp => mesp.MapElement));

            var removedMapElements = MapElementsInside.Where(c => !newMapElementsInside.Contains(c)).ToList();
            foreach (var rme in removedMapElements)
                MapElementsInside.Remove(rme);
            foreach (var c in newMapElementsInside)
                MapElementsInside.Add(c);
        }

        void UpdateSize()
        {
            Size = inputController.Mouse.Position - Start;
        }

        void Draw()
        {
            Globals.GLRenderer.Schedule(new RectangleRenderTask(inputController.HoverBoxColor, Start, Size));
        }
    }
}