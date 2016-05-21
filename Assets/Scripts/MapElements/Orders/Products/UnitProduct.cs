using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders.Products
{
    public class UnitProduct : Product
    {
        public Unit Unit { get; private set; }

        public UnitProduct(Unit unit, int cost, float productionTime)
            : base(unit.mapElementName, cost, productionTime)
        {
            Unit = unit;
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), Unit);
        }
    }
}