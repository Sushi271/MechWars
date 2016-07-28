using MechWars.GLRendering;
using MechWars.Pathfinding;
using UnityEngine;
using MechWars.MapElements.WallNeighbourhoods;
using System.Collections.Generic;
using MechWars.Utils;
using MechWars.Mapping;
using MechWars.FogOfWar;

namespace MechWars
{
    public class Globals : MonoBehaviour
    {
        public static bool Destroyed { get; private set; }

        static Globals instance;
        public static Globals Instance
        {
            get
            {
                if (Destroyed)
                {
                    instance = null;
                    return null;
                }
                if (instance == null)
                {
                    var gameObject = GameObject.FindGameObjectWithTag(Tag.Globals);
                    if (gameObject != null)
                        instance = gameObject.GetComponent<Globals>();
                }
                return instance;
            }
        }

        //=====================================================================================

        public bool debugComplexOrderStrings;
        public float aggroScanInterval = 1;

        List<Counter> DEBUG_counters;
        public static List<Counter> DEBUG_Counters { get { return Instance.DEBUG_counters; } }
        
        public static Spectator Spectator { get { return Destroyed ? null : MapSettings.spectator; } }
        public static Player HumanPlayer { get { return Spectator == null ? null : Spectator.player; } }
        public static Army HumanArmy { get { return HumanPlayer == null ? null : HumanPlayer.army; } }

        public static IEnumerable<Army> Armies
        {
            get
            {
                foreach (var p in MapSettings.players)
                    yield return p.army;
            }
        }

        public static GameObject MainCameraObject { get { return GameObject.FindGameObjectWithTag(Tag.MainCamera); } }
        public static GameObject GUICameraObject { get { return GameObject.FindGameObjectWithTag(Tag.GUICamera); } }
        public static Camera MainCamera { get { return MainCameraObject.GetComponent<Camera>(); } }

        public static GLRenderer GLRenderer
        {
            get
            {
                if (GUICameraObject == null)
                    throw new System.Exception(string.Format("No object with tag \"{0}\" found.", Tag.GUICamera));
                return GUICameraObject.GetComponent<GLRenderer>();
            }
        }

        static MapSettings mapSettings;
        public static MapSettings MapSettings
        {
            get
            {
                return TryLazyGetGlobalsComponent(ref mapSettings);
            }
        }

        static Map map;
        public static Map Map
        {
            get
            {
                return TryLazyGetGlobalsComponent(ref map);
            }
        }

        static Textures textures;
        public static Textures Textures
        {
            get
            {
                return TryLazyGetGlobalsComponent(ref textures);
            }
        }

        static Prefabs prefabs;
        public static Prefabs Prefabs
        {
            get
            {
                return TryLazyGetGlobalsComponent(ref prefabs);
            }
        }

        static WallNeighbourhoodDictionary wallNeighbourhoodDictionary;
        public static WallNeighbourhoodDictionary WallNeighbourhoodDictionary
        {
            get
            {
                return TryLazyGetGlobalsComponent(ref wallNeighbourhoodDictionary);
            }
        }
        
        ShapeDatabase shapeDatabase;
        public static ShapeDatabase ShapeDatabase { get { return LazyGetGlobalsField(
            ref Instance.shapeDatabase, o => new ShapeDatabase()); } }
        
        LOSShapeDatabase losShapeDatabase;
        public static LOSShapeDatabase LOSShapeDatabase { get { return LazyGetGlobalsField(
            ref Instance.losShapeDatabase, o => new LOSShapeDatabase()); } }

        //===== PRIVATE =======================================================================

        void Start()
        {
            Destroyed = false;
            DEBUG_counters = new List<Counter>();
            DEBUG_counters.Add(new Counter());
        }

        void OnDestroy()
        {
            Destroyed = true;
        }

        static T TryLazyGetGlobalsComponent<T>(ref T field)
            where T : class
        {
            if (field == null)
            {
                field = Instance.GetComponent<T>();

                if (field == null)
                    throw new System.Exception("Globals object does not have " + typeof(T).Name + " component.");
            }
            return field;
        }

        static T LazyGetGlobalsField<T>(ref T field, System.Func<object, T> ctor, object args = null)
        {
            if (field == null)
                field = ctor(args);
            return field;
        }
    }
}