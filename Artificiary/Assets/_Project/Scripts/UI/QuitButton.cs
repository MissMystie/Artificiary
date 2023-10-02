using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mystie.UI
{
    public class QuitButton : MonoBehaviour
    {
        [SerializeField] private Button btn;

        private void OnEnable()
        {
            btn.onClick.AddListener(OnQuitBtn);
        }

        private void OnDisable()
        {
            btn.onClick.RemoveListener(OnQuitBtn);
        }

        private void OnQuitBtn()
        {
            GameManager.Quit();
        }

        private void OnReset()
        {
            btn = GetComponent<Button>();
        }
    }
}
