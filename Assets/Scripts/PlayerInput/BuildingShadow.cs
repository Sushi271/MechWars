using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using System.Linq;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class BuildingShadow
    {
        InputController inputController;

        Building constructor;
        BuildingConstructionOrderAction orderAction;

        Building shadow;

        public bool InsideMap { get; private set; }
        public bool PositionOccupied { get; private set; }
        public bool PositionValid { get { return InsideMap && PositionOccupied; } }

        public Vector2 Position { get; private set; }

        public BuildingShadow(InputController inputController,
            Building constructor, BuildingConstructionOrderAction orderAction)
        {
            this.inputController = inputController;

            this.constructor = constructor;
            this.orderAction = orderAction;

            shadow = Object.Instantiate(orderAction.building.gameObject).GetComponent<Building>();
            shadow.gameObject.name = orderAction.building.gameObject.name + " shadow";
            shadow.isShadow = true;
            var rs = shadow.GetComponentsInChildren<Renderer>();
            foreach (var r in rs)
            {
                var m = r.material;
                var col = m.color;
                col.a = 0.5f;
                m.color = col;
            }
        }

        public void Update()
        {
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
            var shape = orderAction.building.Shape;
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
            bool isOccupied = false;
            foreach (var c in allCoords) //dla każdego c we współrzędnych, które zajmie budynek
            {
                if (Globals.FieldReservationMap[c] != null) // jeżeli choć jedno z pól jest zajete
                {
                    isOccupied = true;
                    break;
                }
            }
            // powyżej prosi się o Any() z lambdą z LInQ, ale to Twój kod :P - Sushi

            PositionOccupied = isOccupied;
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
                col.g = col.b = PositionOccupied ? 0 : 1; // na czerwono
                m.color = col;
            }
        }
    }
}