using System.Collections.Generic;
using System.Xml;
using MechWars.GLRendering;
using MechWars.Utils;
using UnityEngine;
using MechWars.MapElements.Statistics;
using System;

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

        public Stats Stats { get; private set; }

        static int LastId = 1;
        static int NewId
        {
            get
            {
                return LastId++;
            }
        }

        bool inSelectionBox;
        public bool InSelectionBox
        {
            get { return inSelectionBox; }
            set { if (alive) inSelectionBox = value; }
        }

        bool hovered;
        public bool Hovered
        {
            get { return hovered; }
            set { if (alive) hovered = value; }
        }

        bool selected;
        public bool Selected
        {
            get { return selected; }
            set { if (alive) selected = value; }
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

        public IEnumerable<IVector2> AllCoords
        {
            get
            {
                var list = new List<IVector2>();
                if (Shape == null) list.Add(Coords.Round());
                else
                {
                    int xFrom = Mathf.RoundToInt(Coords.x + Shape.DeltaXNeg);
                    int xTo = Mathf.RoundToInt(Coords.x + Shape.DeltaXPos);
                    int yFrom = Mathf.RoundToInt(Coords.y + Shape.DeltaYNeg);
                    int yTo = Mathf.RoundToInt(Coords.y + Shape.DeltaYPos);
                    for (int x = xFrom, i = 0; x <= xTo; x++, i++)
                        for (int y = yFrom, j = 0; y <= yTo; y++, j++)
                            if (Shape[i, j])
                                list.Add(new IVector2(x, y));
                }
                return list;
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

        public virtual bool Interactible { get { return false; } }

        bool alive;
        public bool Alive
        {
            get { return alive; }
            protected set
            {
                if (!alive) return;
                alive = value;
                if (!alive) OnLifeEnd();
            }
        }

        bool ShouldDrawStatusDisplay { get { return Hovered || Selected || InSelectionBox; } }

        public MapElement()
        {
            Stats = new Stats();
        }

        void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
            id = NewId;

            Globals.MapElementsDatabase.Insert(this);

            if (shapeFile == null)
                Shape = MapElementShape.DefaultShape;
            else Shape = MapElementShape.FromString(shapeFile.text);

            ReadStats();

            alive = true;
            UpdateAlive();

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
                Stat attribute = new Stat(name);
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
            UpdateAlive();
        }

        protected virtual void UpdateAlive()
        {
            if (Alive)
            {
                var hitPoints = Stats[StatNames.HitPoints];
                if (hitPoints != null && hitPoints.Value <= 0)
                    Alive = false;
            }
        }

        protected virtual void OnLifeEnd()
        {
            selected = hovered = inSelectionBox = false;
            if (!Globals.Destroyed)
            {
                Globals.FieldReservationMap.ReleaseReservations(this);
                Globals.MapElementsDatabase.Delete(this);
            }
            if (!suspendDestroy) Destroy(gameObject);
        }

        bool suspendDestroy;
        void OnDestroy()
        {
            suspendDestroy = true;
            if (Alive) Alive = false;
            suspendDestroy = false;
        }

        void OnGUI()
        {
            if (ShouldDrawStatusDisplay)
            {
                var statusDisplay = new StatusDisplayInfo(this);
                DrawBorder(statusDisplay);
                DrawHealthBar(statusDisplay);
            }
        }

        public void DrawStatusDisplay(StatusDisplayInfo statusDisplay)
        {
            DrawBorder(statusDisplay);
            DrawHealthBar(statusDisplay);
        }

        void DrawBorder(StatusDisplayInfo statusDisplay)
        {
            var location = new Vector2(statusDisplay.Left, Screen.height - statusDisplay.Top);
            var size = statusDisplay.Size;
            var distance = 0.5f;

            if (InSelectionBox)
                Globals.GLRenderer.Schedule(new RectangleRenderTask(Color.black, location, size));

            if (Globals.Instance.debugStatusDisplays)
            {
                Vector2 v00 = location;
                Vector2 v01 = location + Vector2.up * size.y;
                Vector2 v10 = location + Vector2.right * size.x;
                Vector2 v11 = location + Vector2.right * size.x + Vector2.up * size.y;

                float lineLength = 0.2f;

                Vector2 u = Vector2.up * size.y * lineLength;
                Vector2 d = Vector2.down * size.y * lineLength;
                Vector2 r = Vector2.right * size.x * lineLength;
                Vector2 l = Vector2.left * size.x * lineLength;

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

        void DrawHealthBar(StatusDisplayInfo statusDisplay)
        {
            var hitPoints = Stats[StatNames.HitPoints];
            if (hitPoints == null) return;

            if (Selected || Hovered)
            {
                GUI.depth = (int)(statusDisplay.Distance * 10);
                GUI.BeginGroup(new Rect(statusDisplay.Location, statusDisplay.Size));

                var w = statusDisplay.Width;
                var h = statusDisplay.Height;

                var mainTexture = army.hpBarMain;
                var sideTexture = army.hpBarSide;
                var barAspectRatio = (float)mainTexture.height / (mainTexture.width + 2 * sideTexture.width);
                var sideAspectRatio = (float)sideTexture.height / sideTexture.width;

                float distFromBounds = 0.1f;

                Vector2 barLocation = distFromBounds * statusDisplay.Size;
                float barWidth = (1 - 2 * distFromBounds) * w;
                float barHeight = barWidth * barAspectRatio;
                float sideWidth = barHeight / sideAspectRatio;

                float ratio = hitPoints.Value / hitPoints.MaxValue;
                float contentBarWidth = ratio * barWidth;
                float mainWidth = contentBarWidth - 2 * sideWidth;
                Vector2 mainLocation = barLocation + Vector2.right * sideWidth;
                Vector2 rightSideLocation = mainLocation + Vector2.right * mainWidth;

                Vector2 barSize = new Vector2(barWidth, barHeight);
                Vector2 sideSize = new Vector2(sideWidth, barHeight);
                Vector2 mainSize = new Vector2(mainWidth, barHeight);
                Vector2 leftSideSize = sideSize;
                float sideToFullRatio = sideWidth / barWidth;
                if (ratio < sideToFullRatio)
                    leftSideSize = new Vector2(ratio * barWidth, barHeight);
                
                GUI.DrawTexture(new Rect(barLocation, barSize), Globals.Textures.hpBarBackground);
                GUI.DrawTexture(new Rect(barLocation, leftSideSize), sideTexture);
                if (mainWidth > 0)
                    GUI.DrawTexture(new Rect(mainLocation, mainSize), mainTexture);
                if (rightSideLocation.x > barLocation.x)
                    GUI.DrawTexture(new Rect(rightSideLocation, sideSize), sideTexture);
                GUI.EndGroup();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", mapElementName ?? "", id);
        }
    }
}