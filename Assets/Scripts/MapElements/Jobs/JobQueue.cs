using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.MapElements.Jobs
{
    public class JobQueue
    {
        List<Job> jobs;

        public MapElement Owner { get; private set; }

        public int Count { get { return jobs.Count; } }
        public bool Empty { get { return Count == 0; } }

        public Job CurrentJob { get { return Count == 0 ? null : jobs.First(); } }
        public Job this[int i] { get { return jobs[i]; } }

        public bool Locked { get { return Owner.Dying; } }

        public JobQueue(MapElement owner)
        {
            jobs = new List<Job>();

            Owner = owner;
        }

        public void Add(Job job)
        {
            AssertNotLocked();
            jobs.Add(job);
        }

        public void Clear()
        {
            AssertNotLocked();
            jobs.RemoveAll(j => !j.Started);
        }

        public void TotalClear()
        {
            AssertNotLocked();
            jobs.Clear();
        }

        public void Update()
        {
            if (CurrentJob == null) return;

            CurrentJob.Update();
            if (CurrentJob.Done)
                jobs.RemoveFirst();
        }

        void AssertNotLocked()
        {
            if (Locked)
                throw new System.Exception(string.Format(
                    "JobQueue of MapElement {0} cannot be modified anymore - it's locked.", Owner));
        }
    }
}
