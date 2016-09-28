using UnityEngine;

namespace MechWars.PlayerInput
{
    // klasa ktora odpowiada za wyswietlanie wlasciwego obrazka kursora we wlasciwym miejscu
    public class CursorController : MonoBehaviour
    {
        void Start()
        {
            Cursor.visible = false;
        }

        void Update()
        {
            var pos = Globals.Spectator.InputController.Mouse.Position;
            var cursor = gameObject;

            var rt = cursor.GetComponent<RectTransform>();
            rt.position = pos;
        }
    }
}