using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie
{
    public class SpriteManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer masterSprite;
        [SerializeField] private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

        private void Update()
        {
            UpdateSprites();
        }

        public void UpdateSprites()
        {
            if (masterSprite == null) return;

            foreach (SpriteRenderer sprite in sprites)
            {
                if (sprite != null)
                {
                    sprite.color = masterSprite.color;
                    sprite.sharedMaterial = masterSprite.sharedMaterial;
                    sprite.sortingLayerID = masterSprite.sortingLayerID;
                }
            }
        }

        private void Reset()
        {
            masterSprite = GetComponent<SpriteRenderer>();
            sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
            sprites.Remove(masterSprite);
        }

        private void OnValidate()
        {
            if (sprites.Contains(masterSprite))
                sprites.Remove(masterSprite);
        }
    }
}
