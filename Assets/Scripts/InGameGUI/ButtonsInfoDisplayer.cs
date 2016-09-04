using UnityEngine;
using UnityEngine.UI;

namespace MechWars.InGameGUI
{
    // InfoDisplayer z tekstem - opisem buttonow
    public class ButtonsInfoDisplayer : InfoDisplayer
    {
        Button hoveredButton;
        Vector2 anchorPosition;

        void Start()
        {
            anchorPosition = GetComponent<RectTransform>().anchoredPosition;
        }

        void Update()
        {
            var rectTransform = GetComponent<RectTransform>();
            
            rectTransform.anchoredPosition = anchorPosition + 
                new Vector2(-rectTransform.rect.width, rectTransform.rect.height);
        }

        public void SetHoveredButton(Button button)
        {
            // jesli nie zaszla zadna zmiana to olewamy
            if (button == hoveredButton) return;

            hoveredButton = button;

            // podmieniamy tekst w ramce
            var text = GetComponent<Text>();
            if (button == null)
            {
                // na pusty
                text.text = "";
            }
            else
            {
                // albo na opis nowego buttona
                var oaButton = button.GetComponent<OrderActionButton>();
                var ouButton = button.GetComponent<OrderUtilityButton>();
                if (oaButton != null) // jesli jest skrypt OrderActionButton na buttonie
                    text.text = oaButton.description;
                else if (ouButton != null) // lub jesli jest skrypt OrderUtilityButton na buttonie
                    text.text = ouButton.description;
            }
        }
    }
}
