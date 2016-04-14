using System.Collections.Generic;

namespace MechWars.Pathfinding
{
    public class AStarCoordPairNode : CoordPairNode<float>
    {
        public override float Key { get { return TotalDistance; } }
        public float TotalDistance { get; private set; }

        public AStarCoordPairNode CameFrom { get; set; }

        public AStarCoordPairNode(CoordPair coordPair)
            : base(coordPair)
        {
        }

        public void UpdateTotalLength(float heuristicCostEstimate)
        {
            TotalDistance = Distance + heuristicCostEstimate;
        }
    }
}
