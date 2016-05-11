using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechWars.MapElements.WallNeighbourhoods
{
    public class WallNeighbourhoodDefinition : MonoBehaviour
    {
        public bool up;
        public bool down;
        public bool right;
        public bool left;

        public WallNeighbourhood Neighbourhood
        {
            get
            {
                return new WallNeighbourhood(up, down, right, left);
            }
        }

    }
}
