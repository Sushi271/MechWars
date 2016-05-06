using MechWars.Human;
using MechWars.MapElements;
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

            var selBuilding = hp.SelectionController.SelectedMapElements.FirstOrDefault() as Building;
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
                    var prodOpts = building.unitProductionOptions
                        .Where(po => po.CheckRequirements(building.army)).ToList();
                    var constOpts = building.buildingConstructionOptions
                        .Where(bo => bo.CheckRequirements(building.army)).ToList();
                    var devOpts = building.technologyDevelopmentOptions
                        .Where(tdo => building.army.Technologies.CanDevelop(tdo.technology))
                        .Where(tdo => tdo.CheckRequirements(building.army)).ToList();
                    var count = prodOpts.Count + constOpts.Count + devOpts.Count;

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
                        if (i < prodOpts.Count)
                        {
                            var prodOpt = prodOpts[i];
                            button.name = string.Format("Button {0}", prodOpt.unit.mapElementName);
                            text.text = string.Format("Produce {0} ({1} RP)", prodOpt.unit.mapElementName, prodOpt.cost);
                            button.onClick.AddListener(new UnityAction(
                                () => hp.OrderController.ProductionOrdered(building, prodOpt)));
                        }
                        else if (i < prodOpts.Count + constOpts.Count)
                        {
                            var constOpt = constOpts[i - prodOpts.Count];
                            button.name = string.Format("Button {0}", constOpt.building.mapElementName);
                            text.text = string.Format("Build {0} ({1} RP)", constOpt.building.mapElementName, constOpt.cost);
                            button.onClick.AddListener(new UnityAction(
                                () => hp.OrderController.ConstructionOrdered(building, constOpt)));
                        }
                        else
                        {
                            var devOpt = devOpts[i - prodOpts.Count - constOpts.Count];
                            button.name = string.Format("Button {0}", devOpt.technology.technologyName);
                            text.text = string.Format("Develop {0} ({1} RP)", devOpt.technology.technologyName, devOpt.cost);
                            button.onClick.AddListener(new UnityAction(() => hp.OrderController.DevelopmentOrdered(building, devOpt)));
                        }
                        buttons.Add(button);
                    }

                    if (count > 0 || (!building.UnderConstruction && building.OrderExecutor.Count > 0))
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
            if (building != null)
                building.army.Technologies.OnTechnologyDevelopmentChanged += InvokeRefreshBuildingGUI;
        }

        void UnsubscribeEvents(Building building)
        {
            if (building != null)
                building.army.Technologies.OnTechnologyDevelopmentChanged -= InvokeRefreshBuildingGUI;
        }

        void InvokeRefreshBuildingGUI()
        {
            refreshBuildingGUI = true;
        }
    }
}
