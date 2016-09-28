using System.Linq;
using UnityEngine;

namespace MechWars.MapElements.Statistics
{
    public class Stat
    {
        public string Name { get; private set; }
        public MapElement Owner { get; private set; }
        
        int lastUpdate = -1;
        
        float baseMaxValue;

        float value;
        public float Value
        {
            get
            {
                if (lastUpdate == Time.frameCount) return value;
                lastUpdate = Time.frameCount;
                UpdateValues();
                return value;
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
                if (lastUpdate == Time.frameCount) return maxValue;
                lastUpdate = Time.frameCount;
                UpdateValues();
                return maxValue;
            }
        }

        public bool HasMaxValue {  get { return Limited && Value == MaxValue; } }

        public Stat(string name, MapElement owner, float baseMaxValue, float startValue, bool limited)
        {
            Name = name;
            Owner = owner;

            this.baseMaxValue = baseMaxValue;

            maxValue = baseMaxValue;
            value = startValue;
            this.limited = limited;

            CorrectValue();
        }

        void CorrectValue()
        {
            if (value < 0) value = 0;
            if (!limited) return;
            if (value > maxValue) value = maxValue;
        }

        void UpdateValues()
        {
            if (Owner.Army == null) return;

            var set = Owner.Army.Technologies.StatChangesTable[Owner.mapElementName, Name];
            if (set.Contains(this)) return;
            set.Add(this);

            var valueShortage = maxValue - value;
            maxValue = baseMaxValue;

            var bonuses =
                from b in Owner.Army.Technologies.GetBonusesFor(Owner)
                where b.statName == Name
                orderby b.order, b.type
                select b;
            foreach (var b in bonuses)
                maxValue = b.ApplyTo(maxValue);

            value = maxValue - valueShortage;
        }

        public void Invalidate()
        {
            lastUpdate = -1;
        }

        public Stat Clone(MapElement newOwner)
        {
            var stat = new Stat(Name, newOwner, baseMaxValue, baseMaxValue - (maxValue - value), limited);
            return stat;
        }

        public override string ToString()
        {
            return string.Format("Attribute: {0}, Value: {1}, MaxValue: {2}, Limited: {3}", Name, Value, MaxValue, Limited);
        }
    }
}
