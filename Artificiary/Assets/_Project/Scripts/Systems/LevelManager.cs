using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Core
{
    public class LevelManager : MonoBehaviour
    {
        #region Singleton

        public static LevelManager Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = FindObjectOfType<LevelManager>();
                if (instance != null)
                    return instance;

                return null;
            }
        }

        protected static LevelManager instance;

        #endregion

        [SerializeField] private Transform startPosition;

        public Entity player { get; private set; }

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            GameObject playerObj = MultiTags.FindWithMultiTag(Tags.PLAYER_TAG);
            if (playerObj != null) player = Entity.Get(playerObj);

            if (player != null)
            {
                if (GameManager.gameState == GameState.Play && startPosition != null)
                    player.transform.position = startPosition.position;

                if (player.Health != null)
                    player.Health.onDeath += (health) => { StartCoroutine(OnPlayerDeath()); };
            }
            else
            {
                Debug.LogError("No player found.", this);
            }
        }

        public IEnumerator OnPlayerDeath()
        {
            
            Debug.Log("Player dead!");

            yield break;
        }
    }
}
