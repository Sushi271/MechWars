using MechWars.MapElements.Orders;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Building : MapElement
    {
        public bool isResourceDeposit;
        public List<UnitProductionOption> unitProductionOptions;
        public List<BuildingConstructionOption> buildingConstructionOptions;

        QueueOrderExecutor orderExecutor;
        public QueueOrderExecutor OrderExecutor
        {
            get
            {
                if (UnderConstruction)
                    throw new System.Exception(string.Format(
                        "Building {0} cannot provide OrderExecutor - it's under construction.", this));
                return orderExecutor;
            }
            private set { orderExecutor = value; }
        }

        public bool UnderConstruction { get { return ConstructionInfo != null; } }
        public BuildingConstructionInfo ConstructionInfo { get; set; }

        HashSet<IVector2> allNeighbourFields;

        public Building()
        {
            selectable = true;
            OrderExecutor = new QueueOrderExecutor(() => new IdleOrder(this));
            allNeighbourFields = new HashSet<IVector2>();
        }

        protected override void OnStart()
        {

            base.OnStart();
            if (isShadow) return;

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

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isShadow) return;

            if (UnderConstruction)
                transform.localScale = new Vector3(1, ConstructionInfo.TotalProgress, 1);
            else
            {
                transform.localScale = Vector3.one;
                OrderExecutor.Update();
            }
        }

        public Unit Spawn(Unit unit)
        {
            if (UnderConstruction)
                throw new System.Exception(string.Format(
                    "Building {0} cannot spawn units - it's under construction.", this));

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
            newUnit.InitializeReservation();
            newUnit.army = army;

            return newUnit;
        }

        public Building Construct(BuildingConstructionOption buildingCO, Vector2 location)
        {
            if (UnderConstruction)
                throw new System.Exception(string.Format(
                    "Building {0} cannot construct buildings - it's under construction.", this));

            if (!buildingConstructionOptions.Contains(buildingCO))
                throw new System.Exception(string.Format(
                    "Building {0} cannot construct Building {1}", this, buildingCO.building));

            var building = buildingCO.building;
            var bci = new BuildingConstructionInfo(buildingCO);
            int startCost = Mathf.CeilToInt(bci.Cost * bci.TotalProgress);
            if (startCost > army.resources)
            {
                Debug.Log(string.Format("Not enough resources to start Building {0} construction", building));
                return null;
            }

            building.Coords = location;
            if (building.AllCoords.Any(c => Globals.FieldReservationMap[c]))
            {
                // TODO: go back to exception, once the SAFE control is on the level of mouse choosing
                Debug.Log(string.Format(
                //throw new System.Exception(string.Format(
                "Cannot construct Building {0} in location {1} - it's occupied.", building, location));
                return null;
            }

            army.resources -= startCost;
            bci.Paid += startCost;

            var gameObject = Instantiate(building.gameObject);
            gameObject.transform.parent = transform.parent;
            gameObject.transform.position = new Vector3(location.x, 0, location.y);
            gameObject.name = building.gameObject.name;
            var newBuilding = gameObject.GetComponent<Building>();
            newBuilding.InitializeReservation();
            newBuilding.army = army;
            newBuilding.ConstructionInfo = bci;
            newBuilding.resourceValue = bci.Paid;
            newBuilding.ReadStats();
            var hpStat = newBuilding.Stats[StatNames.HitPoints];
            if (hpStat == null)
                throw new System.Exception(string.Format(
                    "Building {0} doesn't have {1} Stat.", building, StatNames.HitPoints));
            hpStat.Value = bci.TotalProgress * hpStat.MaxValue;

            return newBuilding;
        }

        public void GiveOrder(IOrder order)
        {
            if (UnderConstruction)
                throw new System.Exception(string.Format(
                    "Building {0} cannot take orders - it's under construction.", this));

            if (order is Order<Building> || order is Order<MapElement>)
                OrderExecutor.Give(order);
            else throw new System.Exception(string.Format(
                "Order {0} not suitable for MapElement {1}.", order, this));
        }

        public void CancelCurrentOrder()
        {
            if (UnderConstruction)
                throw new System.Exception(string.Format(
                    "Building {0} cannot cancel orders - it's under construction.", this));
            if (OrderExecutor.Count > 0)
                OrderExecutor.Cancel(0);
        }

        public void CancelOrder(IOrder order)
        {
            if (UnderConstruction)
                throw new System.Exception(string.Format(
                    "Building {0} cannot cancel orders - it's under construction.", this));
            OrderExecutor.Cancel(order);
        }

        public void CancelAllOrders()
        {
            if (UnderConstruction)
                throw new System.Exception(string.Format(
                    "Building {0} cannot cancel orders - it's under construction.", this));
            OrderExecutor.CancelAll();
        }

        protected override void OnLifeEnd()
        {
            base.OnLifeEnd();
            if (!UnderConstruction)
                OrderExecutor.Terminate();
        }

        public override StringBuilder TEMP_PrintStatus()
        {
            var sb = base.TEMP_PrintStatus().AppendLine()
                .AppendLine(string.Format("Is resource deposit: {0}", isResourceDeposit))
                .AppendLine(string.Format("Under construction: {0}", UnderConstruction));
            if (UnderConstruction)
            {
                sb.AppendLine("Construction info:")
                    .AppendLine(string.Format("    Paid/Cost: {0} / {1} ({2:P1})", ConstructionInfo.Paid, ConstructionInfo.Cost, ConstructionInfo.TotalProgress))
                    .Append(string.Format("    Construction time: {0}", ConstructionInfo.ConstructionTime));
            }
            else
            {
                sb.AppendLine("Order queue:");
                if (OrderExecutor.Count == 0)
                    sb.Append("    ---");
                else for (int i = 0; i < OrderExecutor.Count; i++)
                    {
                        string line = string.Format("{0}: {1}", i, OrderExecutor[i]);
                        if (i < OrderExecutor.Count - 1)
                            sb.AppendLine(line);
                        else sb.Append(line);
                    }
            }

            return sb;
        }
    }
}
