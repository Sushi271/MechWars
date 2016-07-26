using System.Linq;

namespace MechWars.MapElements.Statistics
{
    public class Stat
    {
        public string Name { get; private set; }
        public MapElement Owner { get; private set; }

        float value;
        public float Value
        {
            get
            {
                var val = value;
                if (limited || Owner.Army == null) return value;

                var bonuses =
                    from b in Owner.Army.Technologies.GetBonusesFor(Owner)
                    where b.statName == Name
                    orderby b.order, b.type
                    select b;
                foreach (var b in bonuses)
                    val = b.ApplyTo(val);
                return val;
            }
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
            get
            {
                var val = maxValue;
                if (!limited || Owner.Army == null) return maxValue;

                var bonuses =
                    from b in Owner.Army.Technologies.GetBonusesFor(Owner)
                    where b.statName == Name
                    orderby b.order, b.type
                    select b;
                foreach (var b in bonuses)
                    val = b.ApplyTo(val);
                return val;
            }
            set
            {
                if (value < 0) maxValue = 0;
                else maxValue = value;
                CorrectValue();
            }
        }

        public bool HasMaxValue {  get { return Limited && Value == MaxValue; } }

        public Stat(string name, MapElement owner)
        {
            Name = name;
            Owner = owner;
        }

        void CorrectValue()
        {
            if (value < 0) value = 0;
            if (!limited) return;
            if (value > MaxValue) value = MaxValue;
        }

        public override string ToString()
        {
            return string.Format("Attribute: {0}, Value: {1}, MaxValue: {2}, Limited: {3}", Name, Value, MaxValue, Limited);
        }
    }
}
