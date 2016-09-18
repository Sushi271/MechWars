using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements
{
    public class ParticleGroup : MonoBehaviour
    {
        bool active;

        public Building building;
        public List<ParticleSystem> particles;

        void Start()
        {
            foreach (var ps in particles)
                ps.Stop();
        }

        void Update()
        {
            if (building == null) return;

            if (!active && !building.UnderConstruction)
            {
                active = true;
                foreach (var ps in particles)
                    ps.Play();
            }
        }
    }
}