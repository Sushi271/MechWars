using System.Collections.Generic;
using UnityEngine;

namespace MechWars.Mapping
{
    public class QuadTreeMap : MonoBehaviour
    {
        public QuadTree ResourcesQuadTree { get; private set; }
        public Dictionary<Army, QuadTree> ArmyQuadTrees { get; private set; }

        void Start()
        {
            ResourcesQuadTree = Globals.Map.CreateQuadTree();

            ArmyQuadTrees = new Dictionary<Army, QuadTree>();
            var players = Globals.MapSettings.players;
            foreach (var p in players)
            {
                var army = p.army;
                if (army == null) continue;
                ArmyQuadTrees[army] = Globals.Map.CreateQuadTree();
            }
        }
    }
}
