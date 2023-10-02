using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mystie
{
    public class SceneTransition : MonoBehaviour
    {
        public float transitionTime = 20f;
        public string nextLevel = "Tutorial";

        void Start()
        {
            StartCoroutine(Transition());

            GameManager.controls.UI.Pause.performed += ctx => LoadLevel();
        }

        private void OnDestroy()
        {
            GameManager.controls.UI.Pause.performed -= ctx => LoadLevel();
        }

        IEnumerator Transition()
        {
            yield return new WaitForSeconds(transitionTime);

            LoadLevel();
        }

        private void LoadLevel()
        {
            StopAllCoroutines();
            SceneManager.LoadSceneAsync(nextLevel);
        }
    }
}
