using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.MapElements
{
    public class AttackHead : MonoBehaviour
    {
        public List<GameObject> tips;

        public IEnumerable<Vector3> TipsPositions { get { return from t in tips select t.transform.position; } }
    }
}
