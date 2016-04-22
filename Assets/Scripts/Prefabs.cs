using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MechWars
{
    public class Prefabs : MonoBehaviour
    {
        [PrefabType(PrefabType.Resource)]
        public GameObject resource1;

        public List<GameObject> ResourcePrefabs { get { return GetPrefabByType(PrefabType.Resource); } }
        public GameObject RandomResourcePrefab { get { return new System.Random().Choice(ResourcePrefabs); } }

        List<GameObject> GetPrefabByType(PrefabType prefabType)
        {
            return (from f in typeof(Prefabs).GetFields(BindingFlags.Public | BindingFlags.Instance)
                    let attrs = f.GetCustomAttributes(typeof(PrefabTypeAttribute), false)
                    where
                        attrs.Count() > 0 &&
                        (attrs.First() as PrefabTypeAttribute).PrefabType == prefabType
                    select f.GetValue(this) as GameObject)
                   .ToList();
        }

        // ===========================================================================

        enum PrefabType
        {
            Resource
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
