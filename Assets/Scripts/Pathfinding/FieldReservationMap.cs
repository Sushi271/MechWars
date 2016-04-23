﻿using System.Collections.Generic;
using System.Text;
using MechWars.MapElements;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.Pathfinding
{
    public class FieldReservationMap : MonoBehaviour
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public IVector2 Size { get { return new IVector2(Width, Height); } }

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
            get { return reservationTable[x, y]; }
            private set { reservationTable[x, y] = value; }
        }
        public MapElement this[IVector2 coords]
        {
            get { return this[coords.X, coords.Y]; }
            private set { this[coords.X, coords.Y] = value; }
        }

        public bool CoordsInside(IVector2 coords)
        {
            return
                0 <= coords.X && coords.X < Width &&
                0 <= coords.Y && coords.Y < Height;
        }

        void Start()
        {
            reservationDictionary = new Dictionary<MapElement, List<IVector2>>();

            Width = Globals.Instance.MapWidth;
            Height = Globals.Instance.MapHeight;
            reservationTable = new MapElement[Width, Height];
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                var sb = new StringBuilder();
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        if (this[j, i] == null) sb.Append('.');
                        else sb.Append(this[j, i].id);
                    }
                    sb.AppendLine();
                }
                Debug.Log(sb.ToString());
            }
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
    }
}