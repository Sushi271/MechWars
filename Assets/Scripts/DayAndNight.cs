using MechWars;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float dayAndNightCycleTime = Globals.Instance.dayAndNightCycleTime;
        if (dayAndNightCycleTime == 0) dayAndNightCycleTime = 1; // zabezpieczenie, bo gdy 0 to będzie dziel/0 dwie linijki niżej
        float time = dayAndNightCycleTime * 60; // 60 sekund w minucie, okres pelnego cyklu
        float speed = 360 / time; // szybkosc = odwrotnosc okresu

        transform.RotateAround(Vector3.zero, Vector3.right, speed * Time.deltaTime); //względem pktu 0, w prawo, szybkość
        transform.LookAt(Vector3.zero); //kierunek patrzenia
    }
}