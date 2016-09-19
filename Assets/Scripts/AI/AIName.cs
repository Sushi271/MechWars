namespace MechWars.AI
{
    public static class AIName
    {
        // Message & Request
        public const string FindMeResources = "Find me Resources";
        public const string ConstructMeBuilding = "Construct me Building";
        public const string ProduceMeUnits = "Produce me Units";
        public const string HandMeOnUnits = "Hand me on Units";
        public const string HarvestMore = "Harvest more";
        public const string NoRefineriesAndNoResources = "No Refineries and no resources";
        public const string NoHarvestersAndNoResources = "No Harvesters and no resources";
        public const string Ok = "Ok";

        // MapElement: Building
        public const string ConstructionYard = "ConstructionYard";
        public const string Refinery = "Refinery";
        public const string Factory = "Factory";

        // MapElement: Unit
        public const string Scout = "Scout";
        public const string Harvester = "Harvester";

        // MapElementPurpose
        public const string Scouting = "Scouting";
        public const string Harvesting = "Harvesting";
        public const string ResourceDeposit = "ResourceDeposit";
    }
}