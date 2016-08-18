namespace MechWars.AI
{
    public class MapElementPurpose
    {
        public static MapElementPurpose Scouting(float value) { return new MapElementPurpose("Scouting", value); }
        public static MapElementPurpose Harvesting(float value) { return new MapElementPurpose("Harvesting", value); }

        public string Name { get; private set; }
        public float Value { get; set; }

        public MapElementPurpose(string name, float value)
        {
            Name = name;
            Value = value;
        }
    }
}