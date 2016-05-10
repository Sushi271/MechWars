using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class HarvestOrder : Order<Unit>
    {
        MoveOrder move;
        CollectOrder collect;
        DepositOrder deposit;

        HarvestMode mode;

        bool reloadMove;

        public Unit Unit { get; private set; }
        public Resource Resource { get; private set; }
        public Building Refinery { get; private set; }

        public bool TankFull
        {
            get
            {
                var carriedResourceStat = Unit.Stats[StatNames.CarriedResource];
                if (carriedResourceStat == null)
                    throw new System.Exception(string.Format(
                        "Unit {0} doesn't have Stat {1}.", StatNames.CarriedResource));
                return carriedResourceStat.Value == carriedResourceStat.MaxValue;
            }
        }

        public bool TankEmpty
        {
            get
            {
                var carriedResourceStat = Unit.Stats[StatNames.CarriedResource];
                if (carriedResourceStat == null)
                    throw new System.Exception(string.Format(
                        "Unit {0} doesn't have Stat {1}.", StatNames.CarriedResource));
                return carriedResourceStat.Value == 0;
            }
        }

        public HarvestOrder(Unit orderedUnit, Resource resource)
            : base("Harvest", orderedUnit)
        {
            Unit = (Unit)MapElement;
            Resource = resource;
            mode = HarvestMode.Collect;
        }

        public HarvestOrder(Unit orderedUnit, Building refinery)
            : base("Harvest", orderedUnit)
        {
            Unit = (Unit)MapElement;
            Refinery = refinery;
            mode = HarvestMode.Deposit;
        }

        protected override bool RegularUpdate()
        {
            if (!Stopping && !Stopped)
            {
                bool decided = DecideMode();
                if (!decided) Stop();
                else ExecuteMode();
                return false;
            }
            return true;
        }

        protected override bool StoppingUpdate()
        {
            if (move == null) return true;
            if (move.SingleMoveInProgress)
            {
                move.Update();
                if (move.Stopped) return true;
            }
            else if (Resource == null) return true;
            else if (mode == HarvestMode.Collect)
            {
                if (collect == null) return true;
                else if (!collect.InRange) return true;
                else
                {
                    collect.Update();
                    if (collect.Stopped) return true;
                }
            }
            else if (mode == HarvestMode.Deposit)
            {
                if (deposit == null) return true;
                else if (!deposit.InRange) return true;
                else
                {
                    deposit.Update();
                    if (deposit.Stopped) return true;
                }
            }
            return false;
        }

        protected override void TerminateCore()
        {
        }

        bool DecideMode()
        {
            if (mode == HarvestMode.Collect)
            {
                if (TankFull || Resource == null && PickResource() == null)
                {
                    mode = HarvestMode.Deposit;
                    if (PickRefinery() == null) return false;
                }
            }
            else if (mode == HarvestMode.Deposit)
            {
                if (TankEmpty)
                {
                    mode = HarvestMode.Collect;
                    if (Resource == null && PickResource(true) == null) return false;
                }
            }
            return true;
        }

        Resource PickResource(bool log = false)
        {
            var resources = Globals.MapElementsDatabase.Resources.Where(r => r.Alive && r.value > 0);
            if (resources.Count() == 0)
            {
                if (log)
                    Debug.Log(Unit + ": No more resources!"); // TODO: play message "No more resources!"
                return null;
            }
            Resource = resources.SelectMin(r => Vector2.Distance(r.Coords, Unit.Coords));
            if (collect != null)
                collect.Resource = Resource;
            reloadMove = true;
            return Resource;
        }

        Building PickRefinery()
        {
            var buildings = Globals.MapElementsDatabase.Buildings;
            var refineries = buildings.Where(b => b.army == Unit.army && b.isResourceDeposit && !b.UnderConstruction);
            if (refineries.Count() == 0)
            {
                Debug.Log(Unit + ": No refineries!"); // TODO: play message "No refineries!"
                return null;
            }
            Refinery = refineries.SelectMin(r => Vector2.Distance(r.Coords, Unit.Coords));
            if (deposit != null)
                deposit.Refinery = Refinery;
            reloadMove = true;
            return Refinery;
        }

        void ExecuteMode()
        {
            if (mode == HarvestMode.Collect)
            {
                if (move != null && move.SingleMoveInProgress)
                    move.Update();
                else
                {
                    if (Resource != null)
                    {
                        if (collect == null)
                            collect = new CollectOrder(Unit, Resource);
                        if (!collect.InRange)
                        {
                            if (move == null || reloadMove)
                            {
                                move = new MoveOrder(Unit, Resource.Coords.Round());
                                reloadMove = false;
                            }
                            move.Update();
                        }
                        else
                        {
                            move = null;
                            collect.Update();
                            if (collect.Stopped)
                            {
                                if (!Resource.Alive || Resource.value == 0)
                                    Resource = null;
                                collect = null;
                            }
                        }
                    }
                }
            }
            else if (mode == HarvestMode.Deposit)
            {
                if (deposit == null)
                    deposit = new DepositOrder(Unit, Refinery);

                if (move != null && move.SingleMoveInProgress)
                    move.Update();
                else
                {
                    if (!deposit.InRange)
                    {
                        if (move == null || reloadMove)
                        {
                            move = new MoveOrder(Unit, Refinery.Coords.Round());
                            reloadMove = false;
                        }
                        move.Update();
                    }
                    else
                    {
                        move = null;
                        deposit.Update();
                        if (deposit.Stopped)
                        {
                            Refinery = null;
                            deposit = null;
                        }
                    }
                }
            }
        }

        void OnSingleMoveFinished()
        {
            if (mode == HarvestMode.Collect)
                move.Destination = Resource.Coords.Round();
            else if (mode == HarvestMode.Deposit)
                move.Destination = Refinery.Coords.Round();
        }

        protected override void OnStopCalled()
        {
            if (move != null) move.Stop();
            if (collect != null) collect.Stop();
            if (deposit != null) deposit.Stop();
        }


        protected override string SpecificsToString()
        {
            return string.Format("{0}", 
                mode == HarvestMode.Collect ?
                (MapElement)Resource : Refinery);
        }

        enum HarvestMode
        {
            Collect,
            Deposit
        }
    }
}
