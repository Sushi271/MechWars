using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements
{
    public class ParticleManager : MonoBehaviour
    {
        public List<ParticleGroup> particleGroups;

        public Dictionary<string, ParticleGroup> ParticleGroupDictionary { get; private set; }

        void Start()
        {
            foreach (var pg in particleGroups)
            {
                ParticleGroupDictionary.Add(pg.name, pg);
            }
        }
    }
}