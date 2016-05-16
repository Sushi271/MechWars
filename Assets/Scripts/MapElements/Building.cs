using MechWars.MapElements.Orders;
using MechWars.MapElements.Orders.Actions;
using MechWars.MapElements.Production;
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
        public List<TechnologyDevelopmentOption> technologyDevelopmentOptions;

        public event System.Action OnConstructionFinished;
        
        public bool UnderConstruction { get { return ConstructionInfo != null; } }
        public BuildingConstructionInfo ConstructionInfo { get; private set; }
        protected override bool CanAddToArmy { get { return true; } }
        public override bool Selectable { get { return true; } }
        public override bool CanBeAttacked { get { return true; } }

        HashSet<IVector2> allNeighbourFields;

        public Building()
        {
            allNeighbourFields = new HashSet<IVector2>();
        }

        protected override OrderExecutor CreateOrderExecutor()
        {
            return new OrderExecutor(() => new IdleOrder(this), false);
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
                    if (Globals.FieldReservationMap[field] != this)
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

            if (!buildingCO.CheckRequirements(army))
                throw new System.Exception(string.Format(
                    "Building {0} is not meeting requirements to construct Building {1}", this, buildingCO.building));

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
                throw new System.Exception(string.Format(
                    "Cannot construct Building {0} in location {1} - it's occupied.", building, location));
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

        public void FinishConstruction()
        {
            ConstructionInfo = null;
            OrderExecutor.Enable();
            if (OnConstructionFinished != null)
                OnConstructionFinished();
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
                sb.AppendLine("Default order:");
                sb.AppendLine(string.Format("    {0}",
                    OrderExecutor.DefaultOrder == null ? "---" :
                    OrderExecutor.DefaultOrder.ToString()));
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
