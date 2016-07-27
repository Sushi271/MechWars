using MechWars.FogOfWar;
using System.Linq;
using System.Text;
using UnityEngine;

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
        }

        protected override Sprite GetMarkerImage()
        {
            return Globals.Textures.resourceMarker;
        }

        protected override float GetMarkerHeight()
        {
            return 2;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            var scale = Mathf.Pow(Size, 1 / 3f);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        protected override void UpdateArmiesQuadTrees()
        {
            foreach (var a in Globals.Armies)
            {
                var visible = AllCoords.Any(c => a.VisibilityTable[c.X, c.Y] == Visibility.Visible);
                if (visible != VisibleToArmies[a])
                {
                    VisibleToArmies[a] = visible;
                    if (visible) a.ResourcesQuadTree.Insert(this);
                    else a.ResourcesQuadTree.Remove(this);
                }
            }
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