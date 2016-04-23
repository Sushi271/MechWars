using MechWars.MapElements.Orders;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Building : MapElement
    {
        public bool isResourceDeposit;

        public QueueOrderExecutor OrderExecutor { get; private set; }
        public override bool Interactible { get { return true; } }

        HashSet<IVector2> allNeighbourFields;

        public Building()
        {
            selectable = true;
            OrderExecutor = new QueueOrderExecutor();
            allNeighbourFields = new HashSet<IVector2>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitializeNeighbourFields();
        }

        void InitializeNeighbourFields()
        {
            var deltas = UnityExtensions.NeighbourDeltas;
            foreach (var c in AllCoords)
                foreach (var d in deltas)
                {
                    var field = c + d;
                    if (Globals.FieldReservationMap.CoordsInside(field) &&
                        Globals.FieldReservationMap[field] != this)
                        allNeighbourFields.Add(field);
                }
        }

        public Unit Spawn(Unit unit)
        {
            var freeNeighbourFields =
                (from n in allNeighbourFields
                where Globals.FieldReservationMap[n] == null
                select n).ToList();
            if (freeNeighbourFields.Count == 0)
                return null;

            var field = new System.Random().Choice(freeNeighbourFields);
            var gameObject = Instantiate(unit.gameObject);
            gameObject.transform.parent = transform.parent;
            gameObject.transform.position = new Vector3(field.X, 0, field.Y);
            gameObject.name = unit.gameObject.name;
            var newUnit = gameObject.GetComponent<Unit>();
            newUnit.army = army;

            return newUnit;
        }

        public void GiveOrder(IOrder order)
        {
            if (order is Order<Building> || order is Order<MapElement>)
                OrderExecutor.Give(order);
            else throw new System.Exception(string.Format(
                "Order {0} not suitable for MapElement {1}.", order, this));
        }

        public void CancelOrder(IOrder order)
        {
            OrderExecutor.Cancel(order);
        }

        public void CancelAllOrders()
        {
            OrderExecutor.CancelAll();
        }
    }
}
