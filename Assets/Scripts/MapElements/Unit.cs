using MechWars.MapElements.Orders;
using MechWars.MapElements.Statistics;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        public bool canCollectResources;

        public SingleOrderExecutor OrderExecutor { get; private set; }
        public override bool Interactible { get { return true; } }

        public Unit()
        {
            selectable = true;
            OrderExecutor = new SingleOrderExecutor();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            OrderExecutor.Update();
        }

        public void MoveStepTo(int x, int y, out bool finished)
        {
            float dx = x - X;
            float dy = y - Y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            var speed = Stats[StatNames.Speed];
            if (speed == null)
                throw new System.Exception(string.Format("Missing {0} Stat in Unit's Stats! (Unit {1})",
                    StatNames.Speed, this));
            float deltaDist = speed.Value * Time.deltaTime;

            if (deltaDist >= dist)
            {
                X = x;
                Y = y;
                finished = true;
                return;
            }

            var dPos = new Vector3(dx, 0, dy).normalized * deltaDist;
            transform.position += dPos;
            finished = false;
        }

        public void GiveOrder(IOrder order)
        {
            if (order is Order<Unit> || order is Order<MapElement>)
                OrderExecutor.Give(order);
            else throw new System.Exception(string.Format(
                "Order {0} not suitable for MapElement {1}.", order, this));
        }

        protected override void OnLifeEnd()
        {
            base.OnLifeEnd();

            if (!Globals.Destroyed)
            {
                var costStat = Stats[StatNames.Cost];
                if (costStat == null || costStat.Value == 0) return;
                var cost = (int)costStat.Value;

                var value = new System.Random().Next(cost / 4, cost / 2);
                if (value == 0) value = 1;

                var noArmy = GameObject.FindGameObjectsWithTag(Tag.Army)
                    .Where(a => a.GetComponent<Army>() == null).FirstOrDefault();

                if (noArmy != null)
                {
                    var resPrefab = Globals.Prefabs.RandomResourcePrefab;
                    var gameObject = Instantiate(resPrefab);
                    gameObject.transform.parent = noArmy.transform;
                    gameObject.transform.position = transform.position;
                    gameObject.name = resPrefab.name;
                    var resource = gameObject.GetComponent<Resource>();
                    resource.value = value;
                }
            }
        }
    }
}