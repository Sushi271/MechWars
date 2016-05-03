using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars
{
    public class TechnologyController
    {
        HashSet<Technology> developingTechnologies;
        HashSet<Technology> developedTechnologies;

        public Army Army { get; private set; }
        public IEnumerable<Technology> DevelopingTechnologies { get { return developingTechnologies; } }
        public IEnumerable<Technology> DevelopedTechnologies { get { return developedTechnologies; } }
        public IEnumerable<StatBonus> CurrentBonuses
        {
            get
            {
                return DevelopedTechnologies.SelectMany(t => t.bonuses);
            }
        }

        public event System.Action OnTechnologyDevelopmentChanged;

        public TechnologyController(Army army)
        {
            developingTechnologies = new HashSet<Technology>();
            developedTechnologies = new HashSet<Technology>();

            Army = army;
        }

        public bool CanDevelop(Technology technology)
        {
            return
                developingTechnologies.None(t => t.IsTheSameAs(technology)) &&
                developedTechnologies.None(t => t.IsTheSameAs(technology));
        }

        public void StartDeveloping(Technology technology)
        {
            if (developingTechnologies.Any(t => t.IsTheSameAs(technology)))
                throw new System.Exception(string.Format(
                    "Army {0} is already developing technology {1}.", Army, technology));
            if (developedTechnologies.Any(t => t.IsTheSameAs(technology)))
                throw new System.Exception(string.Format(
                    "Army {0} has already developed technology {1}.", Army, technology));
            developingTechnologies.Add(technology);

            if (OnTechnologyDevelopmentChanged != null)
                OnTechnologyDevelopmentChanged();
        }

        public void CancelDeveloping(Technology technology)
        {
            if (developingTechnologies.None(t => t.IsTheSameAs(technology)))
                throw new System.Exception(string.Format(
                    "Army {0} is not developing technology {1}.", Army, technology));
            developingTechnologies.RemoveWhere(t => t.IsTheSameAs(technology));

            if (OnTechnologyDevelopmentChanged != null)
                OnTechnologyDevelopmentChanged();
        }

        public void FinishDeveloping(Technology technology)
        {
            CancelDeveloping(technology);
            developedTechnologies.Add(technology);
        }

        public IEnumerable<StatBonus> GetBonusesFor(MapElement receiver)
        {
            return CurrentBonuses.Where(b => b.receiver.mapElementName == receiver.mapElementName);
        }
    }
}
