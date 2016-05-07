namespace MechWars.MapElements.Jobs
{
    public abstract class Job
    {
        public MapElement MapElement { get; private set; }
        public bool Started { get; private set; }
        public bool Done { get; private set; }

        public Job(MapElement mapElement)
        {
            MapElement = mapElement;
            Started = false;
            Done = false;
        }

        public void Update()
        {
            Started = true;
            OnUpdate();
        }

        protected abstract void OnUpdate();

        protected void SetDone()
        {
            Done = true;
        }
    }
}
