using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MechWars.GLRendering;
using MechWars.Utils;
using UnityEngine;

namespace MechWars.MapElements
{
    public class MapElement : MonoBehaviour
    {
        public string mapElementName;
        public int id;
        public bool selectable;
        public Army army;
        public TextAsset shapeFile;
        public TextAsset statsFile;
        public float displaySize = 1;
        public float displayYOffset = 0;

        StatusDisplay statusDisplay;

        public Stats Stats { get; private set; }

        static int LastId = 1;
        static int NewId
        {
            get
            {
                return LastId++;
            }
        }

        public bool InSelectionBox { get; set; }
        public bool Hovered { get; set; }
        public bool Selected { get; set; }

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

        public MapElementShape Shape { get; private set; }

        public MapElement()
        {
            Stats = new Stats();
            statusDisplay = new StatusDisplay(this, StatusDisplayAction);
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

            ReadStats();

            InitializeReservation();
        }

        void ReadStats()
        {
            if (statsFile == null) return;

            var xml = new XmlDocument();
            xml.LoadXml(statsFile.text);
            var root = xml.DocumentElement;
            var nodes = root.SelectNodes("Stat");
            foreach (var n in nodes)
            {
                var e = n as XmlElement;
                if (e == null) continue;
                var name = e.GetAttribute("name");
                var value = float.Parse(e.GetAttribute("value"));
                var limited = e.HasAttribute("max_value");
                float maxValue = 0;
                if (limited) maxValue = float.Parse(e.GetAttribute("max_value"));
                Attribute attribute = new Attribute(name);
                attribute.MaxValue = maxValue;
                attribute.Value = value;
                attribute.Limited = limited;
                Stats.Add(attribute);
            }
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
            statusDisplay.Draw();
        }

        private void StatusDisplayAction(int centerX, int centerY, int displayWidth, int displayHeight, float distance)
        {
            Vector2 location = new Vector2(centerX - displayWidth * 0.5f, centerY - displayHeight * 0.5f);
            Vector2 size = new Vector2(displayWidth, displayHeight);
            
            float glDistance = 0.9f - (distance / 100.0f);

            DrawBorder(centerX, centerY, displayWidth, displayHeight, glDistance, location, size);
            DrawHealthBar(centerX, centerY, displayWidth, displayHeight, glDistance, location, size);
        }

        void DrawBorder(int centerX, int centerY, int displayWidth, int displayHeight, float distance, Vector2 location, Vector2 size)
        {
            if (InSelectionBox)
                Globals.GLRenderer.Schedule(new RectangleRenderTask(Color.black, location, size));

            if (Globals.Instance.debugStatusDisplays)
            {
                Vector2 v00 = location;
                Vector2 v01 = location + Vector2.up * displayHeight;
                Vector2 v10 = location + Vector2.right * displayWidth;
                Vector2 v11 = v10 + Vector2.up * displayHeight;

                float lineLength = 0.2f;

                Vector2 u = Vector2.up * displayHeight * lineLength;
                Vector2 d = Vector2.down * displayHeight * lineLength;
                Vector2 r = Vector2.right * displayWidth * lineLength;
                Vector2 l = Vector2.left * displayWidth * lineLength;

                if (!InSelectionBox && (Hovered || Selected))
                {
                    Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v00, v00 + r, distance));
                    Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v00, v00 + u, distance));

                    Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v01, v01 + r, distance));
                    Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v01, v01 + d, distance));

                    Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v10, v10 + l, distance));
                    Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v10, v10 + u, distance));

                    Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v11, v11 + l, distance));
                    Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v11, v11 + d, distance));

                    if (Selected)
                    {
                        Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v00 + 2 * r, v10 + 2 * l, distance));
                        Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v01 + 2 * r, v11 + 2 * l, distance));
                        Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v00 + 2 * u, v01 + 2 * d, distance));
                        Globals.GLRenderer.Schedule(new LineRenderTask(Color.black, v10 + 2 * u, v11 + 2 * d, distance));
                    }
                }
            }
        }

        void DrawHealthBar(int centerX, int centerY, int displayWidth, int displayHeight, float distance, Vector2 location, Vector2 size)
        {
            var hitPoints = Stats[StatNames.HitPoints];
            if (hitPoints == null) return;

            if (Selected || Hovered)
            {
                float distFromBounds = 0.1f;
                float barThickness = 0.07f;

                Vector2 barLocation = location + size.VY() +
                    Vector2.right * size.x * distFromBounds +
                    Vector2.down * size.y * (distFromBounds + barThickness);
                float barHeight = barThickness * size.y;
                float barWidth = (1 - 2 * distFromBounds) * size.x;

                float ratio = hitPoints.Value / hitPoints.MaxValue;
                float colorBarWidth = ratio * barWidth;
                float blackBarWidth = barWidth - colorBarWidth;
                
                Vector2 colorBarSize = new Vector2(colorBarWidth, barHeight);
                Vector2 blackBarSize = new Vector2(blackBarWidth, barHeight);
                Vector2 blackBarLocation = barLocation + colorBarSize.VX();

                Color color =
                    ratio > 0.666 ? Color.green :
                    ratio > 0.333 ? Color.yellow :
                    Color.red;

                Globals.GLRenderer.Schedule(new FillRectangleRenderTask(color, barLocation, colorBarSize, distance));
                if (blackBarWidth > 0)
                    Globals.GLRenderer.Schedule(new FillRectangleRenderTask(Color.black, blackBarLocation , blackBarSize, distance));

                if (Hovered)
                {
                    float borderOffset = 0.03f * size.x;

                    Vector2 locHorBot = barLocation - Vector2.one * borderOffset;
                    Vector2 locHorTop = barLocation + new Vector2(-borderOffset, barHeight);
                    Vector2 sizeHor = new Vector2(barWidth + 2 * borderOffset, borderOffset);

                    Vector2 locVerLft = barLocation + Vector2.left * borderOffset;
                    Vector2 locVerRgt = barLocation + Vector2.right * barWidth;
                    Vector2 sizeVer = new Vector2(borderOffset, barHeight);

                    Globals.GLRenderer.Schedule(new FillRectangleRenderTask(Color.black, locHorBot, sizeHor, distance));
                    Globals.GLRenderer.Schedule(new FillRectangleRenderTask(Color.black, locHorTop, sizeHor, distance));
                    Globals.GLRenderer.Schedule(new FillRectangleRenderTask(Color.black, locVerLft, sizeVer, distance));
                    Globals.GLRenderer.Schedule(new FillRectangleRenderTask(Color.black, locVerRgt, sizeVer, distance));
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", mapElementName ?? "", id);
        }
    }
}