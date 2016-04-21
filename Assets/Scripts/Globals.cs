using System.Collections.Generic;
using System.Linq;
using MechWars.GLRendering;
using MechWars.Pathfinding;
using UnityEngine;

namespace MechWars
{
    public class Globals : MonoBehaviour
    {
        static Globals instance;
        public static Globals Instance
        {
            get
            {
                if (instance == null)
                {
                    var gameObject = GameObject.FindGameObjectWithTag("Globals");
                    if (gameObject != null)
                        instance = gameObject.GetComponent<Globals>();
                }
                return instance;
            }
        }

        //=====================================================================================

        public bool isGameplay;
        public bool debugStatusDisplays;

        public int mapWidth = 64;
        public int mapHeight = 64;
        
        public List<GameObject> sortedPlayers;
        public List<GameObject> sortedArmies;

        public float dayAndNightCycleTime;

        public Globals()
        {
            sortedPlayers = new List<GameObject>();
            sortedArmies = new List<GameObject>();
        }

        public int MapWidth
        {
            get
            {
                if (mapWidth <= 0)
                    mapWidth = 1;
                return mapWidth;
            }
        }

        public int MapHeight
        {
            get
            {
                if (mapHeight <= 0)
                    mapHeight = 1;
                return mapHeight;
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

        static FieldReservationMap fieldReservationMap;
        public static FieldReservationMap FieldReservationMap
        {
            get
            {
                return TryLazyGetGlobalsComponent(ref fieldReservationMap);
            }
        }

        static Materials materials;
        public static Materials Materials
        {
            get
            {
                return TryLazyGetGlobalsComponent(ref materials);
            }
        }

        static StatusDisplayer statusDisplayer;
        public static StatusDisplayer StatusDisplayer
        {
            get
            {
                return TryLazyGetGlobalsComponent(ref statusDisplayer);
            }
        }

        public static Army GetArmyForPlayer(Player player)
        {
            int idx = Globals.Instance.sortedPlayers.IndexOf(player.gameObject);
            if (idx == -1) return null;
            return Globals.Instance.sortedArmies[idx].GetComponent<Army>();
        }

        public static Player GetPlayerForArmy(Army army)
        {
            int idx = Globals.Instance.sortedArmies.IndexOf(army.gameObject);
            if (idx == -1) return null;
            return Globals.Instance.sortedPlayers[idx].GetComponent<Player>();
        }

        //===== PRIVATE =======================================================================

        void Start()
        {
            if (isGameplay)
                CheckPlayerArmyAssignmentCorrectness();
        }

        void CheckPlayerArmyAssignmentCorrectness()
        {
            var players = GameObject.FindGameObjectsWithTag(Tag.Player);
            var notPlayers = players.Where(p => p.GetComponent<Player>() == null);
            if (notPlayers.Count() > 0)
            {
                var gameObjects = string.Join(", ", notPlayers.Select(np => string.Format("\"{0}\"", np.name)).ToArray());
                throw new System.Exception(string.Format(
                    "GameObject with Player Tag must have Player script attached (GameObjects: {0}).", gameObjects));
            }

            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                var player = sortedPlayers[i];
                var army = sortedArmies[i];

                if (player == null)
                    throw new System.Exception("Globals.sortedPlayers List contains a NULL-Player.");
                if (!players.Contains(player))
                    throw new System.Exception(string.Format(
                        "Globals.sortedPlayers List contains a non-Player (GameObject: \"{0}\").", player.name));
                if (army == null)
                    throw new System.Exception(string.Format(
                        "Globals.sortedArmies List contains a NULL-Army for Player \"{0}\".", player.name));
                if (army.GetComponent<Army>() == null)
                    throw new System.Exception(string.Format(
                        "Globals.sortedArmies List contains a non-Army object " +
                        "(Player GameObject: \"{0}\", Value GameObject: \"{1}\").", player.name, army.name));

                var samePlayers = new List<int>();
                var sameArmies = new List<int>();
                for (int j = 0; j < sortedPlayers.Count; j++)
                {
                    if (sortedPlayers[j] == player)
                        samePlayers.Add(j);
                    if (sortedArmies[j] == army)
                        sameArmies.Add(j);
                }
                if (samePlayers.Count > 1)
                    throw new System.Exception(string.Format(
                        "Globals.sortedPlayers List contains a the same Player object {0} times (Player GameObject: \"{1}\").",
                        samePlayers.Count, player.name));
                if (sameArmies.Count > 1)
                    throw new System.Exception(string.Format(
                        "Globals.sortedArmies List contains a the same Army object {0} times (Army GameObject: \"{1}\").",
                        samePlayers.Count, army.name));
            }
        }

        static T TryLazyGetGlobalsComponent<T>(ref T field)
        {
            if (field == null)
            {
                field = Instance.GetComponent<T>();

                if (field == null)
                    throw new System.Exception("Globals object does not have " + typeof(T).Name + " component.");
            }
            return field;
        }
    }
}