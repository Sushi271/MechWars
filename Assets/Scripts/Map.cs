using System.Linq;
using MechWars.MapElements;
using MechWars.Utils;
using UnityEngine;

namespace MechWars
{
    public class Map : MonoBehaviour
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public IVector2 Size { get { return new IVector2(Width, Height); } }

        MapElement[,] mapElements;
        public MapElement this[int x, int y]
        {
            get { return mapElements[x, y]; }
        }
        public MapElement this[IVector2 coords]
        {
            get { return this[coords.X, coords.Y]; }
        }

        void Start()
        {
            Width = Globals.Instance.MapWidth;
            Height = Globals.Instance.MapHeight;

            mapElements = new MapElement[Width, Height];
            ScanMap();
        }

        void ScanMap()
        {
            var allMapElements = GameObject.FindGameObjectsWithTag(Tag.MapElement);
            var mapElementAABBs =
                from a in allMapElements
                select new
                {
                    MapElement = a.GetComponent<MapElement>(),
                    AABB = a.GetComponent<Collider>().bounds
                };

            var fieldColliderObject = UnityExtensions.CreateFieldCollider();
            var fieldCollider = fieldColliderObject.GetComponent<BoxCollider>();

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    fieldCollider.transform.localPosition = new Vector3(x, 0, y);
                    var actorAABB = mapElementAABBs.FirstOrDefault(aa => aa.AABB.IntersectsStrictly(fieldCollider.bounds));
                    if (actorAABB != null) mapElements[x, y] = actorAABB.MapElement;
                }

            GameObject.DestroyImmediate(fieldCollider);
        }
    }
}