using MechWars.MapElements.Orders.Actions;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.AI
{
    public class CreationMethodDictionary
    {
        Dictionary<IKind, CreationMethod> dict;

        public CreationMethod this[IKind created]
        {
            get { return dict[created]; }
        }

        public CreationMethodDictionary(AIBrain brain)
        {
            dict = new Dictionary<IKind, CreationMethod>();

            InitializeDictionary(brain);
        }

        void InitializeDictionary(AIBrain brain)
        {
            var knowledge = brain.MainAgent.Knowledge;
            var meKinds = knowledge.MapElementKinds;
            var techKinds = knowledge.TechnologyKinds;

            // --- CONST YARD ---

            var constYard = brain.MapElementPrefabList[AIName.ConstructionYard];
            var constructionOAs = constYard.orderActions.OfType<BuildingConstructionOrderAction>();

            var refineryOA = constructionOAs.First(oa => oa.Building.mapElementName == AIName.Refinery);
            var refineryCM = new CreationMethod(meKinds[AIName.Refinery],
                meKinds[AIName.ConstructionYard], refineryOA.Cost, refineryOA.StartCost, refineryOA.ProductionTime);
            Add(refineryCM);

            var factoryOA = constructionOAs.First(oa => oa.Building.mapElementName == AIName.Factory);
            var factoryCM = new CreationMethod(meKinds[AIName.Factory],
                meKinds[AIName.ConstructionYard], factoryOA.Cost, factoryOA.StartCost, factoryOA.ProductionTime);
            factoryCM.BuildingRequirements.Add(meKinds[AIName.Refinery]);
            Add(factoryCM);

            // --- FACTORY ---

            var factory = brain.MapElementPrefabList[AIName.Factory];
            var productionOAs = factory.orderActions.OfType<UnitProductionOrderAction>();
            
            var scoutOA = productionOAs.First(oa => oa.unit.mapElementName == AIName.Scout);
            var scoutCM = new CreationMethod(meKinds[AIName.Scout],
                meKinds[AIName.Factory], scoutOA.cost, 0, scoutOA.productionTime);
            Add(scoutCM);

            var harvesterOA = productionOAs.First(oa => oa.unit.mapElementName == AIName.Harvester);
            var harvesterCM = new CreationMethod(meKinds[AIName.Harvester],
                meKinds[AIName.Factory], harvesterOA.cost, 0, harvesterOA.productionTime);
            Add(harvesterCM);
        }

        void Add(CreationMethod method)
        {
            dict.Add(method.Created, method);
            method.Created.CreationMethods.Add(method);
        }
    }
}