using LDtkUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class Resizeable : MonoBehaviour, ILDtkImportedEntity
    {
        [SerializeField] protected BoxCollider2D col;
        [SerializeField] protected SpriteRenderer sprite;

        [SerializeField] protected Vector2 size;

        public void OnLDtkImportEntity(EntityInstance entityInstance)
        {
            //Debug.Log("Resize: " + entityInstance.UnityScale);
            size = entityInstance.UnityScale;
            
            if (sprite != null) {
                sprite.size = size;
            }

            if (col != null) {
                col.size = size;
                col.offset = new Vector2(0f, size.y / 2);
            }
            
            transform.localScale = Vector2.one;
        }
    }
}
