using MechWars.FogOfWar;
using MechWars.MapElements.Orders;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Unit : MapElement
    {

        protected override bool CanAddToArmy { get { return true; } }
        protected override bool CreateVisibilityGhost { get { return false; } }
        public override bool Selectable { get { return true; } }
        public override bool CanBeAttacked { get { return true; } }
        public override bool CanBeEscorted { get { return true; } }
        public override bool CanRotateItself { get { return true; } }

        public Move Move { get; private set; }

        protected override OrderQueue CreateOrderQueue(bool enabled = true)
        {
            var orderExecutor = base.CreateOrderQueue();
            orderExecutor.GiveReplaces = true;
            return orderExecutor;
        }

        protected override Sprite GetMarkerImage()
        {
            if (Army != null)
                return Army.unitMarker;
            return Globals.Textures.neutralUnitMarker;
        }

        protected override float GetMarkerHeight()
        {
            return 3;
        }

        protected override void InitializeInVisibilityTable()
        {
            if (Army != null)
                Army.VisibilityTable.IncreaseVisibility(this);
        }

        protected override void FinalizeInVisibilityTable()
        {
            if (Army != null)
                Army.VisibilityTable.DecreaseVisibility(this);
        }

        public bool SetMove(SingleMoveOrder singleMoveOrder)
        {
            if (Move != null || singleMoveOrder.State != OrderState.BrandNew)
                return false;
            Move = new Move(singleMoveOrder);
            return true;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Move != null)
            {
                Move.Update();
                if (Move.Done)
                    Move = null;
            }
        }

        protected override void UpdateArmiesQuadTrees()
        {
            foreach (var a in Globals.Armies)
            {
                if (a == Army) continue;

                var visible = AllCoords.Any(c => a.VisibilityTable[c.X, c.Y] == Visibility.Visible);
                if (visible != VisibleToArmies[a])
                {
                    VisibleToArmies[a] = visible;
                    if (visible) a.EnemiesQuadTree.Insert(this);
                    else a.EnemiesQuadTree.Remove(this);
                }
            }
        }

        public override StringBuilder DEBUG_PrintStatus(StringBuilder sb)
        {
            base.DEBUG_PrintStatus(sb)
                .AppendLine();
            DEBUG_PrintOrders(sb);
            return sb;
        }
    }
}