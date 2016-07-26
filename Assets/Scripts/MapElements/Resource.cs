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

        public override bool Selectable { get { return true; } }

        protected override void OnStart()
        {
            base.OnStart();

            startValue = value;
            if (startValue == 0) startValue = 1;
            InitializeInQuadTree();
        }

        protected override Sprite GetMarkerImage()
        {
            return Globals.Textures.resourceMarker;
        }

        protected override float GetMarkerHeight()
        {
            return 2;
        }

        protected override void InitializeInQuadTree()
        {
            Globals.QuadTreeMap.ResourcesQuadTree.Insert(this);
        }

        protected override void FinalizeInQuadTree()
        {
            Globals.QuadTreeMap.ResourcesQuadTree.Remove(this);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            var scale = Mathf.Pow(Size, 1 / 3f);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        public override StringBuilder DEBUG_PrintStatus(StringBuilder sb)
        {
            base.DEBUG_PrintStatus(sb)
                .AppendLine()
                .Append(string.Format("Resources: {0} / {1} ({2:P1})", value, startValue, (float)value / startValue));
            return sb;
        }
    }
}