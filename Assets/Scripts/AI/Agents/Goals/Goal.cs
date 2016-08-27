namespace MechWars.AI.Agents.Goals
{
    public abstract class Goal
    {
        public string Name { get; private set; }
        public Agent Agent { get; private set; }
        public GoalState State { get; private set; }

        public bool InFinalState {  get { return State == GoalState.Finished || State == GoalState.Canceled; } }

        public event System.Action<Goal> Starting;
        public event System.Action<Goal> Started;
        public event System.Action<Goal> Updating;
        public event System.Action<Goal> Updated;
        public event System.Action<Goal> Finishing;
        public event System.Action<Goal> Finished;
        public event System.Action<Goal> Canceling;
        public event System.Action<Goal> Canceled;

        public Goal(string name, Agent agent)
        {
            Agent = agent;
            State = GoalState.BrandNew;
        }

        public void Start()
        {
            if (State != GoalState.BrandNew)
                throw new System.Exception("Start can only be called once, when State is BrandNew.");

            if (Starting != null) Starting(this);
            OnStart();
            State = GoalState.Started;
            if (Started != null) Started(this);
        }

        protected virtual void OnStart()
        {
        }

        public void Update()
        {
            if (State != GoalState.Started)
                throw new System.Exception("Update can only be called after Start, when State is Started.");

            if (Updating != null) Updating(this);
            OnUpdate();
            if (Updated != null) Updated(this);
        }

        protected virtual void OnUpdate()
        {
        }

        protected void Finish()
        {
            if (State != GoalState.Started)
                throw new System.Exception("Finish can only be called after Start, when State is Started.");

            if (Finishing != null) Finishing(this);
            State = GoalState.Finished;
            if (Finished != null) Finished(this);
        }

        public void Cancel()
        {
            if (State != GoalState.Finished)
                throw new System.Exception("Cancel cannot be called after Finish, when State is Finished.");
            
            if (Canceling != null) Canceling(this);
            OnCancel();
            State = GoalState.Canceled;
            if (Canceled != null) Canceled(this);
        }

        protected virtual void OnCancel()
        {
        }
    }
}