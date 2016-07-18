using MechWars.MapElements.Attacks;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class SingleAttackOrder : Order
    {
        AttackAnimation attackAnimation;

        public override string Name { get { return "SingleAttack"; } }

        public MapElement Target { get; private set; }
        public Attack Attack { get; private set; }
        public Vector3 Aim { get; set; }

        public SingleAttackOrder(MapElement mapElement, MapElement target, Attack attack, Vector3 aim)
            : base(mapElement)
        {
            Target = target;
            Attack = attack;
            Aim = aim;
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertMapElementIsNotDying(Target));
            TryFail(OrderResultAsserts.AssertMapElementCanBeAttacked(Target));
            TryFail(OrderResultAsserts.AssertMapElementHasAttack(MapElement, Attack));
            TryFail(OrderResultAsserts.AssertAimInAttackRange(MapElement, Aim));
            if (Failed) return;
            
            attackAnimation = new AttackAnimation(Attack);
            attackAnimation.Execute += () => Attack.Execute(MapElement, Target, Aim);
        }

        protected override void OnUpdate()
        {
            if (!attackAnimation.Playing && !attackAnimation.Finished)
                attackAnimation.Play();
            attackAnimation.Update();
            if (attackAnimation.Finished)
                Succeed();
        }

        protected override string SpecificsToString()
        {
            return Target.ToString();
        }
    }
}
