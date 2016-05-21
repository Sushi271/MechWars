using UnityEngine;

namespace MechWars
{
    public class Player : MonoBehaviour
    {
        Army army;
        public Army Army
        {
            get
            {
                if (army == null)
                    army = Globals.GetArmyForPlayer(this);
                return army;
            }
        }
    }
}