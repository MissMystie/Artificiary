using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mystie.Core
{
    public class GameManager
    {
        public static event Action<GameState> onGameStateChanged;
        public static event Action onPause;
        public static event Action onUnpause;

        public static GameManager instance = new GameManager();
        public static GameManager Instance
        {
            get { return instance; }
        }

        public static SystemDataScriptable systemData;
        public static Controls controls;

        public static bool isPaused = false;
        public static GameState gameState { get; private set; }

        private const string systemDataPath = "SystemData";

        private GameManager()
        {
            //gameState = GameState.StartScreen;

            isPaused = false;

            controls = new Controls();
            controls.UI.Enable();

            SceneManager.sceneLoaded += OnSceneLoaded;

            systemData = Resources.Load<SystemDataScriptable>(systemDataPath);
            if (systemData == null) Debug.LogError("GameManager: System Data not found.");

            GameSettings.LoadSettings(systemData);
        }

        private void OnDestroy()
        {
            controls.UI.Disable();
        }

        public static void SetGameState(GameState state)
        {
            if (gameState != state) return;

            gameState = state;
            onGameStateChanged?.Invoke(gameState);
        }

        public static void Pause()
        {
            if (isPaused) return;

            gameState = GameState.Pause;

            Time.timeScale = 0f;
            isPaused = true;
            onPause?.Invoke();
        }

        public static void Unpause()
        {
            if (!isPaused) return;

            gameState = GameState.Play;

            Time.timeScale = 1f;
            isPaused = false;
            onUnpause?.Invoke();
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Unpause();
        }

        public static void LoadMainMenu()
        {
            Debug.Log("Loading main menu...");
            gameState = GameState.MainMenu;
            SceneManager.LoadSceneAsync(systemData.mainMenuScene);
        }

        public static void Gameover()
        {
            Debug.Log("Loading gameover...");
            gameState = GameState.Gameover;
            SceneManager.LoadSceneAsync(systemData.gameoverScene);
        }

        public static void Quit()
        {
            Debug.Log("Quitting the game...");

#if UNITY_EDITOR

            if (UnityEditor.EditorApplication.isPlaying == true)
                UnityEditor.EditorApplication.isPlaying = false;

#endif

            Application.Quit();
        }
    }

    public enum GameState
    {
        StartScreen,
        MainMenu,
        Play,
        Pause,
        Gameover
    }
}
