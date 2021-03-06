﻿using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class HarvestOrder : ComplexOrder
    {
        public override string Name { get { return "Harvest"; } }

        Resource resource;

        public Unit Unit { get; private set; }
        public Resource Resource
        {
            get { return resource; }
            private set
            {
                if (resource == value) return;
                resource = value;
            }
        }
        public Building Deposit { get; private set; }

        IVector2 DepositClosestCoords { get { return Deposit.GetClosestFieldTo(Unit.Coords); } }

        IVector2? resourceCoords;

        MoveOrder moveOrder;
        CollectOrder collectOrder;
        DepositOrder depositOrder;

        HarvestMode mode;

        Stat carriedResourceStat;

        public bool TankFull { get { return carriedResourceStat.HasMaxValue; } }
        public bool TankEmpty { get { return carriedResourceStat.Value == 0; } }

        public HarvestOrder(Unit unit, Resource resource)
            : base(unit)
        {
            DebugHelper.BreakPoint(() => resource == null);

            Unit = unit;
            Resource = resource;
            mode = HarvestMode.Collect;
        }

        public HarvestOrder(Unit unit, Building deposit)
            : base(unit)
        {
            Unit = unit;
            Deposit = deposit;
            mode = HarvestMode.Deposit;
        }

        protected override void OnStart()
        {
            DebugHelper.BreakPoint(() => resource == null);

            TryFail(OrderResultAsserts.AssertMapElementHasStat(MapElement, StatNames.CarriedResource, out carriedResourceStat));
            if (Deposit != null) TryFail(OrderResultAsserts.AssertBuildingIsResourceDeposit(Deposit));
            if (Failed) return;

            UpdateResourceCoords();
            GiveNewSubOrder();
        }

        protected override void OnUpdate()
        {
            if (!Resource.IsTrueNull())
            {
                CorrectResource();
                if (Resource.value == 0)
                {
                    Resource = null;
                    UpdateResourceCoords();
                }
            }
            if (!Deposit.IsTrueNull() && Deposit.Dying)
                Deposit = null;
        }

        void UpdateResourceCoords()
        {
            resourceCoords = Resource == null ? null : new IVector2?(Resource.Coords.Round());
        }

        void CorrectResource()
        {
            if (!Resource.IsGhost)
            {
                if (Resource.Dying) return;

                var targetVisible = Resource.VisibleToArmies[MapElement.Army];
                if (targetVisible) return;
                
                var ghost = Resource.Ghosts[MapElement.Army];
                if (ghost == null)
                    throw new System.Exception("Resource has no Ghost, though it CanHaveGhosts and it's not visible by harvesting Army.");

                Resource = (Resource)ghost;
                UpdateResourceCoords();
            }
            else
            {
                if (!Resource.GhostRemoved) return;

                Resource = (Resource)Resource.OriginalMapElement;
                UpdateResourceCoords();
            }
        }

        protected override void OnSubOrderUpdating()
        {
            if (SubOrder == moveOrder && 
                mode == HarvestMode.Deposit &&
                Deposit == null)
                    moveOrder.Stop();
        }

        protected override void OnSubOrderStopped()
        {
            if (SubOrder == moveOrder) moveOrder = null;
            else if (SubOrder == collectOrder) collectOrder = null;
            else if (SubOrder == depositOrder) depositOrder = null;

            if (State != OrderState.Stopping && !InFinalState)
                GiveNewSubOrder();
        }

        protected override void OnSubOrderFinished()
        {
            if (SubOrder == moveOrder)
            {
                moveOrder = null;
                if (mode == HarvestMode.Collect)
                {
                    Resource = null;
                    resourceCoords = null;
                    var newResource = MapElement.PickClosestResourceInRange(StatNames.ViewRange);
                    if (newResource != null &&
                        Unit.Coords.Round().IsNeighbourTo(newResource.Coords.Round()))
                    {
                        Resource = newResource;
                        UpdateResourceCoords();

                        collectOrder = new CollectOrder(Unit, Resource);
                        GiveSubOrder(collectOrder);
                    }
                    else GiveNewSubOrder();
                }
                else if (mode == HarvestMode.Deposit)
                {
                    Deposit = PickDeposit();
                    if (Deposit != null &&
                        Unit.Coords.Round().IsNeighbourTo(DepositClosestCoords))
                    {
                        depositOrder = new DepositOrder(Unit, Deposit);
                        GiveSubOrder(depositOrder);
                    }
                    else GiveNewSubOrder();
                }
            }
            else if (SubOrder == collectOrder)
            {
                if (!TankFull)
                {
                    Resource = null;
                    resourceCoords = null;
                }
                else if (Resource.value == 0)
                {
                    Resource = MapElement.PickClosestResourceInRange(StatNames.ViewRange);
                    UpdateResourceCoords();
                }
                GiveNewSubOrder();
            }
            else if (SubOrder == depositOrder)
            {
                if (!TankEmpty)
                    Deposit = null;
                GiveNewSubOrder();
            }
        }

        void GiveNewSubOrder()
        {
            if (mode == HarvestMode.Collect)
            {
                if (TankFull)
                {
                    mode = HarvestMode.Deposit;
                    GiveNewSubOrder();
                }
                else
                {
                    if (resourceCoords == null)
                    {
                        Resource = MapElement.PickClosestResourceInRange(StatNames.ViewRange);
                        if (Resource == null)
                        {
                            if (!TankEmpty)
                            {
                                resourceCoords = Unit.Coords.Round();
                                mode = HarvestMode.Deposit;
                                GiveNewSubOrder();
                            }
                            else Succeed();
                            return;
                        }
                        UpdateResourceCoords();
                    }
                    if (resourceCoords != null)
                    {
                        moveOrder = new MoveOrder(Unit, resourceCoords.Value);
                        GiveSubOrder(moveOrder);
                    }
                }
            }
            else if (mode == HarvestMode.Deposit)
            {
                if (TankEmpty)
                {
                    mode = HarvestMode.Collect;
                    GiveNewSubOrder();
                }
                else
                {
                    if (Deposit == null)
                        Deposit = PickDeposit();
                    if (Deposit == null)
                        Succeed();
                    else
                    {
                        moveOrder = new MoveOrder(Unit, DepositClosestCoords);
                        GiveSubOrder(moveOrder);
                    }
                }
            }
        }

        Building PickDeposit()
        {
            var deposits = Unit.Army.Buildings.Where(b => b.Army == Unit.Army
                && b.isResourceDeposit && !b.UnderConstruction && !b.Dying);
            if (deposits.Count() == 0) return null;

            return deposits.SelectMin(d => Vector2.Distance(d.Coords, Unit.Coords));
        }

        protected override string SpecificsToStringCore()
        {
            return string.Format("Resource: {0}, Deposit: {1}",
                Resource != null ? Resource.ToString() : "NONE",
                Deposit != null ? Deposit.ToString() : "NONE");
        }

        enum HarvestMode
        {
            Collect,
            Deposit
        }
    }
}
