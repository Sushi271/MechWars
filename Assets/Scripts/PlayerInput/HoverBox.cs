using System.Collections.Generic;
using System.Linq;
using MechWars.GLRendering;
using MechWars.MapElements;
using UnityEngine;
using MechWars.Utils;

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

            // bounds of hoverbox on screen
            var x0 = Mathf.Min(Start.x, Start.x + Size.x);
            var x1 = Mathf.Max(Start.x, Start.x + Size.x);
            var y0 = Mathf.Min(Start.y, Start.y + Size.y);
            var y1 = Mathf.Max(Start.y, Start.y + Size.y);

            // rays from corners of hoverbox
            var rays = new[]
            {
                Globals.MainCamera.ScreenPointToRay(new Vector3(x0, y0, 0)),
                Globals.MainCamera.ScreenPointToRay(new Vector3(x0, y1, 0)),
                Globals.MainCamera.ScreenPointToRay(new Vector3(x1, y0, 0)),
                Globals.MainCamera.ScreenPointToRay(new Vector3(x1, y1, 0))
            };

            // corners of hoverbox cast on the surface
            var corners = rays.Select(r =>
            {
                RaycastHit hitInfo;
                bool hit = Physics.Raycast(r, out hitInfo, Mathf.Infinity, LayerMask.GetMask(Layer.HoverBoxRaycast));
                if (!hit) throw new System.Exception("No hit on HoverBoxRaycast (this should never happen).");
                return hitInfo.point;
            });

            // axis-aligned bounding rectangle for shape being the cast of the hoverbox
            var mapSize = Globals.Map.Size;
            var x0_3 = Mathf.RoundToInt(Mathf.Clamp(corners.Aggregate((v1, v2) => v1.x < v2.x ? v1 : v2).x, 0, mapSize));
            var x1_3 = Mathf.RoundToInt(Mathf.Clamp(corners.Aggregate((v1, v2) => v1.x > v2.x ? v1 : v2).x, 0, mapSize));
            var z0_3 = Mathf.RoundToInt(Mathf.Clamp(corners.Aggregate((v1, v2) => v1.z < v2.z ? v1 : v2).z, 0, mapSize));
            var z1_3 = Mathf.RoundToInt(Mathf.Clamp(corners.Aggregate((v1, v2) => v1.z > v2.z ? v1 : v2).z, 0, mapSize));
            var location = new IVector2(x0_3, z0_3);
            var size = new IVector2(x1_3 + 1, z1_3 + 1) - location;
            var range = new RectangleBounds(location, size);

            // searching quadtrees from all armies with visible actions for mapelements
            var mapElements = new List<MapElement>();
            foreach (var a in Globals.Armies)
            {
                if (!a.actionsVisible) continue;
                var quadTrees = new[] { a.AlliesQuadTree, a.EnemiesQuadTree, a.ResourcesQuadTree };
                foreach (var qt in quadTrees)
                    mapElements.AddRange(qt.QueryRange(range).Select(qtme => qtme.MapElement));
            }
            mapElements = mapElements.Distinct().ToList();
            
            // filtering mapelements by casting them back onto screen
            var mapElementsScreenPos = mapElements.Select(me => new
            {
                MapElement = me,
                ScreenPosition = Globals.MainCamera.WorldToScreenPoint(me.transform.position)
            });
            var newMapElementsInside = new HashSet<MapElement>(mapElementsScreenPos.Where(mesp =>
                {
                    var pos = mesp.ScreenPosition;
                    return
                        x0 <= pos.x && pos.x <= x1 &&
                        y0 <= pos.y && pos.y <= y1;
                }).Select(mesp => mesp.MapElement));

            // handling hoverbox mapelements collection
            var removedMapElements = MapElementsInside.Where(c => !newMapElementsInside.Contains(c)).ToList();
            foreach (var rme in removedMapElements)
                MapElementsInside.Remove(rme);
            foreach (var c in newMapElementsInside)
                MapElementsInside.Add(c);
            Debug.Log(MapElementsInside.ToDebugMessage());
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