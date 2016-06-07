using MechWars.MapElements.Orders;
using MechWars.MapElements.Orders.Actions;
using MechWars.MapElements.Orders_OLD.Products;
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

        protected override OrderExecutor CreateOrderExecutor(bool enabled = false)
        {
            return base.CreateOrderExecutor(enabled);
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            if (!UnderConstruction) OrderExecutor.Enable();

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

            if (UnderConstruction)
                transform.localScale = new Vector3(1, ConstructionInfo.TotalProgress, 1);
            else transform.localScale = Vector3.one;
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

        public BuildingProduct Construct(IBuildingConstructArgs args, Vector2 location)
        {
            if (UnderConstruction)
                throw new System.Exception(string.Format(
                    "Building {0} cannot construct buildings - it's under construction.", this));
            
            if (orderActions.OfType<BuildingConstructionOrderAction>().None(oa => oa.Equals(args)))
                throw new System.Exception(string.Format(
                    "Building {0} cannot construct Building {1}", this, args.Building));

            if (!args.CheckRequirements(army))
                throw new System.Exception(string.Format(
                    "Building {0} is not meeting requirements to construct Building {1}", this, args.Building));

            var prefab = args.Building;
            int startCost = args.StartCost;
            if (startCost > army.resources)
            {
                Debug.Log(string.Format("Not enough resources to start Building {0} construction", prefab));
                return null;
            }

            prefab.Coords = location;
            if (prefab.AllCoords.Any(c => Globals.FieldReservationMap[c]))
            {
                throw new System.Exception(string.Format(
                    "Cannot construct Building {0} in location {1} - it's occupied.", prefab, location));
            }

            army.resources -= startCost;

            var gameObject = Instantiate(prefab.gameObject);
            gameObject.transform.parent = transform.parent;
            gameObject.transform.position = new Vector3(location.x, 0, location.y);
            gameObject.name = prefab.gameObject.name;

            var building = gameObject.GetComponent<Building>();
            building.InitializeReservation();
            building.army = army;
            building.resourceValue = startCost;
            building.ReadStats();

            var buildingProduct = new BuildingProduct(building, args.Cost, args.ProductionTime);

            var bci = new BuildingConstructionInfo(buildingProduct, startCost);
            building.ConstructionInfo = bci;
            var hpStat = building.Stats[StatNames.HitPoints];
            if (hpStat == null)
                throw new System.Exception(string.Format(
                    "Building {0} doesn't have {1} Stat.", prefab, StatNames.HitPoints));
            hpStat.Value = bci.TotalProgress * hpStat.MaxValue;

            return buildingProduct;
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

        public override StringBuilder DEBUG_PrintStatus(StringBuilder sb)
        {
            base.DEBUG_PrintStatus(sb)
                .AppendLine()
                .AppendLine(string.Format("Is resource deposit: {0}", isResourceDeposit))
                .AppendLine(string.Format("Under construction: {0}", UnderConstruction));
            if (UnderConstruction)
            {
                sb
                    .AppendLine("Construction info:")
                    .AppendLine(string.Format("    Paid/Cost: {0} / {1} ({2:P1})", ConstructionInfo.Paid, ConstructionInfo.Cost, ConstructionInfo.TotalProgress))
                    .Append(string.Format("    Construction time: {0}", ConstructionInfo.ConstructionTime));
            }
            else DEBUG_PrintOrders(sb);

            return sb;
        }
    }
}
