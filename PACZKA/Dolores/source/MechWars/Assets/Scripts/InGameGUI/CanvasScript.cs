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

        public ResourceCounter resourceCounter;
        
        void Start()
        {
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
