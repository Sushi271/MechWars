namespace MechWars.AI.Agents.Goals
{
    public abstract class Goal
    {
        public string Name { get; private set; }
        public Agent Agent { get; private set; }
        public GoalState State { get; private set; }
        public float Importance { get; set; }

        public bool InFinalState {  get { return State == GoalState.Finished || State == GoalState.Canceled; } }
        
        public Goal(string name, Agent agent)
        {
            Agent = agent;
            State = GoalState.BrandNew;
        }

        public void Start()
        {
            if (State != GoalState.BrandNew)
                throw new System.Exception("Start can only be called once, when State is BrandNew.");
            
            OnStart();
            State = GoalState.Started;
            OnStarted();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStarted()
        {
        }

        public void Update()
        {
            if (State != GoalState.Started)
                throw new System.Exception("Update can only be called after Start, when State is Started.");
            
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        public void Finish()
        {
            if (State != GoalState.Started)
                throw new System.Exception("Finish can only be called after Start, when State is Started.");

            OnFinishing();
            State = GoalState.Finished;
            OnFinished();
        }

        protected virtual void OnFinishing()
        {
        }

        protected virtual void OnFinished()
        {
        }

        public void Cancel()
        {
            if (State == GoalState.Finished)
                throw new System.Exception("Cancel cannot be called after Finish, when State is Finished.");

            OnCanceling();
            State = GoalState.Canceled;
            OnCanceled();
        }

        protected virtual void OnCanceling()
        {
        }

        protected virtual void OnCanceled()
        {
        }
    }
}