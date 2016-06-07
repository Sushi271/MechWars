using System.Collections.Generic;
using System.Linq;
using MechWars.Utils;
using MechWars.MapElements;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;

namespace MechWars.Pathfinding
{
    public class AStarPathfinder : IPathfinder
    {
        CoordPair start;
        CoordPair target;

        Dictionary<CoordPair, AStarCoordPairNode> evaluated;
        Dictionary<CoordPair, AStarCoordPairNode> toEvaluate;
        BinaryHeap<float, AStarCoordPairNode> priorityQueue;


        public Path FindPath(CoordPair start, CoordPair target, MapElement orderedMapElement)
        {
            this.start = start;
            this.target = target;

            var reservation = Globals.FieldReservationMap[target.Vector];
            if (reservation != null && reservation != orderedMapElement)
                target = DesignateAlternateTarget(false);

            evaluated = new Dictionary<CoordPair, AStarCoordPairNode>();
            toEvaluate = new Dictionary<CoordPair, AStarCoordPairNode>();
            priorityQueue = new BinaryHeap<float, AStarCoordPairNode>();

            var node = new AStarCoordPairNode(start);
            node.UpdateTotalLength(HeuristicCostEstimate(start, target));
            AddNode(node);
            
            Path p = null;

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

            if (p == null)
            {
                var alternateTargetCoords = DesignateAlternateTarget();
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

        CoordPair DesignateAlternateTarget(bool baseOnGraph = true)
        {
            var evaluated2 = new HashSet<CoordPair>();
            var toEvaluate2 = new HashSet<CoordPair>();
            var priorityQueue2 = new BinaryHeap<float, CoordPairNode<float>>();
            
            bool found = false;
            float distToOriginal = 0;

            var node = new CoordPairNode<float>(target);
            priorityQueue2.Insert(node);
            toEvaluate2.Add(node.CoordPair);

            var closestGraphNodes = new HashSet<CoordPairNode<float>>();

            while (!priorityQueue2.Empty)
            {
                var current = priorityQueue2.Extract();

                if (found && current.Distance > distToOriginal) continue;

                CoordPairNode<float> alt;
                bool fieldAccessible;
                if (baseOnGraph)
                {
                    AStarCoordPairNode alt2;
                    fieldAccessible = evaluated.TryGetValue(current.CoordPair, out alt2); // if it's in the graph - it's accessible
                    alt = alt2;
                }
                else
                {
                    fieldAccessible = Globals.FieldReservationMap[current.CoordPair.Vector] == null;
                    alt = new CoordPairNode<float>(current.CoordPair) { Distance = CoordPair.Distance(start, current.CoordPair) };
                }

                if (fieldAccessible)
                {
                    if (!found)
                    {
                        found = true;
                        distToOriginal = current.Distance;
                    }
                    closestGraphNodes.Add(alt);
                }
                
                evaluated2.Add(current.CoordPair);
                toEvaluate2.Remove(current.CoordPair);
                foreach (var n in current.CoordPair.Neighbours)
                {
                    if (evaluated2.Contains(n) || toEvaluate2.Contains(n)) continue;

                    var newDistance = CoordPair.Distance(n, target);
                    if (found && newDistance > distToOriginal) continue;
                    
                    node = new CoordPairNode<float>(n);
                    node.Distance = newDistance;
                    priorityQueue2.Insert(node);
                    toEvaluate2.Add(n);
                }
            }

            if (!found)
                throw new System.Exception(string.Format(
                    "Cannot find alternate target for CoordPair {0}.", target.ToString()));

            var alternate = closestGraphNodes.SelectMin(n => n.Distance);
            return alternate.CoordPair;
        }
    }
}
