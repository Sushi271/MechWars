using MechWars.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechWars.MapElements.WallNeighbourhoods
{
    [ExecuteInEditMode]
    public class WallModelReplacer : MonoBehaviour
    {
        GameObject auxiliaryModel;
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
                    GameObject.Destroy(auxiliaryModel);
                var model = Globals.WallNeighbourhoodDictionary.WallTypesDictionary[neighbourhood].gameObject;
                auxiliaryModel = GameObject.Instantiate(model);
                auxiliaryModel.transform.parent = gameObject.transform;
                auxiliaryModel.transform.localPosition = Vector3.zero;
                this.neighbourhood = neighbourhood;
                notGenerated = false;
            }
        }
    }
}
