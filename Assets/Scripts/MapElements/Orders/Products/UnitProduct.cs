using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders.Products
{
    public class UnitProduct : Product
    {
        public Unit Unit { get; private set; }

        public UnitProduct(Building producer, Unit unit)
        {
            Unit = unit;

            var prodOp = producer.unitProductionOptions.Find(po => po.unit == unit);
            if (prodOp == null)
                throw new System.Exception(string.Format("Building {0} cannot produce Unit {1}", producer, Unit));
            if (!prodOp.CheckRequirements(producer.army))
                throw new System.Exception(string.Format("Building {0} is not meeting requirements to produce Unit {1}", producer, Unit));

            Name = unit.mapElementName;
            Cost = prodOp.cost;
            ProductionTime = prodOp.productionTime;
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), Unit);
        }
    }
}