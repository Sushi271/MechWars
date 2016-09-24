using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Linq;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class BuildingShadow : IBuildingPlacement
    {
        InputController inputController;
        Building prefab;
        Building shadow;

        bool destroyed;

        public bool InsideMap { get; private set; }
        public bool CannotBuild { get; private set; }

        public Vector2 Position { get; private set; }

        public BuildingShadow(InputController inputController, BuildingConstructionOrderAction orderAction)
        {
            this.inputController = inputController;
            prefab = orderAction.building;

            shadow = Object.Instantiate(prefab.gameObject).GetComponent<Building>();
            shadow.gameObject.SetLayerRecursively(LayerMask.NameToLayer(Layer.BuildingShadow));
            shadow.gameObject.name = prefab.gameObject.name + " shadow";
            shadow.isShadow = true;
            var rs = shadow.GetComponentsInChildren<Renderer>();
            //foreach (var r in rs)
            //{
            //    var m = r.material;
            //    var col = m.color;
            //    col.a = 0.5f;
            //    m.color = col;
            //}
        }

        public void Destroy()
        {
            Object.Destroy(shadow.gameObject);
            destroyed = true;
        }

        public void Update()
        {
            if (destroyed) return;
            UpdateLocation();
            UpdateLook();
        }

        void UpdateLocation()
        {
            var coords = inputController.Mouse.MapRaycast.PreciseCoords;
            InsideMap = coords.HasValue;
            if (!coords.HasValue) return;

            var p = coords.Value;
            var shape = prefab.Shape;
            var W = shape.Width;
            var H = shape.Height;

            // snapowanie siem:

            // czy trzeba przesunąć w poziomie i w pionie na krawędź pola
            //     (gdy rozmiar jest parzysty, to budynek leży między polami, na krawędzi)
            float horizontalOffset = W % 2 == 0 ? 0.5f : 0;
            float verticalOffset = H % 2 == 0 ? 0.5f : 0;

            // snapowanie w jedną albo w drugą stronę, jeszcze nie wiemy w którą, dlatego Floor i Ceil
            float xSnap1 = Mathf.Floor(p.x - horizontalOffset) + horizontalOffset;
            float xSnap2 = Mathf.Ceil(p.x - horizontalOffset) + horizontalOffset;
            float ySnap1 = Mathf.Floor(p.y - verticalOffset) + verticalOffset;
            float ySnap2 = Mathf.Ceil(p.y - verticalOffset) + verticalOffset;

            // wybieramy z Floor i Ceil bliższe snapowanie
            float closerXSnap = Mathf.Min(Mathf.Abs(p.x - xSnap1), Mathf.Abs(p.x - xSnap2));
            float closerYSnap = Mathf.Min(Mathf.Abs(p.y - ySnap1), Mathf.Abs(p.y - ySnap2));
            Position = new Vector2(closerXSnap, closerYSnap);

            shadow.Coords = Position; //ustawienie shadowowi snapowane współrzędne
            var allCoords = shadow.AllCoords.ToList();
            bool cannotBuild = allCoords.All(c => // dla każdego c we współrzędnych, które zajmie budynek sprawdzamy czy:
                Globals.Map[c] == null &&         // każde pole c jest wolne
                inputController.ConstructionRange.FieldInRange(c)); // i w zasięgu budowania

            CannotBuild = cannotBuild;
            shadow.transform.position = new Vector3(Position.x, 0, Position.y);
        }

        void UpdateLook()
        {
            var renderers = shadow.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                r.enabled = InsideMap; // zeby zniknal
                var m = r.material;
                var col = m.color;
                col.g = col.b = CannotBuild ? 0 : 1; // na czerwono
                m.color = col;
            }
        }
    }
}