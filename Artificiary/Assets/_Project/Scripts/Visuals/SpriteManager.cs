using Mystie.Core;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie
{
    public class SpriteManager : MonoBehaviour
    {
        public SpriteRenderer masterSprite;
        public List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        private Material baseMaterial;

        private void Awake()
        {
            baseMaterial = masterSprite.sharedMaterial;
            UpdateSprites();
        }

        private void Update()
        {
            UpdateSprites();
        }

        [Button()]
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

        public void SetMaterial(Material mat)
        {
            masterSprite.sharedMaterial = mat;
            UpdateSprites();
        }

        public void ResetMaterial()
        {
            masterSprite.sharedMaterial = baseMaterial;
            UpdateSprites();
        }

        public static implicit operator SpriteRenderer(SpriteManager spriteManager)
        {
            return spriteManager.masterSprite;
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
