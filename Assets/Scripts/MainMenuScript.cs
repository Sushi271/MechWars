using UnityEngine;
using System.Collections;

namespace MechWars
{
    public class MainMenuScript : MonoBehaviour
    {
        private bool _isFirstMenu = true;
        private bool _isOptionMenu = false;
		
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
			GUI.Label(new Rect(30,75,300,25), gameTitle);
			
            FirstMenu();
			OptionsMenu();
			
			if(_isOptionMenu==true)
			{
				if (GUI.Button(new Rect(10, Screen.height -35, 150, 25), "Back"))
				{
					_isOptionMenu=false;
					
					_isFirstMenu=true;
				}
			}
        }

        void FirstMenu()
        {
            if (_isFirstMenu)
            {
                if (GUI.Button(new Rect(10, Screen.height / 2 - 100, 150, 25), "New Game"))
                {
					Application.LoadLevel("Test");

                }
				if (GUI.Button(new Rect(10, Screen.height / 2 - 65, 150, 25), "Options"))
				{
					_isFirstMenu=false;
					_isOptionMenu=true;
				}
				if (GUI.Button(new Rect(10, Screen.height / 2 - 30, 150, 25), "Exit"))
				{
				Application.Quit();

				}
			}
        }
		void OptionsMenu()
		{
			if (_isOptionMenu)
			{
				
			}
		}
    }
}
