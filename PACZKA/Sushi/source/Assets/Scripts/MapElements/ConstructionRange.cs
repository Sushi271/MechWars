using MechWars.FogOfWar;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements
{
    public class ConstructionRange : MonoBehaviour
    {
        bool invalid = true;

        MapElementSurroundingShape surroundingShape;
        bool[,] inRange;
        GameObject[,] tiles;

        public GameObject tilePrefab;
        public Building constructingBuilding;
        
        void Start()
        {
            if (constructingBuilding == null)
                throw new System.Exception("ConstructionRange.constructingBuilding must be set.");
            var rangeStat = constructingBuilding.Stats[StatNames.ConstructionRange];
            if (rangeStat == null)
                throw new System.Exception(string.Format(
                    "ConstructionRange.constructingBuilding must have {0} Stat.", StatNames.ConstructionRange));
            invalid = false;

            var range = rangeStat.Value;
            surroundingShape = new MapElementSurroundingShape(range, constructingBuilding.Shape);

            var x = constructingBuilding.Coords.x;
            var y = constructingBuilding.Coords.y;

            inRange = new bool[surroundingShape.Width, surroundingShape.Height];
            tiles = new GameObject[surroundingShape.Width, surroundingShape.Height];
            for (int rx = surroundingShape.GetXMin(x), i = 0; rx <= surroundingShape.GetXMax(x); rx++, i++)
                for (int ry = surroundingShape.GetYMin(y), j = 0; ry <= surroundingShape.GetYMax(y); ry++, j++)
                    if (surroundingShape[i, j]) tiles[i, j] = CreateTile(rx, ry);
        }

        GameObject CreateTile(float x, float y)
        {
            var tile = Instantiate(tilePrefab);
            tile.transform.position = new Vector3(x, 0, y);
            tile.transform.parent = transform;
            tile.SetActive(false);
            return tile;
        }

        void Update()
        {
            if (invalid) return;

            var x = constructingBuilding.Coords.x;
            var y = constructingBuilding.Coords.y;
            
            for (int rx = surroundingShape.GetXMin(x), i = 0; rx <= surroundingShape.GetXMax(x); rx++, i++)
                for (int ry = surroundingShape.GetYMin(y), j = 0; ry <= surroundingShape.GetYMax(y); ry++, j++)
                {
                    if (tiles[i, j] == null) continue;
                    if (rx < 0 || Globals.Map.Size <= rx ||
                        ry < 0 || Globals.Map.Size <= ry) continue;

                    var inRange = Globals.Map[rx, ry].IsTrueNull() &&
                        constructingBuilding.Army.VisibilityTable[rx, ry] != Visibility.Unknown;
                    this.inRange[i, j] = inRange;
                    tiles[i, j].SetActive(inRange);
                }
        }

        public bool FieldInRange(IVector2 coords)
        {
            var x = constructingBuilding.Coords.x;
            var y = constructingBuilding.Coords.y;
            var i = coords.X - (int)(x + surroundingShape.DeltaXNeg);
            var j = coords.Y - (int)(y + surroundingShape.DeltaYNeg);


            if (i < 0 || surroundingShape.Width <= i ||
                j < 0 || surroundingShape.Height <= j) return false;

            return inRange[i, j];
        }
    }
}
