using System.Text;

namespace MechWars.MapElements.Orders
{
    public abstract class ComplexOrder : Order
    {
        public Order NextSubOrder { get; private set; }
        public Order SubOrder { get; private set; }
        public bool HasSubOrder { get { return SubOrder != null; } }

        protected override bool CanStop { get { return true; } }
        protected override bool CanFinalizeStop { get { return !HasSubOrder; } }
        protected override bool CanFinish { get { return !HasSubOrder; } }

        public ComplexOrder(MapElement mapElement)
            : base(mapElement)
        {
        }

        protected void GiveSubOrder(Order subOrder)
        {
            if (subOrder.State != OrderState.BrandNew)
                throw new System.ArgumentException("State property of Order subOrder must be BrandNew.");
            NextSubOrder = subOrder;
            if (SubOrder == null)
                AdvanceSubOrders();
        }

        void AdvanceSubOrders()
        {
            SubOrder = NextSubOrder;
            NextSubOrder = null;
        }

        protected sealed override void UpdateCore()
        {
            if (!HasSubOrder) return;

            if (SubOrder.State == OrderState.BrandNew)
            {
                OnSubOrderStarting();
                SubOrder.Start();
                if (SubOrder.State == OrderState.Started)
                    OnSubOrderStarted();
            }
            if (SubOrder.State == OrderState.Started || SubOrder.State == OrderState.Stopping)
            {
                OnSubOrderUpdating();
                SubOrder.Update();
                OnSubOrderUpdated();
            }
            if (SubOrder.State == OrderState.Finished)
                SubOrderFinished();
            else if (SubOrder.State == OrderState.Stopped)
                SubOrderStopped();
            else if (SubOrder.State == OrderState.Terminated)
                SubOrderTerminated();
        }

        void SubOrderFinished()
        {
            OnSubOrderFinished();
            AdvanceSubOrders();
        }

        void SubOrderStopped()
        {
            OnSubOrderStopped();
            AdvanceSubOrders();
        }

        void SubOrderTerminated()
        {
            OnSubOrderTerminated();
            AdvanceSubOrders();
        }

        protected virtual void OnSubOrderStarting()
        {
        }

        protected virtual void OnSubOrderStarted()
        {
        }

        protected virtual void OnSubOrderUpdating()
        {
        }

        protected virtual void OnSubOrderUpdated()
        {
        }

        protected virtual void OnSubOrderFinished()
        {
        }

        protected virtual void OnSubOrderStopped()
        {
        }

        protected virtual void OnSubOrderTerminated()
        {
        }

        protected sealed override void OnStopping()
        {
            if (NextSubOrder != null)
                NextSubOrder.CancelBrandNew();

            if (SubOrder != null)
            {
                if (SubOrder.State == OrderState.BrandNew)
                    CancelBrandNew();
                else if (SubOrder.State == OrderState.Started)
                    SubOrder.Stop();
            }
        }

        protected sealed override string SpecificsToString()
        {
            var specifics = SpecificsToStringCore();
            if (specifics == null || specifics == string.Empty)
                specifics = null;

            var sb = new StringBuilder();
            if (specifics != null)
                sb.Append(specifics);

            if (Globals.Instance.debugComplexOrderStrings)
            {
                if (specifics != null)
                    sb.Append(", ");
                sb.Append("SubOrder = ");
                sb.Append(HasSubOrder ? SubOrder.ToString() : "NONE");
            }
            return sb.ToString();
        }

        protected virtual string SpecificsToStringCore()
        {
            return null;
        }
    }
}
