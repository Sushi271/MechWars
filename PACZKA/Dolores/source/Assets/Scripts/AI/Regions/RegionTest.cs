using UnityEngine;

namespace MechWars.AI.Regions
{
    public class RegionTest : MonoBehaviour
    {
        public TextAsset regionReadFile;
        public Region region;
        public RegionHull hull;
        public RegionConvexHull convexHull;
        
        void Start()
        {
            region = new Region();
        }
    }
}
