using System.Collections.Generic;
using UnityEngine;

namespace MechWars
{
    public class DayAndNight : MonoBehaviour
    {
        public List<GameObject> lights;
        public float cycleTime;

        // Update is called once per frame
        void Update()
        {
            var minutes = cycleTime;
            if (minutes == 0) minutes = 1; // zabezpieczenie, bo gdy 0 to będzie dziel/0 dwie linijki niżej
            float seconds = minutes * 360;  // 60 sekund w minucie, okres pelnego cyklu
            float speed = 360 / seconds;   // szybkosc = odwrotnosc okresu

            foreach (var light in lights)
            {
                var transform = light.transform;
                transform.RotateAround(Vector3.zero, Vector3.right, speed * Time.deltaTime); //względem pktu 0, w prawo, szybkość
                transform.LookAt(Vector3.zero); //kierunek patrzenia
            }
        }
    }
}