using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements.WallNeighbourhoods
{
    public class WallModelReplacer : MonoBehaviour
    {
        public GameObject auxiliaryModel;
        WallNeighbourhood neighbourhood;

        bool notGenerated = true;

        void Update()
        {
            var mapElement = GetComponent<MapElement>();
            var coords = mapElement.Coords.Round();

            var up = Globals.FieldReservationMap[coords.X, coords.Y + 1];
            var down = Globals.FieldReservationMap[coords.X, coords.Y - 1];
            var right = Globals.FieldReservationMap[coords.X + 1, coords.Y];
            var left = Globals.FieldReservationMap[coords.X - 1, coords.Y];

            var neighbourhood = new WallNeighbourhood(
                up is Building,
                down is Building,
                right is Building,
                left is Building);

            if (!neighbourhood.Equals(this.neighbourhood) || notGenerated)
            {
                if (auxiliaryModel != null)
                    Destroy(auxiliaryModel);
                var model = Globals.WallNeighbourhoodDictionary.WallTypesDictionary[neighbourhood].gameObject;
                auxiliaryModel = Instantiate(model);
                auxiliaryModel.transform.SetParent(gameObject.transform, false);
                this.neighbourhood = neighbourhood;
                notGenerated = false;
            }
        }
    }
}
