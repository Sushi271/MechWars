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
            // na pusty, jeśli button niezaznaczony
            if (button == null) text.text = "";
            // albo na opis nowego buttona
            else text.text = button.GetComponent<IOrderButton>().Description;
        }
    }
}
