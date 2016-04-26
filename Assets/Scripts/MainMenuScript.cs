using UnityEngine;
using UnityEngine.SceneManagement;

namespace MechWars
{
    public class MainMenuScript : MonoBehaviour
    {
        public GUISkin GameSkin;

        private bool _isFirstMenu = true;
        private bool _isOptionMenu = false;
        private bool _isPlayMenu = false;
        private bool _isGraphicsOptions = false;
        private bool _isAudioOptions = false;
        private bool _isAudioON = true;
        private bool _isFullscreenON = false;

        private float _gameVolume = 0.6f;

        public string gameTitle = "";
        
        // Use this for initialization
        void Start()
        {
            _gameVolume = PlayerPrefs.GetFloat("Game Volume", _gameVolume);
            if (PlayerPrefs.HasKey("Game Volume"))
            {
                AudioListener.volume = PlayerPrefs.GetFloat("Game Volume", _gameVolume);
            }
            else
            {
                PlayerPrefs.SetFloat("Game Volume", _gameVolume);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        void OnGUI()
        {
            GUI.skin = GameSkin;

            GUI.Label(new Rect(Screen.width / 2 - 50, 35, 300, 25), gameTitle, "Menu Title");

            FirstMenu();
            OptionsMenu();
            PlayMenu();
            AudioOptionsDisplay();
            GraphicOptionsDisplay();
        }

        void FirstMenu()
        {
            if (_isFirstMenu)
            {
                if (GUI.Button(new Rect(50, Screen.height / 2 - 100, 181, 48), "", "Play Button Style"))
                {
                    if (_isPlayMenu == false)
                    {
                        //_isFirstMenu=false;
                        _isPlayMenu = true;
                        _isOptionMenu = false;
                    }
                    else
                    {
                        _isPlayMenu = false;
                    }
                }
                if (GUI.Button(new Rect(50, Screen.height / 2, 181, 48), "", "Options Button Style"))
                {
                    if (_isOptionMenu == false)
                    {
                        //_isFirstMenu=false;
                        _isPlayMenu = false;
                        _isOptionMenu = true;
                    }
                    else
                    {
                        _isOptionMenu = false;
                    }
                }
                if (GUI.Button(new Rect(50, Screen.height / 2 + 100, 181, 48), "", "Exit Button Style"))
                {
                    Application.Quit();

                }
            }
        }

        void PlayMenu()
        {
            if (_isPlayMenu)
            {
                GUI.Button(new Rect(235, Screen.height / 2 - 76, 50, 56), "", "Link Button Style");
                if (GUI.Button(new Rect(289, Screen.height / 2 - 100, 229, 48), "", "AI Button Style"))
                {
                    SceneManager.LoadScene("Test");
                }
                if (GUI.Button(new Rect(289, Screen.height / 2 - 44, 229, 48), "", "Player Button Style"))
                {
                    SceneManager.LoadScene("Test");
                }
            }
        }

        void OptionsMenu()
        {
            if (_isOptionMenu)
            {
                GUI.Button(new Rect(235, Screen.height / 2 + 24, 50, 56), "", "Link Button Style");
                string audioButtonStyle = _isAudioON ? "AudioON Button Style" : "AudioOFF Button Style";
                if (GUI.Button(new Rect(289, Screen.height / 2, 229, 48), "", audioButtonStyle))
                {
                    _isAudioON = !_isAudioON;
                    _isAudioOptions = true;
                    _isGraphicsOptions = false;
                    AudioOFF();
                }
                string fullscreenButtonStyle = _isFullscreenON ? "FullscreenON Button Style" : "FullscreenOFF Button Style";
                if (GUI.Button(new Rect(289, Screen.height / 2 + 56, 229, 48), "", fullscreenButtonStyle))
                {
                    _isFullscreenON = !_isFullscreenON;
                    _isAudioOptions = false;
                    _isGraphicsOptions = true;
                    ChangeFullscreen();
                }
            }
        }

        public void AudioOptionsDisplay()
        {
            if (_isAudioOptions)
            {

            }
        }

        public void GraphicOptionsDisplay()
        {
            if (_isGraphicsOptions)
            {


            }

        }

        public void ChangeFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        public void AudioOFF()
        {

        }
    }
}