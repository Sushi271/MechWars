using UnityEngine;
using System.Collections;

namespace MechWars
{
    public class MainMenuScript : MonoBehaviour
    {
		public GUISkin GameSkin;
		
        private bool _isFirstMenu = true;
        private bool _isOptionMenu = false;
		private bool _isPlayMenu = false;
		
		public string gameTitle = "";

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        void OnGUI()
        {
			GUI.skin=GameSkin;
			
			GUI.Label(new Rect(30,75,300,25), gameTitle, "Menu Title");
			
            FirstMenu();
			OptionsMenu();
			PlayMenu();
			
			if(_isPlayMenu==true)
			{
				if (GUI.Button(new Rect(10, Screen.height -35, 150, 25), "Back"))
				{
					_isOptionMenu=false;
					_isPlayMenu = false;
					_isFirstMenu=true;
				}
			}
			if(_isOptionMenu==true)
			{
				if (GUI.Button(new Rect(10, Screen.height -35, 150, 25), "Back"))
				{
					_isOptionMenu=false;
					_isPlayMenu = false;
					_isFirstMenu=true;
				}
			}
        }

        void FirstMenu()
        {
            if (_isFirstMenu)
            {
                if (GUI.Button(new Rect(10, Screen.height / 2 - 100, 150, 25), "Play"))
                {
					_isFirstMenu=false;
					_isPlayMenu=true;
					_isOptionMenu=false;
					

                }
				if (GUI.Button(new Rect(10, Screen.height / 2 - 65, 150, 25), "Options"))
				{
					_isFirstMenu=false;
					_isPlayMenu=false;
					_isOptionMenu=true;
				}
				if (GUI.Button(new Rect(10, Screen.height / 2 - 30, 150, 25), "Exit"))
				{
				Application.Quit();

				}
			}
        }
		void PlayMenu()
		{
			if (_isPlayMenu)
			{
				if (GUI.Button(new Rect(10, Screen.height / 2 - 100, 150, 25), "AI vs AI"))
				{
					//Application.LoadLevel("Test");
				}
				if (GUI.Button(new Rect(10, Screen.height / 2 - 65, 150, 25), "Player vs AI"))
				{
					//Application.LoadLevel("Test");
				}
			}
		}
		void OptionsMenu()
		{
			if (_isOptionMenu)
			{
				if (GUI.Button(new Rect(10, Screen.height / 2 - 100, 150, 25), "Audio"))
				{
					
				}
				if (GUI.Button(new Rect(10, Screen.height / 2 - 65, 150, 25), "Fullscreen"))
				{
					
				}
			}
		}
    }
}
