using MechWars.MapElements.Statistics;
using MechWars.Utils;
using UnityEngine;
using MechWars.MapElements.Attacks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MechWars.MapElements.Orders
{
    public static class OrderResultAsserts
    {
        public static FailOrderResult AssertMapElementHasStat(MapElement mapElement, string statName, out Stat stat)
        {
            stat = mapElement.Stats[statName];
            if (stat == null)
                return new FailOrderResult(string.Format(
                    "Missing {0} Stat in MapElement's Stats! (MapElement {1})",
                    statName, mapElement));
            return null;
        }

        public static FailOrderResult AssertMapElementHasStat(MapElement mapElement, string statName)
        {
            Stat stat;
            return AssertMapElementHasStat(mapElement, statName, out stat);
        }

        public static FailOrderResult AssertMapElementIsInIntegerCoords(MapElement mapElement)
        {
            if (!mapElement.Coords.IsInteger())
                return new FailOrderResult(string.Format(
                    "MapElement.Coords {0} is not an integer vector (MapElement {1}).",
                    mapElement.Coords, mapElement));
            return null;
        }

        public static FailOrderResult AssertIsNotNull(object o, string objectName)
        {
            if (o == null)
                return new FailOrderResult(string.Format(
                    "{0} is NULL.", objectName));
            return null;

        }

        public static FailOrderResult AssertBuildingIsResourceDeposit(Building building)
        {
            if (!building.isResourceDeposit)
                return new FailOrderResult(string.Format(
                    "Building {0} is not a resource deposit.", building));
            return null;
        }

        public static FailOrderResult AssertBuildingIsNotUnderConstruction(Building building)
        {
            if (building.UnderConstruction)
                return new FailOrderResult(string.Format(
                    "Building {0} is not under construction.", building));
            return null;
        }

        public static FailOrderResult AssertUnitIsNeighbourOf(Unit unit, MapElement mapElement)
        {
            var allRefineryCoords = mapElement.AllCoords;
            if (allRefineryCoords.None(c =>
                {
                    var dr = c - unit.Coords;
                    return Mathf.Abs(dr.x) <= 1 && Mathf.Abs(dr.y) <= 1;
                }))
                return new FailOrderResult(string.Format(
                    "Unit {0} is not a neighbour of MapElement {1}.", unit, mapElement));
            return null;
        }

        public static FailOrderResult AssertMapElementHasAttack(MapElement mapElement, Attack attack)
        {
            if (!mapElement.GetComponents<Attack>().Any(a => a == attack))
                return new FailOrderResult(string.Format(
                    "MapElement {0} does not have given Attack (of type {1}).",
                    mapElement, attack.GetType()));
            return null;
        }

        public static FailOrderResult AssertMapElementHasReadiedAttack(MapElement mapElement)
        {
            if (mapElement.ReadiedAttack == null)
                return new FailOrderResult(string.Format(
                    "MapElement {0} does not have any Attack ready.",
                    mapElement));
            return null;
        }

        public static FailOrderResult AssertAimInAttackRange(MapElement mapElement, Vector3 aim)
        {
            if (!mapElement.HasPositionInRange(aim.AsHorizontalVector2(), StatNames.AttackRange))
                return new FailOrderResult(string.Format(
                    "Aim {0} is not in MapElement {1} attack range.",
                    aim, mapElement));
            return null;
        }

        public static FailOrderResult AssertDestinationIsNeighbourCoords(MapElement mapElement, IVector2 destination)
        {
            if (!mapElement.Coords.Round().IsNeighbourTo(destination))
                return new FailOrderResult(string.Format(
                    "Destination {0} is not a neighbour of MapElement.Coords ({1}, MapElement {2}).",
                    destination, mapElement.Coords, mapElement));
            return null;
        }

        public static FailOrderResult AssertDestinationIsInsideMap(IVector2 destination)
        {
            if (!Globals.FieldReservationMap.CoordsInside(destination))
                return new FailOrderResult(string.Format(
                    "Destination {0} is outside of map.",
                    destination));
            return null;
        }

        public static FailOrderResult AssertDestinationIsNotOccupied(MapElement mapElement, IVector2 destination)
        {
            if (Globals.FieldReservationMap.FieldOccupiedFor(mapElement, destination))
                return new FailOrderResult(string.Format(
                    "Destination {0} is occupied for MapElement {1}.",
                    destination, mapElement));
            return null;
        }

        public static FailOrderResult AssertMapElementIsNotDying(MapElement mapElement)
        {
            if (mapElement.Dying)
                return new FailOrderResult(string.Format(
                    "MapElement {0} is Dying.", mapElement));
            return null;
        }

        public static FailOrderResult AssertMapElementHasAnyAttacks(MapElement mapElement)
        {
            if (mapElement.GetComponents<Attack>().Empty())
                return new FailOrderResult(string.Format(
                    "MapElement {0} has no Attacks.", mapElement));
            return null;
        }

        public static FailOrderResult AssertMapElementCanBeAttacked(MapElement mapElement)
        {
            if (!mapElement.CanBeAttacked)
                return new FailOrderResult(string.Format(
                    "MapElement {0} cannot be attacked.", mapElement));
            return null;
        }

        public static FailOrderResult AssertMapElementsCanBeAttacked(IEnumerable<MapElement> mapElement)
        {
            var nonAttackable = mapElement.Where(t => !t.CanBeAttacked);
            if (!nonAttackable.Empty())
                return new FailOrderResult(string.Format("Following MapElements cannot be attacked: {0}.",
                    nonAttackable.ToDebugMessage()));
            return null;
        }
    }
}
