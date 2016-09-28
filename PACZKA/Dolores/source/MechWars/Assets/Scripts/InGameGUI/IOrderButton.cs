using UnityEngine;

namespace MechWars.InGameGUI
{
    public interface IOrderButton
    {
        KeyCode Hotkey { get; }
        string Description { get; }
        GameObject gameObject { get; }
    }
}