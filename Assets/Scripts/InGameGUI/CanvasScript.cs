using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.MapElements.Statistics;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    public class CanvasScript : MonoBehaviour
    {
        public Button buttonPrefab;

        Building building;
        bool refreshBuildingGUI;

        List<Button> buttons;

        public GameObject debugInfo;
        public GameObject debugInfoBack;
        bool debugInfoActive;

        void Start()
        {
            buttons = new List<Button>();
            if (Globals.HumanArmy != null)
                Globals.HumanArmy.OnBuildingConstructionFinished += InvokeRefreshBuildingGUI;
        }

        void Update()
        {
            if (Globals.Spectator == null)
                throw new System.Exception("Globals.Spectator is NULL.");

            var inputController = Globals.Spectator.InputController;
            var selBuilding = inputController.SelectionMonitor.SelectedMapElements.FirstOr(MapElement.Null) as Building;
            if (selBuilding != building || refreshBuildingGUI)
            {
                refreshBuildingGUI = false;

                UnsubscribeEvents(building);
                building = selBuilding;
                SubscribeEvents(building);

                foreach (var b in buttons)
                {
                    b.onClick.RemoveAllListeners();
                    Destroy(b.gameObject);
                }
                buttons.Clear();

                if (building != MapElement.Null && !building.UnderConstruction && !building.IsGhost && building.Army == Globals.HumanArmy)
                {
                    var productionOAs = building.orderActions.OfType<UnitProductionOrderAction>()
                        .Where(poa => poa.CheckRequirements(building.Army)).ToList();
                    var constructionOAs = building.orderActions.OfType<BuildingConstructionOrderAction>()
                        .Where(coa => coa.CheckRequirements(building.Army)).ToList();
                    var developmentOAs = building.orderActions.OfType<TechnologyDevelopmentOrderAction>()
                        .Where(doa => building.Army.Technologies.CanDevelop(doa.technology))
                        .Where(doa => doa.CheckRequirements(building.Army)).ToList();
                    var count = productionOAs.Count + constructionOAs.Count + developmentOAs.Count;

                    float margin = 4;
                    float h = buttonPrefab.GetComponent<RectTransform>().sizeDelta.y;
                    float x = margin;
                    float y = count * (h + margin) + margin;
                    for (int i = 0; i < count; i++, y -= h + margin)
                    {
                        var button = Instantiate(buttonPrefab);
                        var rectTransform = button.GetComponent<RectTransform>();
                        rectTransform.SetParent(transform);
                        rectTransform.position = new Vector3(x, y, 0);
                        var text = button.GetComponent<ButtonScript>().innerText;
                        if (i < productionOAs.Count)
                        {
                            var poa = productionOAs[i];
                            button.name = string.Format("Button {0}", poa.unit.mapElementName);
                            text.text = string.Format("Produce {0} ({1} RP)", poa.unit.mapElementName, poa.cost);
                            button.onClick.AddListener(new UnityAction(() =>
                                poa.GiveOrder(building, inputController.OrderActionArgs)));
                        }
                        else if (i < productionOAs.Count + constructionOAs.Count)
                        {
                            var coa = constructionOAs[i - productionOAs.Count];
                            button.name = string.Format("Button {0}", coa.building.mapElementName);
                            text.text = string.Format("Build {0} ({1} RP)", coa.building.mapElementName, coa.cost);
                            button.onClick.AddListener(new UnityAction(() =>
                            {
                                inputController.CarriedOrderAction = coa;
                                inputController.CreateShadow(coa);
                            }));
                        }
                        else
                        {
                            var doa = developmentOAs[i - productionOAs.Count - constructionOAs.Count];
                            button.name = string.Format("Button {0}", doa.technology.technologyName);
                            text.text = string.Format("Develop {0} ({1} RP)", doa.technology.technologyName, doa.cost);
                            button.onClick.AddListener(new UnityAction(() =>
                                doa.GiveOrder(building, inputController.OrderActionArgs)));
                        }
                        buttons.Add(button);
                    }

                    if (count > 0 || (!building.UnderConstruction && building.OrderQueue != null && building.OrderQueue.OrderCount > 0))
                    {
                        var button = Instantiate(buttonPrefab);
                        var rectTransform = button.GetComponent<RectTransform>();
                        rectTransform.SetParent(transform);
                        rectTransform.position = new Vector3(x, margin, 0);
                        var text = button.GetComponent<ButtonScript>().innerText;
                        button.name = "Button Cancel";
                        text.text = "Cancel";
                        button.onClick.AddListener(new UnityAction(() => building.OrderQueue.CancelCurrent()));
                        buttons.Add(button);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.F1))
                debugInfoActive = !debugInfoActive;
            debugInfo.SetActive(debugInfoActive);
            debugInfoBack.SetActive(debugInfoActive);

            if (Input.GetKeyDown(KeyCode.F2))
            {
                var army = Globals.Armies.First(a => a.name == "Autobots");
                if (army != null)
                    Debug.Log(army.ResourcesQuadTree.ToString());
            }
        }

        void OnDestroy()
        {
            if (Globals.HumanArmy != null)
                Globals.HumanArmy.OnBuildingConstructionFinished -= InvokeRefreshBuildingGUI;
        }

        void SubscribeEvents(Building building)
        {
            if (building != MapElement.Null && building.Army != null)
                building.Army.Technologies.OnTechnologyDevelopmentChanged += InvokeRefreshBuildingGUI;
        }

        void UnsubscribeEvents(Building building)
        {
            if (building != MapElement.Null && building.Army != null)
                building.Army.Technologies.OnTechnologyDevelopmentChanged -= InvokeRefreshBuildingGUI;
        }

        void InvokeRefreshBuildingGUI()
        {
            refreshBuildingGUI = true;
        }
    }
}
