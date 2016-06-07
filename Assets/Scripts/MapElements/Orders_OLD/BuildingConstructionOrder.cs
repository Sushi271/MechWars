﻿using System;
using MechWars.MapElements.Orders_OLD.Products;
using MechWars.MapElements.Statistics;
using UnityEngine;

namespace MechWars.MapElements.Orders_OLD
{
    public class BuildingConstructionOrder : Order_OLD
    {
        BuildingConstructionInfo constructionInfo;
        ProductionOrder productionOrder;

        public Building ConstructingBuilding { get; private set; }
        public Building ConstructedBuilding { get; private set; }

        public BuildingConstructionOrder(Building orderedBuilding, BuildingProduct buildingProduct)
            : base("Construction", orderedBuilding)
        {
            ConstructingBuilding = orderedBuilding;
            ConstructedBuilding = buildingProduct.Building;

            constructionInfo = ConstructedBuilding.ConstructionInfo;
            productionOrder = new ProductionOrder(ConstructingBuilding, buildingProduct, constructionInfo.Paid);
        }

        protected override bool RegularUpdate()
        {
            if (!ConstructedBuilding.Alive)
            {
                Stop();
                return false;
            }
            productionOrder.Update();
            PostStepUpdate();
            if (productionOrder.Done)
            {
                FinalizeConstruction();
                return true;
            }
            return false;
        }

        protected override bool StoppingUpdate()
        {
            productionOrder.Update();
            if (productionOrder.Stopped)
            {
                if (productionOrder.Done) FinalizeConstruction();
                else ConstructedBuilding.army = null;
                return true;
            }
            else return false;
        }

        protected override void TerminateCore()
        {
            ConstructedBuilding.army = null;
        }

        void PostStepUpdate()
        {
            constructionInfo.Paid = productionOrder.Paid;
            var oldProgress = constructionInfo.TotalProgress;
            var newProgress = (float)constructionInfo.Paid / constructionInfo.Cost;
            var dProgress = newProgress - oldProgress;
            constructionInfo.TotalProgress = newProgress;

            ConstructedBuilding.resourceValue = constructionInfo.Paid;
            var hpStat = ConstructedBuilding.Stats[StatNames.HitPoints];
            if (hpStat == null)
                throw new Exception(string.Format(
                    "Building {0} doesn't have {1} Stat.", ConstructedBuilding, StatNames.HitPoints));
            hpStat.Value += dProgress * hpStat.MaxValue;
        }

        void FinalizeConstruction()
        {
            ConstructedBuilding.FinishConstruction();
            Debug.Log(string.Format("Construction of {0} complete.", ConstructedBuilding));
        }

        protected override void OnStopCalled()
        {
            productionOrder.DontRevert = true;
            productionOrder.Stop();
        }

        protected override string SpecificsToString()
        {
            return string.Format("{0}", ConstructedBuilding);
        }
    }
}