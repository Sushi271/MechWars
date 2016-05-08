using MechWars.MapElements.Attacks;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Attacks
{
    public class AttackAnimation
    {
        bool executed;

        public event System.Action Execute;

        public float Duration { get; private set; }
        public float ExecuteTime { get; private set; }

        public bool Playing { get; private set; }
        public bool Finished { get; private set; }
        public float RunningTime { get; private set; }        
        
        public AttackAnimation(Attack attack)
        {
            Duration = attack.animationDuration;
            ExecuteTime = attack.animationExecuteTime;

            if (Duration < 0)
                Duration = 0;
            if (ExecuteTime < Duration)
                ExecuteTime = Duration;

        }

        public void Play()
        {
            if (!Playing && !Finished) Playing = true;
        }

        public void Update()
        {
            if (!Playing || Finished) return;

            RunningTime += Time.deltaTime;
            if (!executed && RunningTime >= ExecuteTime && Execute != null)
            {
                Execute();
                executed = true;
            }
            if (RunningTime >= Duration)
            {
                RunningTime = Duration;
                Playing = false;
                Finished = true;
            }
        }
    }
}
