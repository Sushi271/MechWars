using System.Collections.Generic;
using System.Linq;
using MechWars.GLRendering;
using MechWars.MapElements;
using UnityEngine;

namespace MechWars.Human
{
    public class SelectionBox
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public HashSet<MapElement> MapElementsInside { get; private set; }

        public SelectionBox()
        {
            MapElementsInside = new HashSet<MapElement>();
        }

        public void Draw()
        {
            Globals.GLRenderer.Schedule(new RectangleRenderTask(Color.black, Position, Size));
        }

        public void Update()
        {
            Size = (Vector2)Input.mousePosition - Position;

            var mapElements = GameObject.FindGameObjectsWithTag(Tag.MapElement).Select(go => go.GetComponent<MapElement>());
            var aaa = mapElements.Where(a => a == null);
            if (mapElements.Any(a => a == null))
                throw new System.Exception(string.Format("Object with tag \"{0}\" doesn't have MapElement script [{1}]", Tag.MapElement, aaa.Count()));

            var mapElementsScreenPos = mapElements.Select(a => new
                {
                    MapElement = a,
                    ScreenPosition = Globals.MainCamera.WorldToScreenPoint(a.transform.position)
                });

            var x0 = Mathf.Min(Position.x, Position.x + Size.x);
            var x1 = Mathf.Max(Position.x, Position.x + Size.x);
            var y0 = Mathf.Min(Position.y, Position.y + Size.y);
            var y1 = Mathf.Max(Position.y, Position.y + Size.y);

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
    }
}