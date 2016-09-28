using UnityEngine;

namespace MechWars.Utils
{
    public class CounterResolver : MonoBehaviour
    {
        void Update()
        {
            var counters = Globals.DEBUG_Counters;
            for (int i = 0; i < counters.Count; i++)
            {
                Debug.Log(string.Format("C[{0}]: {1}", i, counters[i].Value));
                counters[i].Reset();
            }
        }
    }
}