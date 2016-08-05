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
            if (!coords.HasValue)
                return;

            var p = coords.Value;
            var shape = prefab.Shape;
            var W = shape.Width;
            var H = shape.Height;

            //punkty do snapowania siem
            float xSnap1, xSnap2;
            if (W % 2 == 0)
            {
                xSnap1 = Mathf.Floor(p.x - 0.5f) + 0.5f;
                xSnap2 = Mathf.Ceil(p.x - 0.5f) + 0.5f;
            }
            else
            {
                xSnap1 = Mathf.Floor(p.x);
                xSnap2 = Mathf.Ceil(p.x);
            }
            float ySnap1, ySnap2;
            if (H % 2 == 0)
            {
                ySnap1 = Mathf.Floor(p.y - 0.5f) + 0.5f;
                ySnap2 = Mathf.Ceil(p.y - 0.5f) + 0.5f;
            }
            else
            {
                ySnap1 = Mathf.Floor(p.y);
                ySnap2 = Mathf.Ceil(p.y);
            }

            float x, y;
            if (Mathf.Abs(p.x - xSnap1) > Mathf.Abs(p.x - xSnap2))
                x = xSnap2;
            else
                x = xSnap1;
            if (Mathf.Abs(p.y - ySnap1) > Mathf.Abs(p.y - ySnap2))
                y = ySnap2;
            else
                y = ySnap1;

            Position = new Vector2(x, y);

            shadow.Coords = Position; //ustawienie shadowowi snapowane współrzędne
            var allCoords = shadow.AllCoords.ToList();
            bool cannotBuild = false;
            foreach (var c in allCoords) //dla każdego c we współrzędnych, które zajmie budynek
            {
                if (Globals.Map[c] != null) // jeżeli choć jedno z pól jest zajete
                {
                    cannotBuild = true;
                    break;
                }
                if (!inputController.ConstructionRange.FieldInRange(c)) // lub jest poza zasiegiem
                {
                    cannotBuild = true;
                    break;
                }
            }
            // powyżej prosi się o Any() z lambdą z LInQ, ale to Twój kod :P - Sushi
            
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