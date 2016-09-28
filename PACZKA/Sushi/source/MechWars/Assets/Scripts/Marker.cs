using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechWars.MapElements
{
    public class Marker : MonoBehaviour
    {
        void Update()
        {
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
