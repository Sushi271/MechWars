using UnityEngine;

namespace MechWars.AI
{
    public class AIBrainTest : MonoBehaviour
    {
        public AIBrain Brain { get { return GetComponent<AIBrain>(); } }
    }
}