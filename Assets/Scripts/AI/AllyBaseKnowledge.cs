using MechWars.AI.Agents;
using MechWars.AI.Regions;
using MechWars.Utils;
using System.Linq;

namespace MechWars.AI
{
    public class AllyBaseKnowledge
    {
        KnowledgeAgent knowledge;
        
        BuildingInfo[,] buildingInfos;
        public BaseRegionBatch BaseRegion { get; private set; }

        public BuildingInfo this[int x, int y]
        {
            get { return buildingInfos[x, y]; }
        }

        public BuildingInfo this[IVector2 tile]
        {
            get { return this[tile.X, tile.Y]; }
        }

        public AllyBaseKnowledge(KnowledgeAgent knowledge)
        {
            this.knowledge = knowledge;
            
            buildingInfos = new BuildingInfo[Globals.MapSettings.Size, Globals.MapSettings.Size];
            BaseRegion = new BaseRegionBatch(knowledge.Brain);

            Initialize();
        }

        void Initialize()
        {
            foreach (var b in knowledge.Army.Buildings)
                AddBuilding(new BuildingInfo(knowledge.MapProxy, b));
        }

        public void AddBuilding(BuildingInfo building)
        {
            if (building.AllCoords.Any(c => this[c] != null))
                throw new System.Exception("Cannot AddBuliding - at least one coord is not empty.");

            foreach (var c in building.AllCoords)
                buildingInfos[c.X, c.Y] = building;

            building.RegionBatch = BaseRegion;
            BaseRegion.Buildings.Add(building);
            foreach (var c in building.AllCoords)
                BaseRegion.Region.AddTile(c);
        }

        public void RemoveBuilding(BuildingInfo building)
        {
            if (building.AllCoords.All(c => this[c] == null))
                throw new System.Exception("Cannot RemoveBuliding - none of its coords contain it.");

            foreach (var c in building.AllCoords)
                buildingInfos[c.X, c.Y] = null;

            building.RegionBatch = null;
            BaseRegion.Buildings.Remove(building);
            foreach (var c in building.AllCoords)
                BaseRegion.Region.RemoveTile(c);
        }
    }
}