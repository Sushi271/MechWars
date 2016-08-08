using MechWars.FogOfWar;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Resource : MapElement
    {
        public int value;
        public int MaxValue { get; private set; }

        public float Size { get { return 1.0f * value / MaxValue; } }
        public override float? LifeValue { get { return value; } }

        public override bool Selectable { get { return true; } }

        ResourceGhostSnapshot resourceGhostSnapshot;

        protected override void MakeSnapshotOf(MapElement originalMapElement)
        {
            base.MakeSnapshotOf(originalMapElement);
            resourceGhostSnapshot = new ResourceGhostSnapshot((Resource)originalMapElement, this);
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (IsGhost)
                ForceDifferentMaxValue(resourceGhostSnapshot.MaxValue);
            else MaxValue = value;

            if (MaxValue == 0) MaxValue = 1;
        }

        void ForceDifferentMaxValue(int maxValue)
        {
            if (maxValue < value)
                maxValue = value;
            this.MaxValue = maxValue;
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
                var visible = Globals.Map[this].Any(c => a.VisibilityTable[c.X, c.Y] == Visibility.Visible);
                if (visible != VisibleToArmies[a])
                {
                    VisibleToArmies[a] = visible;
                    if (visible) a.ResourcesQuadTree.Insert(this);
                    else a.ResourcesQuadTree.Remove(this);
                }
            }
        }

        protected override void AddGhostToQuadTree()
        {
            base.AddGhostToQuadTree();
            ObservingArmy.ResourcesQuadTree.Insert(this);
        }

        protected override void RemoveGhostFromQuadTree()
        {
            base.RemoveGhostFromQuadTree();
            ObservingArmy.ResourcesQuadTree.Remove(this);
        }

        protected override void RemoveFromQuadTrees()
        {
            var coordsList = Globals.Map[this];
            foreach (var a in Globals.Armies)
                if (VisibleToArmies[a])
                    a.ResourcesQuadTree.Remove(this);
        }

        public override StringBuilder DEBUG_PrintStatus(StringBuilder sb)
        {
            if (Dying) return sb;
            base.DEBUG_PrintStatus(sb)
                .AppendLine()
                .Append(string.Format("Resources: {0} / {1} ({2:P1})", value, MaxValue, (float)value / MaxValue));
            return sb;
        }
    }
}