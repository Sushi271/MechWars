using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;

namespace MechWars
{
    [System.Serializable]
    public class Army : MonoBehaviour
    {
        public Player Player { get; private set; }
        public TechnologyController Technologies { get; private set; }

        public string armyName;

        public int resources;

        public Texture hpBarMain;
        public Texture hpBarSide;
        public Texture hpBarTop;

        public Army()
        {
            Technologies = new TechnologyController(this);
        }

        void Start()
        {           
            Player = Globals.GetPlayerForArmy(this);
        }
    }
}