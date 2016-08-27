using MechWars.AI.Agents;
using System.Collections.Generic;
using UnityEngine;
using MechWars.MapElements;

namespace MechWars.AI
{
    public class AIBrain : MonoBehaviour
    {
        public Player player;

        public float resourceRegionDistance = 2;

        HashSet<Agent> agentsToAdd;
        HashSet<Agent> agents;

        public MainAgent MainAgent { get; private set; }
        public FilteringMapProxy MapProxy { get; private set; }

        public MapElementSurroundingShape ResourceRegionDetectionShape { get; private set; }

        public MapElementPrefabList MapElementPrefabList { get { return GetComponent<MapElementPrefabList>(); } }

        void Start()
        {
            agentsToAdd = new HashSet<Agent>();
            agents = new HashSet<Agent>();

            MainAgent = new MainAgent(this);
            MapProxy = new FilteringMapProxy(player.army);

            InitializeResourceRegionDetectionShape();
        }

        void InitializeResourceRegionDetectionShape()
        {
            if (resourceRegionDistance < 0)
                throw new System.Exception("Parameter resourceRegionDistance cannot be < 0.");

            ResourceRegionDetectionShape = new MapElementSurroundingShape(
                resourceRegionDistance, MapElementShape.DefaultShape);
        }

        void Update()
        {

            agents.UnionWith(agentsToAdd);
            agentsToAdd.Clear();
            foreach (var a in agents)
            {
                if (!a.Started) a.Start();
                a.Update();
            }
            agents.RemoveWhere(a => a.Finished);
        }

        public void AddAgent(Agent agent)
        {
            agentsToAdd.Add(agent);
        }
    }
}