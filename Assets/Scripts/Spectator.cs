using MechWars.PlayerInput;
using UnityEngine;

namespace MechWars
{
    public class Spectator : MonoBehaviour
    {
        public Player player;

        public InputController InputController { get; private set; }
        
        public Spectator()
        {
            InputController = new InputController(this);
        }

        void Start()
        {
            var army = Globals.HumanArmy;
            if (army != null) army.actionsVisible = true;
        }

        void Update()
        {
            InputController.Update();
        }
    }
}