using MechWars.MapElements.Attacks;
using UnityEngine;

namespace MechWars.MapElements.Jobs
{
    public class AttackJob : Job
    {
        AttackAnimation attackAnimation;

        public Attack Attack { get; private set; }

        public MapElement Attacker { get; private set; }
        public MapElement Target { get; private set; }

        public Vector2 Aim { get; private set; }
        public Vector2 Direction { get; protected set; }

        public AttackJob(Attack attack, MapElement attacker, MapElement target, Vector2 aim)
            : base(attacker)
        {
            Attack = attack;

            Attacker = attacker;
            Target = target;

            Aim = aim;
            Direction = attack.GetDirection(attacker, target, aim);

            attackAnimation = new AttackAnimation(attack);
            attackAnimation.Execute += AttackAnimation_Execute;
        }

        protected override void OnUpdate()
        {
            if (!attackAnimation.Playing && !attackAnimation.Finished)
                attackAnimation.Play();
            attackAnimation.Update();
            if (attackAnimation.Finished)
            {
                attackAnimation.Execute -= AttackAnimation_Execute;
                SetDone();
            }
        }

        private void AttackAnimation_Execute()
        {
            Attack.Execute(Attacker, Target, Aim);
        }
    }
}
