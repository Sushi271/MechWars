namespace MechWars.MapElements.Orders
{
    public abstract class Order
    {
        protected SuccessOrderResult SuccessResult { get { return new SuccessOrderResult(string.Format("{0} complete.", Name)); } }

        public MapElement MapElement { get; private set; }
        public OrderState State { get; private set; }
        public bool InFinalState { get { return State == OrderState.Finished || State == OrderState.Stopped || State == OrderState.Terminated; } }

        public OrderResult Result { get; private set; }

        public bool Failed { get { return Conclusive && !Result.Success; } }
        public bool Succeeded { get { return Conclusive && Result.Success; } }
        public bool Conclusive { get { return Result != null; } }

        public abstract string Name { get; }

        protected virtual bool CanStop { get { return false; } }
        protected virtual bool CanFinalizeStop { get { return true; } }
        protected virtual bool CanFinish { get { return true; } }

        public bool CanUpdate { get { return State == OrderState.Started || State == OrderState.Stopping; } }

        public Order(MapElement mapElement)
        {
            MapElement = mapElement;
            State = OrderState.BrandNew;
        }

        protected void Succeed()
        {
            TrySucceed(SuccessResult);
        }

        protected bool TrySucceed(SuccessOrderResult orderResult)
        {
            return TryResolve(orderResult);
        }

        protected bool TryFail(FailOrderResult orderResult)
        {
            return TryResolve(orderResult);
        }

        bool TryResolve(OrderResult orderResult)
        {
            if (orderResult == null || Conclusive)
                return false;

            Result = orderResult;
            return true;
        }

        public void Start()
        {
            if (State != OrderState.BrandNew) return;
            
            TryFail(OrderResultAsserts.AssertMapElementIsNotDying(MapElement));
            if (!Failed) OnStart();
            if (Failed) Terminate();
            else if (State != OrderState.Stopping) State = OrderState.Started;
        }

        protected virtual void OnStart()
        {
        }

        public void Update()
        {
            if (!CanUpdate) return;
            
            OnUpdate();
            UpdateCore();
            if (State == OrderState.Stopping)
                FinalizeStop();
            else if (Conclusive) Finish();
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void UpdateCore()
        {
        }

        public bool Stop()
        {
            if (!(State == OrderState.BrandNew || State == OrderState.Started && CanStop))
                return false;
            
            State = OrderState.Stopping;
            OnStopping();
            return true;
        }

        public bool CancelBrandNew()
        {
            if (State != OrderState.BrandNew) return false;

            Stop();
            FinalizeStop();
            return true;
        }

        protected virtual void OnStopping()
        {
        }

        void FinalizeStop()
        {
            if (State != OrderState.BrandNew && !CanFinalizeStop) return;

            State = OrderState.Stopped;
            OnStopped();
        }

        protected virtual void OnStopped()
        {
        }

        void Finish()
        {
            if (!CanFinish) return;

            State = OrderState.Finished;
            OnFinished();
        }

        protected virtual void OnFinished()
        {
        }

        public virtual void Terminate()
        {
            OnTerminating();
            State = OrderState.Terminated;
        }

        protected virtual void OnTerminating()
        {
        }

        protected virtual string SpecificsToString()
        {
            return string.Empty;
        }

        public override string ToString()
        {
            var specifics = SpecificsToString();
            return string.Format("{0}{1}", Name,
                specifics != string.Empty ?
                string.Format(" [ {0} ]", specifics) :
                string.Empty);
        }
    }
}
