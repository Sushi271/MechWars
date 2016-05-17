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

            var mapElementObjects = GameObject.FindGameObjectsWithTag(Tag.MapElement);
            var mapElements = mapElementObjects.Select(go => go.GetComponent<MapElement>());

            // TODO: WTF why this exception is sometimes randomly unpredictably being thrown?!
#if DEBUG_HOVER_BOX
            var nullMapElements = mapElementObjects.Where(me => me.GetComponent<MapElement>() == null);
            if (nullMapElements.Count() > 0)
            {
                throw new System.Exception(string.Format("Object with tag \"{0}\" doesn't have MapElement script {1}, {2}",
                    Tag.MapElement, nullMapElements.ToDebugMessage(), mapElements.ToDebugMessage()));
            }
#else
            mapElements = mapElements.Where(me => me != null && me.GetComponent<MapElement>() != null);
#endif

            var mapElementsScreenPos = mapElements.Select(a => new
                {
                    MapElement = a,
                    ScreenPosition = Globals.MainCamera.WorldToScreenPoint(a.transform.position)
                });

            var x0 = Mathf.Min(Start.x, Start.x + Size.x);
            var x1 = Mathf.Max(Start.x, Start.x + Size.x);
            var y0 = Mathf.Min(Start.y, Start.y + Size.y);
            var y1 = Mathf.Max(Start.y, Start.y + Size.y);

            var newMapElementsInside = new HashSet<MapElement>(mapElementsScreenPos.Where(asp =>
                {
                    var pos = asp.ScreenPosition;
                    return
                        x0 <= pos.x && pos.x <= x1 &&
                        y0 <= pos.y && pos.y <= y1;
                }).Select(asp => asp.MapElement));

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