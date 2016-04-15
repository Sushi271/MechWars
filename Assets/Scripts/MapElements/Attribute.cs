namespace Assets.Scripts.MapElements
{
    public class Attribute
    {
        public string Name { get; set; }

        float value;
        public float Value
        {
            get { return value; }
            set
            {
                this.value = value;
                CorrectValue();
            }
        }

        bool limited;
        public bool Limited
        {
            get { return limited; }
            set
            {
                limited = value;
                CorrectValue();
            }
        }

        float maxValue;
        public float MaxValue
        {
            get { return maxValue; }
            set
            {
                if (value < 0) maxValue = 0;
                else maxValue = value;
                CorrectValue();
            }
        }

        public Attribute(string name)
        {
            Name = name;
        }

        void CorrectValue()
        {
            if (value < 0) value = 0;
            if (!limited) return;
            if (value > maxValue) value = maxValue;
        }

        public override string ToString()
        {
            return string.Format("Attribute: {0}, Value: {1}, MaxValue: {2}, Limited: {3}", Name, Value, MaxValue, Limited);
        }
    }
}
