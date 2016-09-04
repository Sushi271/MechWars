using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    public class CanvasScript : MonoBehaviour
    {
        public Button buttonPrefab;
        
        public GameObject debugInfo;
        public GameObject debugInfoBack;
        bool debugInfoActive;

        public GameObject buttonsInfo;
        public GameObject buttonsInfoBack;

        // lista wszystkich buttonow
        List<Button> buttons;
        public List<Button> Buttons { get { return buttons; } }
        
        void Start()
        {
            // tworzymy liste buttonow
            buttons = new List<Button>();

            // domyslnie chcemy by buttonsInfo nie bylo widoczne
            buttonsInfo.SetActive(false);
            buttonsInfoBack.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                debugInfoActive = !debugInfoActive;
            debugInfo.SetActive(debugInfoActive);
            debugInfoBack.SetActive(debugInfoActive);

            // pobieramy tylko te guziki, ktore sa widoczne
            var possibleButtons = buttons.Where(b => b.gameObject.activeSelf);
            foreach (var b in possibleButtons) // dla kazdej pary klucz-wartosc w slowniku
            {
                var orderActionButton = b.GetComponent<OrderActionButton>();
                // jesli wcisnieto klawisz hotkeya
                if (Input.GetKeyDown(orderActionButton.hotkey))
                    b.onClick.Invoke();
            }
        }

        public void SetHoveredButton(Button button)
        {
            // Canvas ustawia ten podswietlony button (jakis albo NULL) ButtonsInfoDisplayerowi
            buttonsInfo.GetComponent<ButtonsInfoDisplayer>().SetHoveredButton(button);
            // i wylacza/wlacza buttonInfo i jego tlo w zaleznosci od tego czy button jest NULL czy nie
            buttonsInfo.SetActive(button != null);
            buttonsInfoBack.SetActive(button != null);
        }
    }
}
