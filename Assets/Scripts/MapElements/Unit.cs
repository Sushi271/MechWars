using UnityEngine;
using System.Collections;
using MechWars.Orders;
using System.Xml;
using Assets.Scripts.MapElements;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {
        public Army army;

        public TextAsset statsFile;
        public Stats Stats { get; private set; }

        OrderExecutor orderExecutor;
        public IOrder CurrentOrder { get { return orderExecutor.CurrentOrder; } }

        public Unit()
        {
            selectable = true;
            Stats = new Stats();
            orderExecutor = new OrderExecutor(this);
        }

        protected override void OnStart()
        {
            base.OnStart();
            ReadStats();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            orderExecutor.Update();
        }

        public void MoveStepTo(int x, int y, out bool finished)
        {
            float dx = x - X;
            float dy = y - Y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            var speed = Stats[AttributeNames.Speed];
            if (speed == null)
                throw new System.Exception(string.Format("Missing {0} attribute in Unit's Stats! (Unit {1})",
                    AttributeNames.Speed, this));
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

        public void GiveOrder(Order order)
        {
            // TODO: provide control over order type
            orderExecutor.Give(order);
        }

        void ReadStats()
        {
            if (statsFile == null)
                Debug.LogError("Unit has no stats file.");

            var xml = new XmlDocument();
            xml.LoadXml(statsFile.text);
            var root = xml.DocumentElement;
            var nodes = root.SelectNodes("Stat");
            foreach (var n in nodes)
            {
                var e = n as XmlElement;
                if (e == null) continue;
                var name = e.GetAttribute("name");
                var value = float.Parse(e.GetAttribute("value"));
                var limited = e.HasAttribute("max_value");
                float maxValue = 0;
                if (limited) maxValue = float.Parse(e.GetAttribute("max_value"));
                Attribute attribute = new Attribute(name);
                attribute.MaxValue = maxValue;
                attribute.Value = value;
                attribute.Limited = limited;
                Stats.Add(attribute);
            }
        }
    }
}