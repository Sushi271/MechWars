using MechWars.Human;
using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    public class CanvasScript : MonoBehaviour
    {
        public Player thisPlayer;
        public Button buttonPrefab;

        Building building;
        bool refreshBuildingGUI;

        List<Button> buttons;

        void Start()
        {
            buttons = new List<Button>();
            thisPlayer.Army.OnBuildingConstructionFinished += InvokeRefreshBuildingGUI;
        }

        void Update()
        {
            var hp = thisPlayer as HumanPlayer;
            if (hp == null) return;

            var selBuilding = hp.InputController.SelectionMonitor.SelectedMapElements.FirstOrDefault() as Building;
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

                if (building != null && !building.UnderConstruction)
                {
                    var productionOAs = building.orderActions.OfType<UnitProductionOrderAction>()
                        .Where(poa => poa.CheckRequirements(building.army)).ToList();
                    var constructionOAs = building.orderActions.OfType<BuildingConstructionOrderAction>()
                        .Where(coa => coa.CheckRequirements(building.army)).ToList();
                    var developmentOAs = building.orderActions.OfType<TechnologyDevelopmentOrderAction>()
                        .Where(doa => building.army.Technologies.CanDevelop(doa.technology))
                        .Where(doa => doa.CheckRequirements(building.army)).ToList();
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
                                poa.GiveOrder(hp.InputController, building)));
                        }
                        else if (i < productionOAs.Count + constructionOAs.Count)
                        {
                            var coa = constructionOAs[i - productionOAs.Count];
                            button.name = string.Format("Button {0}", coa.building.mapElementName);
                            text.text = string.Format("Build {0} ({1} RP)", coa.building.mapElementName, coa.cost);
                            button.onClick.AddListener(new UnityAction(
                                () => hp.InputController.CarriedOrderAction = coa));
                        }
                        else
                        {
                            var doa = developmentOAs[i - productionOAs.Count - constructionOAs.Count];
                            button.name = string.Format("Button {0}", doa.technology.technologyName);
                            text.text = string.Format("Develop {0} ({1} RP)", doa.technology.technologyName, doa.cost);
                            button.onClick.AddListener(new UnityAction(() =>
                                doa.GiveOrder(hp.InputController, building)));
                        }
                        buttons.Add(button);
                    }

                    if (count > 0 || (!building.UnderConstruction && building.OrderExecutor.OrderCount > 0))
                    {
                        var button = Instantiate(buttonPrefab);
                        var rectTransform = button.GetComponent<RectTransform>();
                        rectTransform.SetParent(transform);
                        rectTransform.position = new Vector3(x, margin, 0);
                        var text = button.GetComponent<ButtonScript>().innerText;
                        button.name = "Button Cancel";
                        text.text = "Cancel";
                        button.onClick.AddListener(new UnityAction(() => hp.OrderController.CancelOrder(building)));
                        buttons.Add(button);
                    }
                }
            }
            else
            {
                foreach (var b in buttons)
                {
                    b.interactable = hp.MouseMode == MouseMode.Default;
                }
            }
        }

        void OnDestroy()
        {
            if (thisPlayer != null && thisPlayer.Army != null)
                thisPlayer.Army.OnBuildingConstructionFinished -= InvokeRefreshBuildingGUI;
        }

        void SubscribeEvents(Building building)
        {
            if (building != null && building.army != null)
                building.army.Technologies.OnTechnologyDevelopmentChanged += InvokeRefreshBuildingGUI;
        }

        void UnsubscribeEvents(Building building)
        {
            if (building != null && building.army != null)
                building.army.Technologies.OnTechnologyDevelopmentChanged -= InvokeRefreshBuildingGUI;
        }

        void InvokeRefreshBuildingGUI()
        {
            refreshBuildingGUI = true;
        }
    }
}
