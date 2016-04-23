using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders.Products
{
    public class UnitProduct : Product
    {
        public Unit Unit { get; private set; }

        public UnitProduct(Unit unit)
        {
            Unit = unit;
            Unit.ReadStats();

            var costStat = unit.Stats[StatNames.Cost];
            var prodTimeStat = unit.Stats[StatNames.ProductionTime];

            Name = unit.mapElementName;
            Cost = costStat == null ? 0 : (int)costStat.Value;
            ProductionTime = prodTimeStat == null ? 0 : prodTimeStat.Value;
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), Unit);
        }
    }
}