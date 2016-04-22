using UnityEngine;

namespace MechWars.MapElements
{
    public class Resource : MapElement
    {
        public override bool Interactible { get { return true; } }

        public int value;
        int startValue;

        public float Size {  get { return (float)value / startValue; } }

        public Resource()
        {
            selectable = true;
        }
        
        protected override void OnStart()
        {
            base.OnStart();

            startValue = value;
            if (startValue == 0) startValue = 1;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            var scale = Mathf.Pow(Size, 1 / 3f);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        protected override void UpdateAlive()
        {
            if (Alive && value == 0) Alive = false;
        }
    }
}