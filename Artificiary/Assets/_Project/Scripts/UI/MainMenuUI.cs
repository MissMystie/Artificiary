using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mystie.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public GameObject mainMenuUI;
        public GameObject settingsUI;

        [Space]

        public string startScene = "Intro";

        public enum UIState { MAINMENU, SETTINGS }
        public UIState state;

        void Start()
        {
            state = UIState.MAINMENU;

            mainMenuUI.SetActive(true);
            settingsUI.SetActive(false);

            GameManager.controls.UI.Pause.performed += ctx => OnPause();
        }

        private void OnDestroy()
        {
            GameManager.controls.UI.Pause.performed -= ctx => OnPause();
        }

        public void PlayBtn()
        {
            SceneManager.LoadSceneAsync(startScene);
        }

        public void OpenSettings()
        {
            state = UIState.SETTINGS;

            mainMenuUI.SetActive(false);
            settingsUI.SetActive(true);
        }

        public void CloseSettings()
        {
            if (state == UIState.SETTINGS)
            {
                state = UIState.MAINMENU;

                settingsUI.SetActive(false);
                mainMenuUI.SetActive(true);
            }
        }

        public void OnPause()
        {
            if (state == UIState.SETTINGS) CloseSettings();
        }

        public void QuitBtn()
        {
            GameManager.Quit();
        }
    }
}
