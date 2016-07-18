using MechWars.MapElements.Attacks;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.Orders
{
    public class AttackOrder : ComplexOrder
    {
        Attack attack;

        RotateOrder rotateOrder;
        SingleAttackOrder singleAttackOrder;

        float cooldown;

        public override string Name { get { return "Attack"; } }
        public MapElement Target { get; private set; }
        
        public AttackOrder(MapElement mapElement, MapElement target)
            : base(mapElement)
        {
            Target = target;
        }

        protected override void OnStart()
        {
            TryFail(OrderResultAsserts.AssertMapElementCanBeAttacked(Target));
            TryFail(OrderResultAsserts.AssertMapElementHasAnyAttacks(MapElement));
            if (Failed) return;

            if (Target.Dying) Succeed();
        }

        protected override void OnUpdate()
        {
            if (HasSubOrder || State == OrderState.Stopping) return;

            if (!Target.Dying)
            {
                if (attack == null)
                    attack = MapElement.PickAttack();
                if (rotateOrder == null)
                {
                    rotateOrder = new RotateOrder(MapElement, true);
                    GiveSubOrder(rotateOrder);
                }
                if (cooldown > 0)
                {
                    cooldown -= Time.deltaTime;
                    cooldown = Mathf.Clamp(cooldown, 0, cooldown);
                }
            }
            else Succeed();
        }

        protected override void OnSubOrderStarting()
        {
            if (Target.Dying)
                SubOrder.CancelBrandNew();
            else if (SubOrder == rotateOrder)
                UpdateRotateOrdersProperties();
        }

        protected override void OnSubOrderUpdating()
        {
            if (Target.Dying)
            {
                if (SubOrder == rotateOrder)
                    SubOrder.Stop();
            }
            else if (SubOrder == rotateOrder)
                UpdateRotateOrdersProperties();
        }

        protected override void OnSubOrderFinished()
        {
            if (SubOrder == rotateOrder)
            {
                rotateOrder = null;
                if (cooldown == 0)
                {
                    var aim = Target.GetClosestAimTo(MapElement.Coords);
                    singleAttackOrder = new SingleAttackOrder(MapElement, Target, attack, aim);
                    GiveSubOrder(singleAttackOrder);
                }
            }
            else if (SubOrder == singleAttackOrder)
            {
                singleAttackOrder = null;
                attack = null;
                var attackSpeedStat = MapElement.Stats[StatNames.AttackSpeed];
                if (attackSpeedStat == null)
                    cooldown = 1;
                else cooldown = 1 / attackSpeedStat.Value;
            }
        }

        void UpdateRotateOrdersProperties()
        {
            if (rotateOrder != null)
            {
                var aim = Target.GetClosestAimTo(MapElement.Coords);
                var direction = attack.GetDirection(MapElement, Target, aim);
                var angle = UnityExtensions.AngleFromToXZ(Vector2.up, direction);
                rotateOrder.TargetRotation = angle;

                if (attack.PitchAdjustable)
                {
                    var headPitch = attack.GetHeadPitch(MapElement, Target, aim);
                    rotateOrder.RotatesVertically = true;
                    rotateOrder.TargetHeadPitch = headPitch;
                }
            }
        }

        protected override string SpecificsToStringCore()
        {
            return Target.ToString();
        }
    }
}
