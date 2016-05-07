using MechWars.GLRendering;
using MechWars.MapElements.Jobs;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace MechWars.MapElements
{
    public class MapElement : MonoBehaviour
    {
        Army lastArmy;

        public string mapElementName;
        public int id;
        public Army army;
        public TextAsset shapeFile;
        public TextAsset statsFile;
        public bool selectable;
        public bool canAttack;
        public bool canBeAttacked;
        public bool canRotate;
        public float yToAim;
        public float displaySize = 1;
        public float displayYOffset = 0;

        public int resourceValue;
        public int additionalResourceValue;
        public bool generateResourcesOnDeath = true;

        public bool isShadow;

        bool reservationInitialized;

        public JobQueue JobQueue { get; private set; }

        public Stats Stats { get; private set; }
        bool statsRead;

        static int LastId = 1;
        static int NewId
        {
            get
            {
                return LastId++;
            }
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

        public MapElementShape Shape { get { return Globals.ShapeDatabase[this]; } }

        public virtual float? LifeValue
        {
            get
            {
                var hp = Stats[StatNames.HitPoints];
                if (hp != null) return hp.Value;
                else return null;
            }
        }

        public bool Dying { get; private set; }

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

        bool ShouldDrawStatusDisplay { get { return Hovered || Selected; } }

        protected virtual bool CanAddToArmy { get { return false; } }

        public MapElement()
        {
            JobQueue = new JobQueue(this);
            Stats = new Stats(this);
            alive = true;
        }

        void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
            if (isShadow) return;

            id = NewId;

            Globals.MapElementsDatabase.Insert(this);
            if (army != null && CanAddToArmy)
                army.AddMapElement(this);

            ReadStats();

            UpdateDying();
            UpdateAlive();

            InitializeReservation();
        }

        public void ReadStats(bool force = false)
        {
            if (statsFile == null) return;
            if (!force && statsRead) return;
            statsRead = true;

            Stats.Clear();

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
                Stat attribute = new Stat(name, this);
                attribute.MaxValue = maxValue;
                attribute.Value = value;
                attribute.Limited = limited;
                Stats.Add(attribute);
            }
        }

        public void InitializeReservation()
        {
            if (reservationInitialized) return;

            var occupiedFields = AllCoords;
            foreach (var coord in occupiedFields)
            {
                Globals.FieldReservationMap.MakeReservation(this, coord);
            }
            reservationInitialized = true;
        }

        public bool MapElementInRange(MapElement other)
        {
            Vector2 coords;
            return MapElementInRange(other, out coords);
        }

        public bool MapElementInRange(MapElement other, out Vector2 outCoords)
        {
            outCoords = Vector2.zero;

            var range = Stats[StatNames.Range];
            if (range == null) return false;

            float minDist = Mathf.Infinity;
            bool inRange = false;

            foreach (var c in other.AllCoords)
            {
                var dr = c - Coords;
                if ((Mathf.Abs(dr.x) <= 1 && Mathf.Abs(dr.y) <= 1 ||
                    dr.magnitude <= range.Value) && dr.magnitude < minDist)
                {
                    inRange = true;
                    minDist = dr.magnitude;
                    outCoords = c;
                }
            }

            return inRange;
        }

        public MapElement AcquireTarget()
        {
            var range = Stats[StatNames.Range];
            if (range == null) return null;

            var coords =
                from c in CoordsInRangeSquare(range.Value)
                where Vector2.Distance(c, Coords) <= range.Value
                where Globals.FieldReservationMap.CoordsInside(c)
                let me = Globals.FieldReservationMap[c]
                where me != null && me.army != null && me.army != army
                where MapElementInRange(me)
                select me;
            if (coords.Count() == 0) return null;

            var target = coords.SelectMin(me => Vector2.Distance(me.Coords, Coords));
            return target;
        }

        IEnumerable<IVector2> CoordsInRangeSquare(float range)
        {
            int xFrom = Mathf.CeilToInt(X - range);
            int xTo = Mathf.FloorToInt(X + range);
            int yFrom = Mathf.CeilToInt(Y - range);
            int yTo = Mathf.FloorToInt(Y + range);

            for (int y = yFrom; y <= yTo; y++)
                for (int x = xFrom; x <= xTo; x++)
                    yield return new IVector2(x, y);
        }

        void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
            if (isShadow) return;

            if (lastArmy != army)
            {
                if (CanAddToArmy)
                {
                    if (lastArmy != null)
                        lastArmy.RemoveMapElement(this);
                    if (army != null)
                        army.AddMapElement(this);
                }
                lastArmy = army;
            }

            UpdateDying();
            JobQueue.Update();
            UpdateAlive();
        }

        protected void UpdateDying()
        {
            if (!Dying && LifeValue == 0)
            {
                JobQueue.Clear();
                Dying = true;
            }
        }

        protected virtual void UpdateAlive()
        {
            if (!Dying || !Alive) return;
            Alive = !JobQueue.Empty;
        }

        protected virtual void OnLifeEnd()
        {
            selected = hovered = false;

            if (army != null && CanAddToArmy)
                army.RemoveMapElement(this);

            if (!Globals.Destroyed)
            {
                Globals.FieldReservationMap.ReleaseReservations(this);
                Globals.MapElementsDatabase.Delete(this);
            }
            if (!suspendDestroy) Destroy(gameObject);

            TurnIntoResource();
        }

        void TurnIntoResource()
        {
            if (Globals.Destroyed || !generateResourcesOnDeath || isShadow) return;

            int resVal = resourceValue + additionalResourceValue;
            if (resVal == 0) return;

            var noArmy = GameObject.FindGameObjectsWithTag(Tag.Army)
                .Where(a => a.GetComponent<Army>() == null).FirstOrDefault();
            if (noArmy == null) return;

            var r = new System.Random();
            var value = r.Next(resVal / 4, resVal / 2);
            if (value == 0) return;

            var allCoords = AllCoords.ToList();
            var proportions = new List<float>();
            float sum = 0;
            for (int i = 0; i < allCoords.Count; i++)
            {
                float a = (float)r.NextDouble();
                sum += a;
                proportions.Add(a);
            }
            for (int i = 0; i < allCoords.Count; i++)
                proportions[i] = proportions[i] / sum * value;
            int iSum = 0;
            float overflow = 0;
            var values = new List<int>();
            for (int i = 0; i < allCoords.Count - 1; i++)
            {
                int intOverflow = (int)overflow;
                overflow -= intOverflow;
                values.Add((int)proportions[i]);
                overflow += proportions[i] - values[i];
                iSum += values[i];
            }
            values.Add(value - iSum);

            for (int i = 0; i < allCoords.Count; i++)
            {
                if (values[i] == 0) continue;

                var resPrefab = Globals.Prefabs.RandomResourcePrefab;
                var gameObject = Instantiate(resPrefab);
                gameObject.transform.parent = noArmy.transform;
                gameObject.name = resPrefab.name;
                var resource = gameObject.GetComponent<Resource>();
                resource.Coords = allCoords[i];
                resource.value = values[i];
                resource.InitializeReservation();
            }
        }

        bool suspendDestroy;
        void OnDestroy()
        {
            suspendDestroy = true;
            if (!Dying)
            {
                JobQueue.TotalClear();
                Dying = true;
            }
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

                if (Hovered || Selected)
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

                Texture mainTexture;
                Texture sideTexture;
                Texture topTexture;
                if (army != null)
                {
                    mainTexture = army.hpBarMain;
                    sideTexture = army.hpBarSide;
                    topTexture = army.hpBarTop;
                }
                else
                {
                    mainTexture = Globals.Textures.hpBarNoArmy;
                    sideTexture = Globals.Textures.hpBarNoArmySide;
                    topTexture = Globals.Textures.hpBarNoArmyTop;
                }


                var baseWidth = topTexture.width + 2 * sideTexture.width;
                var baseHeight = sideTexture.height;

                var barAspectRatio = (float)baseHeight / baseWidth;
                var sideAspectRatio = (float)sideTexture.height / sideTexture.width;

                float distFromBounds = 0.1f;

                Vector2 barLocation = distFromBounds * statusDisplay.Size;
                float barWidth = (1 - 2 * distFromBounds) * statusDisplay.Width;
                float barHeight = barWidth * barAspectRatio;
                float borderThickness = barHeight / sideAspectRatio;

                float ratio = hitPoints.Value / hitPoints.MaxValue;
                float contentBarWidth = ratio * barWidth;
                float mainWidth = contentBarWidth - 2 * borderThickness;
                float mainHeight = barHeight - 2 * borderThickness;

                Vector2 topLocation = barLocation + Vector2.right * borderThickness;
                Vector2 mainLocation = topLocation + Vector2.up * borderThickness;
                Vector2 bottomLocation = mainLocation + Vector2.up * mainHeight;
                Vector2 rightSideLocation = topLocation + Vector2.right * mainWidth;

                Vector2 barSize = new Vector2(barWidth, barHeight);
                Vector2 sideSize = new Vector2(borderThickness, barHeight);
                Vector2 topSize = new Vector2(mainWidth, borderThickness);
                Vector2 mainSize = new Vector2(mainWidth, mainHeight);
                Vector2 leftSideSize = sideSize;
                float sideToFullRatio = borderThickness / barWidth;
                if (ratio < sideToFullRatio)
                    leftSideSize = new Vector2(ratio * barWidth, barHeight);

                GUI.DrawTexture(new Rect(barLocation, barSize), Globals.Textures.hpBarBackground);
                GUI.DrawTexture(new Rect(barLocation, leftSideSize), sideTexture);
                if (mainWidth > 0)
                {
                    GUI.DrawTexture(new Rect(topLocation, topSize), topTexture);
                    GUI.DrawTexture(new Rect(mainLocation, mainSize), mainTexture);
                    GUI.DrawTexture(new Rect(bottomLocation, topSize), topTexture);
                }
                if (rightSideLocation.x > barLocation.x)
                    GUI.DrawTexture(new Rect(rightSideLocation, sideSize), sideTexture);
                GUI.EndGroup();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", mapElementName ?? "", id);
        }

        public virtual StringBuilder TEMP_PrintStatus()
        {
            var sb = new StringBuilder()
                .AppendLine(string.Format("{0} {1}", GetType().Name, ToString()))
                .AppendLine(string.Format("Coords: {0}", Coords))
                .AppendLine(string.Format("Army: {0}", army == null ? "NONE" : army.armyName))
                .AppendLine(string.Format("Can attack: {0}", canAttack))
                .AppendLine(string.Format("Resource value: {0} + {1}", resourceValue, additionalResourceValue))
                .AppendLine(string.Format("Stats ({0}):", Stats.Count));
            if (Stats.Count == 0)
                sb.Append("    ---");
            int i = 0;
            foreach (var kv in Stats)
            {
                var stat = kv.Value;
                string line;
                if (stat.Limited)
                    line = string.Format("    {0}: {1} / {2} ({3:P1})", stat.Name, stat.Value, stat.MaxValue, stat.Value / stat.MaxValue);
                else line = string.Format("    {0}: {1}", stat.Name, stat.Value);
                if (i < Stats.Count - 1)
                    sb.AppendLine(line);
                else sb.Append(line);
                i++;
            }
            return sb;
        }
    }
}