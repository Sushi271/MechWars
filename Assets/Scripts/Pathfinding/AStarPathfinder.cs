using System.Collections.Generic;
using System.Linq;
using System.Timers;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.Pathfinding
{
    public class AStarPathfinder : IPathfinder
    {
        Dictionary<CoordPair, AStarCoordPairNode> evaluated;
        Dictionary<CoordPair, AStarCoordPairNode> toEvaluate;
        BinaryHeap<float, AStarCoordPairNode> priorityQueue;

        public Path FindPath(CoordPair start, CoordPair target, MechWars.MapElements.Unit unit)
        {
            //if (Globals.FieldReservationMap[target.Vector] != null)
            //    target = DesignateAlternateTarget(target);

            evaluated = new Dictionary<CoordPair, AStarCoordPairNode>();
            toEvaluate = new Dictionary<CoordPair, AStarCoordPairNode>();
            priorityQueue = new BinaryHeap<float, AStarCoordPairNode>();

            var node = new AStarCoordPairNode(start);
            node.UpdateTotalLength(HeuristicCostEstimate(start, target));
            AddNode(node);
            
            Path p = null;

            //DateTime dt = DateTime.Now;

            while (!priorityQueue.Empty)
            {
                var current = priorityQueue.Extract();

                if (current.CoordPair == target)
                {
                    p = ReconstructPath(current);
                    break;
                }
                evaluated[current.CoordPair] = current;
                toEvaluate.Remove(current.CoordPair);

                foreach (var n in current.CoordPair.Neighbours)
                {
                    if (evaluated.ContainsKey(n)) continue;
                    if (Globals.FieldReservationMap[n.Vector] != null) continue;

                    var deltaDist = CoordPair.Distance(current.CoordPair, n);
                    var newDistance = current.Distance + deltaDist;
                    
                    bool success = toEvaluate.TryGetValue(n, out node);
                    if (success)
                    {
                        if (newDistance >= node.Distance) continue;
                    }
                    else
                    {
                        node = new AStarCoordPairNode(n);
                        AddNode(node);
                    }

                    node.CameFrom = current;
                    node.Distance = newDistance;
                    node.UpdateTotalLength(HeuristicCostEstimate(n, target));
                    priorityQueue.Correct(node);
                }
            }
            
            //TimeSpan ts = DateTime.Now - dt;
            //Debug.Log(ts);

            if (p == null)
            {
                var time = System.DateTime.Now;
                var alternateTargetCoords = DesignateAlternateTarget(target);
                var alternateTarget = evaluated[alternateTargetCoords];
                p = ReconstructPath(alternateTarget);
            }
            return p;
        }

        void AddNode(AStarCoordPairNode node)
        {
            toEvaluate[node.CoordPair] = node;
            priorityQueue.Insert(node);
        }

        float HeuristicCostEstimate(CoordPair from, CoordPair to)
        {
            return CoordPair.Distance(from, to) * 5; // TODO: heuristic epsilon parameter
        }

        Path ReconstructPath(AStarCoordPairNode current)
        {
            var path = new Path();
            while (current != null)
            {
                path.Push(current.CoordPair);
                current = current.CameFrom;
            }
            return path;
        }

        CoordPair DesignateAlternateTarget(CoordPair originalTarget)
        {
            var evaluated2 = new HashSet<CoordPair>();
            var toEvaluate2 = new HashSet<CoordPair>();
            var priorityQueue2 = new BinaryHeap<float, CoordPairNode<float>>();
            
            bool found = false;
            float dist = 0;

            var node = new CoordPairNode<float>(originalTarget);
            priorityQueue2.Insert(node);
            toEvaluate2.Add(node.CoordPair);

            var closestNodes = new HashSet<AStarCoordPairNode>();

            while (!priorityQueue2.Empty)
            {
                var current = priorityQueue2.Extract();

                if (found && current.Distance > dist) continue;

                AStarCoordPairNode alt;
                bool success = evaluated.TryGetValue(current.CoordPair, out alt);
                if (success)
                {
                    if (!found)
                    {
                        found = true;
                        dist = alt.Distance;
                    }
                    closestNodes.Add(alt);
                }
                
                evaluated2.Add(current.CoordPair);
                toEvaluate2.Remove(current.CoordPair);
                foreach (var n in current.CoordPair.Neighbours)
                {
                    if (evaluated2.Contains(n) || toEvaluate2.Contains(n)) continue;

                    var newDistance = CoordPair.Distance(n, originalTarget);
                    if (found && newDistance > dist) continue;
                    
                    node = new CoordPairNode<float>(n);
                    node.Distance = newDistance;
                    priorityQueue2.Insert(node);
                    toEvaluate2.Add(node.CoordPair);
                }
            }

            if (!found)
                throw new System.Exception(string.Format(
                    "Cannot find alternate target for CoordPair {0}.", originalTarget.ToString()));

            var alternate = closestNodes.Aggregate((n1, n2) => n1.Distance < n2.Distance ? n1 : n2);
            return alternate.CoordPair;
        }
    }
}
