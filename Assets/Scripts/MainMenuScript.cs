using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

private bool _isFirstMenu = true;
private bool _isLevelSelectMenu = false;
private bool _isOptionMenu = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI()
	{
		FirstMenu();
	}
	void FirstMenu()
	{
		if (_isFirstMenu)
		{
			if (GUI.Button(new.Rect(10, Screen.height / 2 - 100, 150, 25) "New Game"))
			{
				
			}
		}
	}
}
