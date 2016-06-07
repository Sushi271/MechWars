using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MechWars
{
    public class Prefabs : MonoBehaviour
    {
        System.Random random = new System.Random();

        [PrefabType(PrefabType.Other)]
        public GameObject marker;
        [PrefabType(PrefabType.Resource)]
        public GameObject resource1;
        [PrefabType(PrefabType.Resource)]
        public GameObject resource2;

        public List<GameObject> ResourcePrefabs { get { return GetPrefabByType(PrefabType.Resource); } }
        public GameObject RandomResourcePrefab { get { return random.Choice(ResourcePrefabs); } }

        List<GameObject> GetPrefabByType(PrefabType prefabType)
        {
            return (from f in typeof(Prefabs).GetFields(BindingFlags.Public | BindingFlags.Instance)
                    let attrs = f.GetCustomAttributes(typeof(PrefabTypeAttribute), false)
                    where
                        attrs.Count() > 0 &&
                        (attrs.First() as PrefabTypeAttribute).PrefabType == prefabType
                    let v = f.GetValue(this)
                    where v != null
                    select v as GameObject)
                   .ToList();
        }

        // ===========================================================================

        enum PrefabType
        {
            Resource,
            Other
        }

        class PrefabTypeAttribute : System.Attribute
        {
            public PrefabType PrefabType { get; set; }

            public PrefabTypeAttribute(PrefabType prefabType)
            {
                PrefabType = prefabType;
            }
        }
    }
}
