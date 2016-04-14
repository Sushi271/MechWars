using System;
using System.Collections.Generic;
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

        public Path FindPath(CoordPair start, CoordPair target)
        {
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

        CoordPair DesignateAlternateTarget()
        {
            throw new NotImplementedException();
        }
    }
}
