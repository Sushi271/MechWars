using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class SingleAttackOrder : Order
    {
        public override string Name { get { return "SingleAttack"; } }

        public MapElement Target { get; private set; }
        public Vector3 Aim { get; set; }

        public SingleAttackOrder(MapElement mapElement, MapElement target, Vector3 aim)
            : base(mapElement)
        {
            Target = target;
            Aim = aim;
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertMapElementIsNotDying(Target));
            TryFail(OrderResultAsserts.AssertMapElementCanBeAttacked(Target));
            TryFail(OrderResultAsserts.AssertMapElementHasReadiedAttack(MapElement));
            TryFail(OrderResultAsserts.AssertAimInAttackRange(MapElement, Aim));
            if (Failed) return;

            MapElement.MakeAttack(Target, Aim, () => attackFinished = true);
        }

        bool attackFinished;

        protected override void OnUpdate()
        {
            if (attackFinished)
                Succeed();
        }

        protected override string SpecificsToString()
        {
            return Target.ToString();
        }
    }
}
