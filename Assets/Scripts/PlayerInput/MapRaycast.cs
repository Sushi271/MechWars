using MechWars.MapElements;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public class MapRaycast
    {
        public MapElement MapElement { get; private set; }
        public IVector2? Coords { get; private set; }
        
        public void Update()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask(Layer.MapElements)))
            {
                var go = hitInfo.collider.gameObject;
                MapElement = go.GetComponentInParent<MapElement>();
                if (MapElement == null)
                    throw new System.Exception("Non-MapElement GameObject is in MapElements layer.");
                Coords = MapElement.Coords.Round();
            }
            else if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask(Layer.Terrain)))
            {
                Coords = new Vector2(hitInfo.point.x, hitInfo.point.z).Round();
                MapElement = Globals.FieldReservationMap[Coords.Value];
            }
            else
            {
                MapElement = null;
                Coords = null;
            }
        }
    }
}