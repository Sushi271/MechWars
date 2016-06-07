using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements
{
    public class ParticleGroup : MonoBehaviour
    {
        public string name;
        public List<ParticleSystem> particles;

        bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value == enabled)
                    return;
                enabled = value;
                foreach (var ps in particles)
                {
                    ps.gameObject.SetActive(value);
                }
            }
        }
    }
}