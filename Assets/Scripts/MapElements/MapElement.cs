using UnityEngine;
using System.Collections;

namespace MechWars.MapElements
{
    public class MapElement : MonoBehaviour
    {
        public string mapElementName;
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
            var mat = gameObject.GetComponent<Renderer>().material;

            Color color = Color.white;
            if (Hovered && Selected) color = new Color(1, 0.5f, 0);
            else if (Hovered) color = new Color(1, 1, 0);
            else if (Selected) color = new Color(1, 0, 0);
            mat.color = color;
        }

        void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
        }

        void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }
    }
}