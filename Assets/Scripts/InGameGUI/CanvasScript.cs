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

        List<Button> buttons;

        void Start()
        {
            buttons = new List<Button>();
        }

        void Update()
        {
            var hp = thisPlayer as HumanPlayer;
            if (hp == null) return;

            var selBuilding = hp.SelectionController.SelectedMapElements.FirstOrDefault() as Building;
            if (selBuilding != building)
            {
                building = selBuilding;
                foreach (var b in buttons)
                {
                    b.onClick.RemoveAllListeners();
                    Destroy(b.gameObject);
                }
                buttons.Clear();

                if (building != null && !building.UnderConstruction)
                {
                    var prodOpts = building.unitProductionOptions;
                    var constOpts = building.buildingConstructionOptions;
                    var count = prodOpts.Count + constOpts.Count;

                    float margin = 4;
                    float h = buttonPrefab.GetComponent<RectTransform>().sizeDelta.y;
                    float x = margin;
                    float y = count * (h + margin) + margin;
                    for (int i = 0; i < count; i++, y -= h + margin)
                    {
                        var button = Instantiate(buttonPrefab);
                        var rectTransform = button.GetComponent<RectTransform>();
                        rectTransform.parent = transform;
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
                        else
                        {
                            var constOpt = constOpts[i - prodOpts.Count];
                            button.name = string.Format("Button {0}", constOpt.building.mapElementName);
                            text.text = string.Format("Build {0} ({1} RP)", constOpt.building.mapElementName, constOpt.cost);
                            button.onClick.AddListener(new UnityAction(
                                () => hp.OrderController.ConstructionOrdered(building, constOpt)));
                        }
                        buttons.Add(button);
                    }

                    if (count > 0)
                    {
                        var button = Instantiate(buttonPrefab);
                        var rectTransform = button.GetComponent<RectTransform>();
                        rectTransform.SetParent(transform);
                        rectTransform.position = new Vector3(x, margin, 0);
                        var text = button.GetComponent<ButtonScript>().innerText;
                        button.name = "Button Cancel";
                        text.text = "Cancel";
                        button.onClick.AddListener(new UnityAction(
                            () => hp.OrderController.CancelOrder(building)));
                        buttons.Add(button);
                    }
                }
            }
            else
            {
                foreach (var b in buttons)
                {
                    b.enabled = hp.OrderController.MouseMode == MouseMode.Default;
                }
            }
        }
    }
}
