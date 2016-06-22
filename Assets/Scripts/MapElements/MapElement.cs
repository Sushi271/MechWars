using MechWars.MapElements.Attacks;
using MechWars.MapElements.Orders;
using MechWars.MapElements.Orders.Actions;
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
        public bool canRotate;
        public float yToAim;
        public float displaySize = 1;
        public float displayYOffset = 0;

        public int resourceValue;
        public int additionalResourceValue;
        public bool generateResourcesOnDeath = true;

        public List<OrderAction> orderActions;

        public bool isShadow;

        bool reservationInitialized;

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

        public float Rotation
        {
            get { return transform.rotation.eulerAngles.y; }
            set
            {
                var ea = transform.rotation.eulerAngles;
                ea.y = value;
                transform.rotation = Quaternion.Euler(ea);
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

        public virtual OrderExecutor OrderExecutor { get; private set; }

        protected virtual bool CanAddToArmy { get { return false; } }
        public virtual bool Selectable { get { return false; } }
        public bool CanAttack { get { return orderActions.Any(oa => oa.IsAttack); } }
        public bool CanEscort { get { return orderActions.Any(oa => oa.IsEscort); } }
        public virtual bool CanBeAttacked { get { return false; } }
        public virtual bool CanBeEscorted { get { return false; } }

        public event LifeEndingEventHandler LifeEnding;

        public MapElement()
        {
            if (isShadow) return;
            
            Stats = new Stats(this);
            alive = true;

            OrderExecutor = CreateOrderExecutor();
        }

        protected virtual OrderExecutor CreateOrderExecutor(bool enabled = true)
        {
            return new OrderExecutor(() => new IdleOrder(this), enabled);
        }

        void Start()
        {
            if (isShadow) return;

            OnStart();
        }

        protected virtual void OnStart()
        {
            id = NewId;

            Globals.MapElementsDatabase.Insert(this);
            if (army != null && CanAddToArmy)
                army.AddMapElement(this);

            ReadStats();

            UpdateDying();
            UpdateAlive();

            InitializeReservation();
            InitializeMinimapMarker();
        }

        void InitializeMinimapMarker()
        {
            // 1. get marker image from this Unit's army
            var markerImage = GetMarkerImage();
            if (markerImage == null) return;
            // 2. get Globals -> Prefabs -> marker
            var markerPrefab = Globals.Prefabs.marker;
            // 3. instantiate gameobject from prefab
            var marker = Instantiate(markerPrefab);
            // 4. get SpriteRenderer Component from newly instantiated gameobject
            var sr = marker.GetComponent<SpriteRenderer>();
            // 5. assign marget image to SpriteRenderer Component's "sprite" field
            sr.sprite = markerImage;
            // 6. set marker's size according to its shape's size & H position
            marker.transform.localScale *= Mathf.Max(Shape.Width, Shape.Height);
            var pos = marker.transform.localPosition;
            pos.y = GetMarkerHeight();
            marker.transform.localPosition = pos;

            // 7. set this Unit as parent for newly instantiated gameobject
            marker.transform.SetParent(this.gameObject.transform, false);
        }

        protected virtual Sprite GetMarkerImage()
        {
            return null;
        }

        protected virtual float GetMarkerHeight()
        {
            return 0;
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

        public bool HasMapElementInRange(MapElement other, string rangeStatName)
        {
            var position = other.GetClosestFieldTo(Coords);
            return HasPositionInRange(position, rangeStatName);
        }

        public IVector2 GetClosestFieldTo(Vector2 position)
        {
            float drLen = Mathf.Infinity;
            IVector2? target = null;
            foreach (var c in AllCoords)
            {
                var dr = position - c;
                var len = dr.magnitude;
                if (len < drLen)
                    drLen = len;
                target = c;
            }
            if (target == null)
                throw new System.Exception(string.Format(
                    "Cannot get closest aim target - AllCoords returns empty list ({0}).", this));
            return target.Value;
        }

        public bool HasPositionInRange(Vector2 position, string rangeStatName)
        {
            var range = Stats[rangeStatName];
            if (range == null) return false;

            var dr = position - Coords;
            return Mathf.Abs(dr.x) <= 1 && Mathf.Abs(dr.y) <= 1 ||
                dr.magnitude <= range.Value;
        }

        public Resource PickClosestResourceInRange(string rangeStatName)
        {
            return PickClosestMapElementInRange<Resource>(rangeStatName, r => r.value > 0);
        }

        public MapElement PickClosestEnemyInRange(string rangeStatName)
        {
            return PickClosestMapElementInRange<MapElement>(rangeStatName, me => me.army != null && me.army != army && !me.Dying);
        }

        public T PickClosestMapElementInRange<T>(string rangeStatName)
            where T : MapElement
        {
            return PickClosestMapElementInRange<T>(rangeStatName, me => true);
        }

        public T PickClosestMapElementInRange<T>(string rangeStatName, System.Func<T, bool> predicate)
            where T : MapElement
        {
            var range = Stats[rangeStatName];
            if (range == null) return null;

            var mapElements =
                from c in CoordsInRangeSquare(range.Value)
                where Vector2.Distance(c, Coords) <= range.Value
                let me = Globals.FieldReservationMap[c]
                where me!= null && me is T
                let tme = (T)me
                where predicate(tme)
                where HasMapElementInRange(tme, rangeStatName)
                select tme;
            return PickClosestMapElementFrom(mapElements);
        }

        public T PickClosestMapElementFrom<T>(IEnumerable<T> mapElements)
            where T : MapElement
        {
            if (mapElements.Empty()) return null;
            return mapElements.SelectMin(me => Vector2.Distance(Coords, me.GetClosestFieldTo(Coords)));
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

        public Attack PickAttack()
        {
            var attacks = GetComponents<Attack>();
            int idx = Random.Range(0, attacks.Length);
            return attacks[idx];
        }

        void Update()
        {
            if (isShadow) return;

            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
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

            if (OrderExecutor.Enabled)
                OrderExecutor.Update();

            UpdateDying();
            UpdateAlive();
        }

        protected void UpdateDying()
        {
            if (!Dying && LifeValue == 0)
            {
                Dying = true;
                OrderExecutor.CancelAll();
            }
        }

        protected virtual void UpdateAlive()
        {
            if (!Dying || !Alive) return;
            Alive = OrderExecutor.OrderCount == 0;
        }

        protected virtual void OnLifeEnd()
        {
            if (LifeEnding != null)
                LifeEnding(this);

            if (army != null && CanAddToArmy)
                army.RemoveMapElement(this);

            if (!Globals.Destroyed)
            {
                Globals.FieldReservationMap.ReleaseReservations(this);
                Globals.MapElementsDatabase.Delete(this);
            }

            if (!suspendDestroy) Destroy(gameObject);

            TurnIntoResource();

            if (OrderExecutor.Enabled)
                OrderExecutor.Terminate();
        }

        void TurnIntoResource()
        {
            if (Globals.Destroyed || !generateResourcesOnDeath || isShadow) return;

            int resVal = resourceValue + additionalResourceValue;
            if (resVal == 0) return;

            var noArmy = Globals.MapSettings.armyObjects
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
            if (isShadow) return;

            suspendDestroy = true;
            if (!Dying) Dying = true;
            if (Alive) Alive = false;
            suspendDestroy = false;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", mapElementName ?? "", id);
        }

        public virtual StringBuilder DEBUG_PrintStatus(StringBuilder sb)
        {
            sb
                .AppendLine(string.Format("{0} {1}", GetType().Name, ToString()))
                .AppendLine(string.Format("Coords: {0}", Coords))
                .AppendLine(string.Format("Army: {0}", army == null ? "NONE" : army.armyName))
                .AppendLine(string.Format("Can attack: {0}", CanAttack))
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

        protected StringBuilder DEBUG_PrintOrders(StringBuilder sb)
        {
            sb
                .AppendLine("Default order:")
                .AppendLine(string.Format("    {0}",
                    OrderExecutor.DefaultOrder == null ? "---" :
                    OrderExecutor.DefaultOrder.ToString()))
                .AppendLine("Order queue:");
            if (OrderExecutor.OrderCount == 0)
                sb.Append("    ---");
            else for (int i = 0; i < OrderExecutor.OrderCount; i++)
                {
                    string line = string.Format("{0}: {1}", i, OrderExecutor[i]);
                    if (i < OrderExecutor.OrderCount - 1)
                        sb.AppendLine(line);
                    else sb.Append(line);
                }
            return sb;
        }
    }
}