using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MechWars.MapElements
{
    public class Resource : MapElement
    {
        public int value;
        int startValue;

        public float Size { get { return 1.0f * value / startValue; } }
        public override float? LifeValue { get { return value; } }

        public Resource()
        {
            selectable = true;
        }

        protected override void OnStart()
        {

            base.OnStart();
            if (isShadow) return;

            startValue = value;
            if (startValue == 0) startValue = 1;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isShadow) return;

            var scale = Mathf.Pow(Size, 1 / 3f);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        public override StringBuilder TEMP_PrintStatus()
        {
            return base.TEMP_PrintStatus().AppendLine()
                .Append(string.Format("Resources: {0} / {1} ({2:P1})", value, startValue, (float)value / startValue));
        }
    }
}