using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ETM.MurkEditor
{
    public class GameManagerEditorWindow : EditorWindow
    {
        public string uid = "";
        public string username = "username";
        public int score = 0;
        public string leaderboard = "leaderboard";

        public const float FETCH_DELAY = 1f;

        [MenuItem("Window/Mista/Game Manager")]
        public static void ShowWindow()
        {
            GetWindow<GameManagerEditorWindow>("Game Manager");
        }

        void OnGUI()
        {
            GUILayout.Label("Leaderboard", EditorStyles.boldLabel);

            GUI.enabled = Application.isPlaying;

            GUILayout.BeginHorizontal();

            GUILayout.Label("UID: " + uid);
            if (GUILayout.Button("Login"))
            {
                //LeaderboardManager.Login();
                //uid = LeaderboardManager.uid;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            username = EditorGUILayout.TextField("Username", username);

            if (GUILayout.Button("Set Username"))
            {
                //LeaderboardManager.SetUsername(username);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            score = EditorGUILayout.IntField("Score", score);
            leaderboard = EditorGUILayout.TextField("Leaderboard", leaderboard);

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Submit Score"))
            {
                //LeaderboardManager.SendScore(leaderboard, score);
            }

            GUI.enabled = true;
        }
    }
}
