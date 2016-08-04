using UnityEngine;

namespace MechWars.Utils
{
    public abstract class ScriptAction : MonoBehaviour
    {
        public abstract void Invoke(object arg = null);
    }
}