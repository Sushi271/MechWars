using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.AI.Agents
{
    public class ConstructionAgent : Agent
    {
        List<Request> requests;

        public ConstructionAgent(AIBrain brain, MainAgent parent)
            : base("Construction", brain, parent)
        {
            requests = new List<Request>();
        }

        protected override void OnUpdate()
        {
            ProcessMessages();
            ProcessRequests();
        }

        void ProcessMessages()
        {
            Message message;
            while ((message = ReceiveMessage()) != null)
            {
                if (message.Name == AIName.ConstructMeBuilding)
                {
                    SendMessage(message.Sender, AIName.Ok, message);
                    requests.Add(new Request(message.Sender, message.Name, int.Parse(message.Arguments[0]), message));
                }
            }
            requests.Sort((r1, r2) => r1.Priority.CompareTo(r2.Priority));
        }

        void ProcessRequests()
        {
            var processed = new List<Request>();
            foreach (var r in requests)
            {
                if (r.Name == AIName.ConstructMeBuilding)
                {
                    var buildingName = r.InnerMessage.Arguments[1];
                    var buildingKind = Knowledge.MapElementKinds[buildingName];
                    var creationMethod = Knowledge.CreationMethods[buildingKind];
                    var creatorKind = creationMethod.Creator;
                    var cost = creationMethod.Cost;
                    var time = creationMethod.Time;
                    // requirements are none, so we can ignore them

                    var creators = Army.Buildings.Where(b => b.mapElementName == creatorKind.Name);
                    if (creators.Empty())
                        continue;
                    var creator = creators.SelectMin(c => c.OrderQueue.OrderCount);
                    var orderAction = creator.orderActions.OfType<BuildingConstructionOrderAction>().FirstOrDefault(
                        oa => oa.Building.mapElementName == buildingKind.Name);
                    if (Army.resources < orderAction.StartCost)
                        continue;

                    var place = PickBuildingPlace(buildingKind);
                    if (place.CannotBuild)
                    {
                        // send message to someone: "Halp, anybody, what to do? IDK O.o" or sth like that
                    }

                    orderAction.GiveOrder(creator, new AIOrderActionArgs(Brain.player, place));

                    processed.Add(r);
                }
            }
            foreach (var r in processed)
                requests.Remove(r);
        }

        AIBuildingPlacement PickBuildingPlace(MapElementKind kind)
        {
            Vector2? placement = null;

            var placements = GetAvailablePlacements(kind);
            if (placements.Count > 0)
            {
                bool closerToResources = kind == Knowledge.MapElementKinds[AIName.Refinery];

                if (closerToResources)
                {
                    var minDist = float.MaxValue;
                    var resRegions = Knowledge.Resources.Regions;
                    if (resRegions.Empty())
                        placement = placements.First();
                    foreach (var r in resRegions)
                        foreach (var p in placements)
                        {
                            float dist;
                            r.ConvexHull.GetPointClosestTo(p, out dist);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                placement = p;
                            }
                        }
                }
                else placement = placements.First();
            }

            if (placement == null)
                return new AIBuildingPlacement(true);
            else return new AIBuildingPlacement(placement.Value);
        }

        List<Vector2> GetAvailablePlacements(MapElementKind kind)
        {
            var shapeW = kind.Shape.Width;
            var shapeH = kind.Shape.Height;
            var hRad = shapeW + 1;
            var vRad = shapeH + 1;

            var region = Knowledge.MyBase.BaseRegion.Region;
            var left = region.Left - hRad;
            var right = region.Right + hRad;
            var bottom = region.CalculateVerticalStart() - vRad;
            var top = region.CalculateVerticalEnd() + vRad;

            var toIdx = new System.Func<IVector2, IVector2>(v => v - new IVector2(left, bottom));
            var toTile = new System.Func<IVector2, IVector2>(v => v + new IVector2(left, bottom));
            var len = new System.Func<IVector2, int>(v => Mathf.Max(Mathf.Abs(v.X), Mathf.Abs(v.Y)));

            var w = right - left + 1;
            var h = top - bottom + 1;
            bool?[,] possibleTiles = new bool?[w, h];

            foreach (var tile in region.AllTiles)
                for (int x = tile.X - hRad; x <= tile.X + hRad; x++)
                    for (int y = tile.Y - vRad; y <= tile.Y + vRad; y++)
                    {
                        var v = new IVector2(x, y);
                        var i = toIdx(v);
                        if (len(tile - v) <= 1)
                            possibleTiles[i.X, i.Y] = false;
                        else if (possibleTiles[i.X, i.Y] == null)
                            possibleTiles[i.X, i.Y] = true;
                    }
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                {
                    var tile = toTile(new IVector2(i, j));
                    if (possibleTiles[i, j] == null || !Globals.Map.IsInBounds(tile) || Globals.Map[tile] != null)
                        possibleTiles[i, j] = false;
                }

            var placements = new List<Vector2>();
            for (int i = 0; i <= w - shapeW; i++)
                for (int j = 0; j <= h - shapeH; j++)
                {
                    var v = toTile(new IVector2(i, j));

                    bool allFree = true;
                    for (int si = 0; si < shapeW; si++)
                    {
                        for (int sj = 0; sj < shapeH; sj++)
                        {
                            if (!kind.Shape[si, sj]) continue;
                            allFree = possibleTiles[i + si, j + sj].Value;
                            if (!allFree) break;
                        }
                        if (!allFree) break;
                    }
                    if (!allFree) continue;

                    placements.Add(v + new Vector2(kind.Shape.DeltaXPos, kind.Shape.DeltaYPos));
                }

            return placements;
        }
    }
}