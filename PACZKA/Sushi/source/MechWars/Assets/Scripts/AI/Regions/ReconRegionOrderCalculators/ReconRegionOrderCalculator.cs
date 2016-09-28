using MechWars.AI.Agents.Goals;
using UnityEngine;

namespace MechWars.AI.Regions.ReconRegionOrderCalculators
{
    public abstract class ReconRegionOrderCalculator : MonoBehaviour
    {
        public abstract float Calculate(CoarseReconGoal goal, ReconRegionBatch region);
    }
}