using System.Collections.Generic;
using MechWars.MapElements;
using UnityEngine;

namespace MechWars
{
    public class Player : MonoBehaviour
    {
        public Army Army { get; private set; }

        void Start()
        {
            Army = Globals.GetArmyForPlayer(this);
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