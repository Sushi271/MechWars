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

        void OnUpdate()
        {
            InputController.Update();
        }
    }
}