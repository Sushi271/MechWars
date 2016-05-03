using System.Collections.Generic;
using MechWars.MapElements;
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

        void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
        }

        void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }
    }
}