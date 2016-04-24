using MechWars.MapElements.Orders;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Building : MapElement
    {
        #region TEMP

        int TEMP_selectedProductionOptionIndex = -1;
        public void TEMP_InitSelectedProductionOption()
        {
            if (unitProductionOptions.Count > 0)
                TEMP_selectedProductionOptionIndex = 0;
            TEMP_UpdateSelectedProductionOption();
        }
        public void TEMP_NextProductionOption()
        {
            if (unitProductionOptions.Count == 0) return;
            TEMP_selectedProductionOptionIndex++;
            if (TEMP_selectedProductionOptionIndex >= unitProductionOptions.Count)
                TEMP_selectedProductionOptionIndex = 0;
            TEMP_UpdateSelectedProductionOption();
        }
        public void TEMP_PreviousProductionOption()
        {
            if (unitProductionOptions.Count == 0) return;
            TEMP_selectedProductionOptionIndex--;
            if (TEMP_selectedProductionOptionIndex < 0)
                TEMP_selectedProductionOptionIndex = unitProductionOptions.Count - 1;
            TEMP_UpdateSelectedProductionOption();
        }
        public void TEMP_UpdateSelectedProductionOption()
        {
            if (0 <= TEMP_selectedProductionOptionIndex && TEMP_selectedProductionOptionIndex < unitProductionOptions.Count)
                TEMP_selectedProductionOption = unitProductionOptions[TEMP_selectedProductionOptionIndex];
            else TEMP_selectedProductionOption = null;
        }
        public UnitProductionOption TEMP_selectedProductionOption;

        int TEMP_selectedConstructionOptionIndex = -1;
        public void TEMP_InitSelectedConstructionOption()
        {
            if (buildingConstructionOptions.Count > 0)
                TEMP_selectedConstructionOptionIndex = 0;
            TEMP_UpdateSelectedConstructionOption();
        }
        public void TEMP_NextConstructionOption()
        {
            if (buildingConstructionOptions.Count == 0) return;
            TEMP_selectedConstructionOptionIndex++;
            if (TEMP_selectedConstructionOptionIndex >= buildingConstructionOptions.Count)
                TEMP_selectedConstructionOptionIndex = 0;
            TEMP_UpdateSelectedConstructionOption();
        }
        public void TEMP_PreviousConstructionOption()
        {
            if (buildingConstructionOptions.Count == 0) return;
            TEMP_selectedConstructionOptionIndex--;
            if (TEMP_selectedConstructionOptionIndex < 0)
                TEMP_selectedConstructionOptionIndex = buildingConstructionOptions.Count - 1;
            TEMP_UpdateSelectedConstructionOption();
        }
        public void TEMP_UpdateSelectedConstructionOption()
        {
            if (0 <= TEMP_selectedConstructionOptionIndex && TEMP_selectedConstructionOptionIndex < buildingConstructionOptions.Count)
                TEMP_selectedConstructionOption = buildingConstructionOptions[TEMP_selectedConstructionOptionIndex];
            else TEMP_selectedConstructionOption = null;
        }
        public BuildingConstructionOption TEMP_selectedConstructionOption;

        public Vector2 TEMP_buildLocation;
        #endregion

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
        public override bool Interactible { get { return true; } }
        
        public bool UnderConstruction {  get { return ConstructionInfo != null; } }
        public BuildingConstructionInfo ConstructionInfo { get; set; }
        
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

            TEMP_InitSelectedProductionOption();
            TEMP_InitSelectedConstructionOption();
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
    }
}
