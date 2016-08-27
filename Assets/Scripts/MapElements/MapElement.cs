using MechWars.FogOfWar;
using MechWars.MapElements.Attacks;
using MechWars.MapElements.Orders;
using MechWars.MapElements.Orders.Actions;
using MechWars.MapElements.Statistics;
using MechWars.Mapping;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace MechWars.MapElements
{
    public class MapElement : MonoBehaviour, IRotatable
    {
        #region Fields & Properties
        
        public string mapElementName;
        public int id;

        public Army nextArmy;
        public Army Army { get; private set; }

        public TextAsset shapeFile;
        public TextAsset statsFile;
        public float displaySize = 1;
        public float displayYOffset = 0;

        public List<GameObject> aims;

        public int resourceValue;
        public int additionalResourceValue;
        public bool generateResourcesOnDeath = true;

        public AttackHead attackHead;

        public List<OrderAction> orderActions;

        public bool isShadow;

        public MapElement OriginalMapElement { get; private set; } // correct this shit for IsNull
        MapElementGhostSnapshot ghostSnapshot;
        public bool IsGhost { get; private set; }
        protected Army ObservingArmy { get; private set; }
        bool addGhostToQuadTree;
        public bool GhostRemoved { get; private set; }
        public Dictionary<Army, MapElement> Ghosts { get; private set; }

        bool mapInitialized;

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

        // Can be called on Ghost
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

        // Can be called on Ghost
        public IEnumerable<IVector2> AllCoords
        {
            get
            {
                if (Shape == null) yield return Coords.Round();
                else
                {
                    var list = new List<IVector2>();
                    int xFrom = Mathf.RoundToInt(Coords.x + Shape.DeltaXNeg);
                    int xTo = Mathf.RoundToInt(Coords.x + Shape.DeltaXPos);
                    int yFrom = Mathf.RoundToInt(Coords.y + Shape.DeltaYNeg);
                    int yTo = Mathf.RoundToInt(Coords.y + Shape.DeltaYPos);
                    for (int x = xFrom, i = 0; x <= xTo; x++, i++)
                        for (int y = yFrom, j = 0; y <= yTo; y++, j++)
                            if (Shape[i, j])
                                yield return new IVector2(x, y);
                }
            }
        }

        // Can be called on Ghost
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

        // Can be called on Ghost
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

        public OrderQueue OrderQueue { get; private set; }

        protected virtual bool CanAddToArmy { get { return false; } }
        public virtual bool Selectable { get { return false; } }

        bool visibleToSpectator;
        public bool VisibleToSpectator
        {
            get { return visibleToSpectator; }
            private set
            {
                if (visibleToSpectator == value) return;

                visibleToSpectator = value;

                var renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers) r.enabled = value;

                if (!value && Selectable)
                {
                    var selMonitor = Globals.Spectator.InputController.SelectionMonitor;
                    if (selMonitor.IsSelected(this))
                    {
                        selMonitor.Deselect(this.AsEnumerable());
                        if (CanHaveGhosts && !IsGhost)
                            selMonitor.Select(Ghosts.Values.Where(g => g != null));
                    }
                }
            }
        }
        public virtual bool CanHaveGhosts { get { return true; } }
        public Dictionary<Army, bool> VisibleToArmies { get; private set; }

        public bool CanAttack { get { return orderActions.Any(oa => oa.IsAttack); } }
        public bool CanEscort { get { return orderActions.Any(oa => oa.IsEscort); } }
        public virtual bool CanBeAttacked { get { return false; } }
        public virtual bool CanBeEscorted { get { return false; } }
        public virtual bool CanRotateItself { get { return false; } }

        public Attack ReadiedAttack { get; private set; }
        public float AttackCooldown { get; private set; }

        public event LifeEndingEventHandler LifeEnding;
        public event LifeEndingEventHandler GhostLifeEnding;

        public float HeadPitch
        {
            get
            {
                var vertical = attackHead as VerticalAttackHead;
                if (vertical == null) return 0;
                return vertical.Pitch;
            }
            set
            {
                var vertical = attackHead as VerticalAttackHead;
                if (vertical == null)
                    throw new System.InvalidOperationException("Cannot set HeadPitch - AttackHead must be Vertical.");
                vertical.Pitch = value;
            }
        }

        #endregion

        #region Initializing

        public void MakeItGhost(MapElement originalMapElement, Army observingArmy)
        {
            IsGhost = true;
            OriginalMapElement = originalMapElement;
            MakeSnapshotOf(originalMapElement);
            ObservingArmy = observingArmy;
            addGhostToQuadTree = true;
        }

        protected virtual void MakeSnapshotOf(MapElement originalMapElement)
        {
            ghostSnapshot = new MapElementGhostSnapshot(originalMapElement, this);
        }

        void Start()
        {
            visibleToSpectator = true;
            if (isShadow) return;
            OnStart();
        }

        protected virtual void OnStart()
        {
            alive = true;

            if (!IsGhost)
            {
                id = NewId;

                OrderQueue = CreateOrderQueue();

                ReadStats();

                if (nextArmy != null)
                    UpdateArmy();

                UpdateDying();
                UpdateAlive();

                InitializeMap();
                InitializeMinimapMarker();

                VisibleToSpectator = false;
                VisibleToArmies = new Dictionary<Army, bool>();
                foreach (var a in Globals.Armies)
                    VisibleToArmies[a] = false;

                if (CanHaveGhosts)
                {
                    Ghosts = new Dictionary<Army, MapElement>();
                    foreach (var a in Globals.Armies)
                        Ghosts[a] = null;
                }
            }
            else
            {
                // Ghost version
                Stats = ghostSnapshot.Stats;
                nextArmy = Army = ghostSnapshot.Army;

                Globals.Map.AddGhost(this);

                VisibleToSpectator = ObservingArmy.actionsVisible;
                VisibleToArmies = new Dictionary<Army, bool>();
                foreach (var a in Globals.Armies)
                {
                    VisibleToArmies[a] = a == ObservingArmy;
                }
            }
        }

        protected virtual OrderQueue CreateOrderQueue(bool enabled = true)
        {
            return new OrderQueue(() => new IdleOrder(this), enabled);
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
            // 5. assign marker image to SpriteRenderer Component's "sprite" field
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

        public void ReadStats()
        {
            if (statsRead) return;
            statsRead = true;
            Stats = new Stats();

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
                Stat attribute = new Stat(name, this, maxValue, value, limited);
                Stats.Add(attribute);
            }
        }

        public void InitializeMap()
        {
            if (mapInitialized) return;

            var occupiedFields = AllCoords;
            foreach (var coord in occupiedFields)
                Globals.Map.MakeReservation(this, coord);

            mapInitialized = true;
        }

        protected virtual void InitializeInVisibilityTable()
        {
        }

        protected virtual void FinalizeInVisibilityTable()
        {
        }

        #endregion

        #region Range checking (for Orders)

        // Can be called on Ghost
        public IVector2 GetClosestFieldTo(Vector2 position)
        {
            float drLenSq = Mathf.Infinity;
            IVector2? target = null;
            foreach (var c in AllCoords)
            {
                var dr = position - c;
                var lenSq = dr.sqrMagnitude;
                if (lenSq < drLenSq)
                {
                    drLenSq = lenSq;
                    target = c;
                }
            }
            if (target == null)
                throw new System.Exception(string.Format(
                    "Cannot get closest field - AllCoords returns empty list ({0}).", this));
            return target.Value;
        }

        // Can be called on Ghost
        public Vector3 GetClosestAimTo(Vector2 position)
        {
            float drLenSq = Mathf.Infinity;
            Vector3? target = null;
            if (aims.Empty())
                throw new System.Exception(string.Format(
                    "Cannot get closest aim - aims list is empty ({0}).", this));
            foreach (var a in aims)
            {
                var aPos = a.transform.position;
                var dr = position - aPos.AsHorizontalVector2();
                var lenSq = dr.sqrMagnitude;
                if (lenSq < drLenSq)
                {
                    drLenSq = lenSq;
                    target = aPos;
                }
            }
            return target.Value;
        }

        public bool HasMapElementInRange(MapElement other, string rangeStatName)
        {
            var position = other.GetClosestAimTo(Coords).AsHorizontalVector2();
            return HasPositionInRange(position, rangeStatName);
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
            return (Resource)PickClosestMapElementInRange<Resource>(rangeStatName, Army.ResourcesQuadTree);
        }

        public MapElement PickClosestEnemyInRange(string rangeStatName, bool onlyAggressive = false)
        {
            return PickClosestMapElementInRange<MapElement>(rangeStatName, Army.EnemiesQuadTree, onlyAggressive);
        }

        MapElement PickClosestMapElementInRange<T>(string rangeStatName, QuadTree searchedQuadTree, bool onlyAggressive = false)
            where T : MapElement
        {
            var rangeStat = Stats[rangeStatName];
            if (rangeStat == null) return null;
            var range = rangeStat.Value;

            var roundRange = Mathf.RoundToInt(range);
            var bounds = new SquareBounds(Coords.Round() - new IVector2(roundRange, roundRange), roundRange * 2 + 1);
            var mapElements = (
                from qtme in searchedQuadTree.QueryRange(bounds)
                where !qtme.MapElement.Dying
                where !onlyAggressive || qtme.MapElement.CanAttack
                select qtme.MapElement)
                .Distinct();

            if (mapElements.Empty()) return null;
            var closest = mapElements.SelectMin(me => Vector2.SqrMagnitude(Coords - me.GetClosestFieldTo(Coords)));
            if (HasMapElementInRange(closest, rangeStatName))
                return (T)closest;
            return null;
        }

        public MapElement PickClosestMapElementFrom<T>(IEnumerable<T> mapElements)
            where T : MapElement
        {
            if (mapElements.Empty()) return null;
            return mapElements.SelectMin(me => Vector2.SqrMagnitude(Coords - me.GetClosestFieldTo(Coords)));
        }

        #endregion

        #region Attacking

        public void ReadyAttack()
        {
            var attacks = GetComponents<Attack>();
            int idx = Random.Range(0, attacks.Length);
            ReadiedAttack = attacks[idx];
        }

        AttackAnimation currentAttackAnimation;
        System.Action attackFinishCallback;

        public void MakeAttack(MapElement target, Vector3 aim, System.Action attackFinishCallback = null)
        {
            if (ReadiedAttack == null)
                throw new System.Exception("Cannot call MakeAttack before calling ReadyAttack.");
            if (AttackCooldown > 0)
                throw new System.Exception("Cannot call MakeAttack if AttackCooldown > 0.");

            currentAttackAnimation = new AttackAnimation(ReadiedAttack);
            currentAttackAnimation.Execute += () => ReadiedAttack.Execute(this, target, aim);

            this.attackFinishCallback = attackFinishCallback;

            var attackSpeedStat = Stats[StatNames.AttackSpeed];
            if (attackSpeedStat == null)
                AttackCooldown = 1;
            else AttackCooldown = 1 / attackSpeedStat.Value;
        }

        #endregion

        #region Updating

        void Update()
        {
            if (isShadow) return;

            OnUpdate();
        }

        bool firstUpdate = true;
        protected virtual void OnUpdate()
        {
            if (!IsGhost)
            {
                if (Army != nextArmy)
                    UpdateArmy();

                if (CanHaveGhosts) UpdateGhosts();
                UpdateArmiesQuadTrees();
                if (CanHaveGhosts) AddGhostsToQuadTrees();

                UpdateVisibilityToSpectator();

                if (CanAttack)
                    UpdateAttack();

                if (OrderQueue.Enabled)
                    OrderQueue.Update();

                if (firstUpdate)
                    foreach (var kv in VisibleToArmies)
                        if (kv.Value)
                            kv.Key.InvokeOnVisibleMapElementCreated(this);

                UpdateDying();
                UpdateAlive();
            }
            else
            {
                // Ghost version
                VisibleToSpectator = ObservingArmy.actionsVisible;
                var ghostFogged = Globals.Map.GetGhostPositions(this) // none are visible but at least one is fogged
                    .Where(c => ObservingArmy.VisibilityTable[c.X, c.Y] != Visibility.Visible)
                    .Any(c => ObservingArmy.VisibilityTable[c.X, c.Y] == Visibility.Fogged);
                if (!ghostFogged)
                    RemoveGhost();
            }
            firstUpdate = false;
        }

        void UpdateArmy()
        {
            if (CanAddToArmy)
            {
                if (Army != null)
                {
                    FinalizeInVisibilityTable();
                    Army.RemoveMapElement(this);
                }
                Army = nextArmy;
                if (Army != null)
                {
                    Army.AddMapElement(this);
                    InitializeInVisibilityTable();
                }
            }
        }

        void UpdateVisibilityToSpectator()
        {
            VisibleToSpectator = Globals.Armies
                .Where(a => a.actionsVisible)
                .Any(a => AllCoords
                    .Any(c => a.VisibilityTable[c.X, c.Y] == Visibility.Visible));
        }

        void UpdateGhosts()
        {
            foreach (var a in Globals.Armies)
            {
                var allCoords = Globals.Map[this];
                var isVisible = allCoords.Any(c => a.VisibilityTable[c.X, c.Y] == Visibility.Visible);
                var isFogged = !isVisible && allCoords.Any(c => a.VisibilityTable[c.X, c.Y] == Visibility.Fogged);

                if (isVisible != VisibleToArmies[a] && isFogged && Ghosts[a] == null)
                {
                    var ghostObject = Instantiate(gameObject);
                    ghostObject.transform.parent = transform.parent;
                    ghostObject.name = string.Format("{0} [GHOST]", name);
                    var ghost = ghostObject.GetComponent<MapElement>();
                    ghost.MakeItGhost(this, a);
                    ghost.GhostLifeEnding += Ghost_GhostLifeEnding;
                    Ghosts[a] = ghost;
                    Globals.Map.AddGhost(ghost);
                }
                else if (isVisible && Ghosts[a] != null)
                    Ghosts[a].RemoveGhost();
            }
        }

        void AddGhostsToQuadTrees()
        {
            foreach (var g in Ghosts.Values)
                if (g != null && g.addGhostToQuadTree)
                    g.AddGhostToQuadTree();
        }

        private void Ghost_GhostLifeEnding(MapElement ghost)
        {
            ghost.GhostLifeEnding -= Ghost_GhostLifeEnding;
            Ghosts[ghost.ObservingArmy] = null;

            if (Selectable)
            {
                var selMonitor = Globals.Spectator.InputController.SelectionMonitor;
                if (selMonitor.IsSelected(ghost))
                    selMonitor.Select(this.AsEnumerable());
            }
        }

        protected virtual void UpdateArmiesQuadTrees()
        {
            var coordsList = Globals.Map[this];
            foreach (var a in Globals.Armies)
            {
                var visible = coordsList.Any(c => a.VisibilityTable[c.X, c.Y] == Visibility.Visible);
                if (visible != VisibleToArmies[a])
                    VisibleToArmies[a] = visible;
            }
        }

        void UpdateAttack()
        {
            if (currentAttackAnimation == null && AttackCooldown > 0)
            {
                AttackCooldown -= Time.deltaTime;
                AttackCooldown = Mathf.Clamp(AttackCooldown, 0, AttackCooldown);
            }

            if (currentAttackAnimation != null)
            {
                if (!currentAttackAnimation.Playing && !currentAttackAnimation.Finished)
                    currentAttackAnimation.Play();
                currentAttackAnimation.Update();
                if (currentAttackAnimation.Finished)
                {
                    ReadiedAttack = null;
                    currentAttackAnimation = null;
                    if (attackFinishCallback != null)
                        attackFinishCallback();
                }
            }
        }

        void UpdateDying()
        {
            if (!Dying && LifeValue == 0)
            {
                Dying = true;
                OrderQueue.CancelAll();
            }
        }

        void UpdateAlive()
        {
            if (!Dying || !Alive) return;
            Alive = !(OrderQueue.OrderCount == 0);

            if (firstUpdate)
                foreach (var kv in VisibleToArmies)
                    if (kv.Value)
                        kv.Key.InvokeOnVisibleMapElementDied(this);
        }

        void RemoveGhost()
        {
            if (!IsGhost)
                throw new System.InvalidOperationException("Cannot call this method on non-Ghost MapElement.");
            if (!GhostRemoved)
            {
                if (GhostLifeEnding != null)
                    GhostLifeEnding(this);

                GhostRemoved = true;
                RemoveGhostFromQuadTree();

                var selMonitor = Globals.Spectator.InputController.SelectionMonitor;
                if (selMonitor.IsSelected(this))
                    selMonitor.Deselect(this.AsEnumerable());

                Globals.Map.RemoveGhost(this);
                Destroy(gameObject);
            }
        }

        protected virtual void AddGhostToQuadTree()
        {
            addGhostToQuadTree = false;
        }

        protected virtual void RemoveGhostFromQuadTree()
        {
        }

        #endregion

        #region Finalizing

        protected virtual void OnLifeEnd()
        {
            if (LifeEnding != null)
                LifeEnding(this);

            if (Army != null && CanAddToArmy)
                Army.RemoveMapElement(this);

            if (CanHaveGhosts)
                foreach (var g in Ghosts.Values)
                    if (g != null)
                        g.OriginalMapElement = null;

            if (!Globals.Destroyed)
            {
                FinalizeInVisibilityTable();
                RemoveFromQuadTrees();
                Globals.Map.ReleaseReservations(this);
            }

            if (!suspendDestroy) Destroy(gameObject);

            TurnIntoResource();

            if (OrderQueue.Enabled)
                OrderQueue.Terminate();
        }

        protected virtual void RemoveFromQuadTrees()
        {
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
                resource.InitializeMap();
            }
        }

        bool suspendDestroy;
        void OnDestroy()
        {
            if (!isShadow && !IsGhost)
            {
                suspendDestroy = true;
                if (!Dying) Dying = true;
                if (Alive) Alive = false;
                suspendDestroy = false;
            }
        }

        #endregion

        #region Debug

        public override string ToString()
        {
            return string.Format("{0} ({1}){2}", mapElementName ?? "", id, IsGhost ? " [GHOST]" : "");
        }

        public virtual StringBuilder DEBUG_PrintStatus(StringBuilder sb)
        {
            if (Dying) return sb;
            sb
                .AppendLine(string.Format("{0} {1}", GetType().Name, ToString()))
                .AppendLine(string.Format("Coords: {0}", Coords))
                .AppendLine(string.Format("Army: {0}", Army == null ? "NONE" : Army.armyName))
                .AppendLine(string.Format("Can attack: {0}", CanAttack))
                .AppendLine(string.Format("Resource value: {0} + {1}", resourceValue, additionalResourceValue));
            if (Stats == null || Stats.Count == 0)
                sb.Append("    ---");
            else
            {
                sb.AppendLine(string.Format("Stats ({0}):", Stats.Count));

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
            }
            return sb;
        }

        protected StringBuilder DEBUG_PrintOrders(StringBuilder sb)
        {
            sb
                .AppendLine("Default order:")
                .AppendLine(string.Format("    {0}",
                    OrderQueue.DefaultOrder == null ? "---" :
                    OrderQueue.DefaultOrder.ToString()))
                .AppendLine("Order queue:");
            if (OrderQueue.OrderCount == 0)
                sb.Append("    ---");
            else for (int i = 0; i < OrderQueue.OrderCount; i++)
                {
                    string line = string.Format("{0}: {1}", i, OrderQueue[i]);
                    if (i < OrderQueue.OrderCount - 1)
                        sb.AppendLine(line);
                    else sb.Append(line);
                }
            return sb;
        }

        #endregion
    }
}