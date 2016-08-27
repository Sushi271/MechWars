using System.Collections.Generic;
using MechWars.MapElements;
using MechWars.Utils;
using UnityEngine;
using System.Linq;

namespace MechWars.Mapping
{
    public class Map : MonoBehaviour
    {
        public int Size { get; private set; }

        Dictionary<MapElement, List<IVector2>> reservationDictionary;
        public List<IVector2> this[MapElement me]
        {
            get
            {
                if (me == null)
                    throw new System.Exception("Cannot get reservations for NULL.");
                List<IVector2> reservations;
                var success = reservationDictionary.TryGetValue(me, out reservations);
                if (!success) return new List<IVector2>();
                else return reservations;
            }
        }

        MapElement[,] reservationTable;
        public MapElement this[int x, int y]
        {
            get { return !IsInBounds(x, y) ? null : reservationTable[x, y]; }
            private set
            {
                if (IsInBounds(x, y))
                    reservationTable[x, y] = value;
            }
        }


        Dictionary<MapElement, List<IVector2>> ghostDictionary;
        public List<IVector2> GetGhostPositions(MapElement ghost)
        {
            if (ghost == null)
                throw new System.Exception("Cannot get positions for NULL Ghost.");
            List<IVector2> positions;
            var success = ghostDictionary.TryGetValue(ghost, out positions);
            if (!success) return new List<IVector2>();
            else return positions;
        }

        public bool ContainsGhost(MapElement ghost)
        {
            if (ghost == null) return false;
            return ghostDictionary.ContainsKey(ghost);
        }

        List<MapElement>[,] ghostsTable;
        public IEnumerable<MapElement> GetGhosts(int x, int y)
        {
            if (!IsInBounds(x, y) ||
                ghostsTable[x, y] == null) return Enumerable.Empty<MapElement>();
            return ghostsTable[x, y];
        }

        public MapElement this[IVector2 coords]
        {
            get { return this[coords.X, coords.Y]; }
            private set { this[coords.X, coords.Y] = value; }
        }
        
        public bool IsInBounds(IVector2 coords)
        {
            return IsInBounds(coords.X, coords.Y);
        }

        public bool IsInBounds(int x, int y)
        {
            return
                0 <= x && x < Size &&
                0 <= y && y < Size;
        }

        public bool FieldOccupiedFor(MapElement mapElement, IVector2 coords)
        {
            var occupier = this[coords];
            return occupier != null && occupier != mapElement;
        }

        void Start()
        {
            reservationDictionary = new Dictionary<MapElement, List<IVector2>>();
            ghostDictionary = new Dictionary<MapElement, List<IVector2>>();

            Size = Globals.MapSettings.Size;
            reservationTable = new MapElement[Size, Size];
            ghostsTable = new List<MapElement>[Size, Size];
        }

        public void MakeReservation(MapElement mapElement, IVector2 coords)
        {
            if (mapElement == null)
                throw new System.Exception("Cannot make reservation for NULL.");
            if (this[coords] != null)
            {
                throw new System.Exception(string.Format("Reservation conflict. " +
                    "Coords: {0}, Old reservation: {1}, new reservation: {2}.",
                    coords.ToString(), this[coords].ToString(), mapElement.ToString()));
            }
            this[coords] = mapElement;

            List<IVector2> reservations;
            reservationDictionary.TryGetValue(mapElement, out reservations);
            if (reservations == null)
            {
                reservations = new List<IVector2>();
                reservationDictionary.Add(mapElement, reservations);
            }
            reservations.Add(coords);
        }

        public void ReleaseReservation(MapElement mapElement, IVector2 coords)
        {
            if (mapElement == null)
                throw new System.Exception("Cannot release reservation for NULL.");
            if (this[coords] != mapElement)
            {
                var realReservation = this[coords] == null ? "NULL" : this[coords].ToString();
                throw new System.Exception(string.Format("Given MapElement doesn't have reservation in given coords. " +
                    "Coords: {0}, Real reservation: {1}, Given MapElement: {2}",
                    coords.ToString(), realReservation, mapElement.ToString()));
            }
            
            var reservations = reservationDictionary[mapElement];
            reservations.Remove(coords);
            if (reservations.Count == 0)
                reservationDictionary.Remove(mapElement);
            this[coords] = null;
        }

        public void ReleaseReservations(MapElement mapElement)
        {
            var reservations = this[mapElement];
            foreach (var r in reservations)
                this[r] = null;
            reservationDictionary.Remove(mapElement);
        }

        public void AddGhost(MapElement ghost)
        {
            var occupiedFields = ghost.AllCoords.ToList();
            foreach (var coord in occupiedFields)
            {
                var list = ghostsTable[coord.X, coord.Y];
                if (list == null)
                {
                    list = new List<MapElement>();
                    ghostsTable[coord.X, coord.Y] = list;
                }
                list.Add(ghost);
            }
            ghostDictionary[ghost] = new List<IVector2>(occupiedFields);
        }

        public void RemoveGhost(MapElement ghost)
        {
            var occupiedFields = ghost.AllCoords;
            foreach (var coord in occupiedFields)
            {
                var list = ghostsTable[coord.X, coord.Y];
                list.Remove(ghost);
                if (list.Empty())
                    ghostsTable[coord.X, coord.Y] = null;
            }
            ghostDictionary.Remove(ghost);
        }

        public QuadTree CreateQuadTree()
        {
            return new QuadTree(new SquareBounds(new IVector2(), Size));
        }
    }
}