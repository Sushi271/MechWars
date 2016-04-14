using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MechWars.Utils;

namespace MechWars.MapElements
{
    public class MapElement : MonoBehaviour
    {
        static int LastId = 1;
        static int NewId
        {
            get
            {
                return LastId++;
            }
        }

        public string mapElementName;
        public int id;
        public bool selectable;

        bool hovered;
        public bool Hovered
        {
            get { return hovered; }
            set
            {
                if (value == hovered) return;
                hovered = value;
                OnHoveredChanged();
            }
        }

        bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (value == selected) return;
                selected = value;
                OnSelectedChanged();
            }
        }
        
        public float X
        {
            get { return transform.position.x; }
            set
            {
                var pos = transform.position;
                pos.x = value;
                transform.position = pos;
            }
        }

        public float Y
        {
            get { return transform.position.z; }
            set
            {
                var pos = transform.position;
                pos.z = value;
                transform.position = pos;
            }
        }

        public Vector2 Coords 
        {
            get { return new Vector2(X, Y); }
            set
            {
                var pos = transform.position;
                pos.x = value.x;
                pos.z = value.y;
                transform.position = pos;
            }
        }

        public Vector2 SnappedCoords
        {
            get
            {
                var x = Coords.x;
                var hw = Shape.Width * 0.5f;
                var snapX = Mathf.Round(x - hw + 0.5f) - 0.5f + hw;

                var y = Coords.y;
                var hh = Shape.Height * 0.5f;
                var snapY = Mathf.Round(y - hh + 0.5f) - 0.5f + hh;

                return new Vector2(snapX, snapY);
            }
        }

        public TextAsset shapeFile;
        public MapElementShape Shape { get; private set; }

        protected virtual void OnHoveredChanged()
        {
            UpdateView();
        }

        protected virtual void OnSelectedChanged()
        {
            UpdateView();
        }

        protected virtual void UpdateView()
        {
            var materials =
                from r in gameObject.GetComponentsInChildren<Renderer>()
                select r.material;

            Color color = Color.white;
            if (Hovered && Selected) color = new Color(1, 0.5f, 0);
            else if (Hovered) color = new Color(1, 1, 0);
            else if (Selected) color = new Color(1, 0, 0);
            foreach (var m in materials) m.color = color;
        }

        void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
            id = NewId;

            if (shapeFile == null)
                Shape = MapElementShape.DefaultShape;
            else Shape = MapElementShape.FromString(shapeFile.text);

            InitializeReservation();
        }

        void InitializeReservation()
        {
            var occupiedFields = CalculateOccupiedFields();
            foreach (var coord in occupiedFields)
            {
                Globals.FieldReservationMap.MakeReservation(this, coord);
            }
        }

        List<IVector2> CalculateOccupiedFields()
        {
            var snappedCoords = SnappedCoords;

            var x = snappedCoords.x;
            var hw = Shape.Width * 0.5;
            var minX = x - hw + 0.5;
            
            var y = snappedCoords.y;
            var hh = Shape.Height * 0.5;
            var minY = y - hh + 0.5;

            var occupiedFields = new List<IVector2>();
            for (int i = 0; i < Shape.Width; i++)
                for (int j = 0; j < Shape.Height; j++)
                    if (Shape[i, j])
                        occupiedFields.Add(new IVector2((int)minX + i, (int)minY + j));

            return occupiedFields;
        }

        void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", mapElementName ?? "", id);
        }
    }
}