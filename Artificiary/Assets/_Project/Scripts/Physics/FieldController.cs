using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class FieldController : MonoBehaviour
    {
        [SerializeField] protected BoxCollider2D col;
        [SerializeField] protected SpriteRenderer sprite;
        [SerializeField] protected Vector2 size = Vector2.one;
        [SerializeField] protected float minSize = 2/16f;
        [SerializeField] protected bool hasMaxSize = true;
        [SerializeField, ShowIf("hasMaxSize")] 
        protected float maxSize = 1f;

        void Update()
        {
            UpdateSize();
        }

        public void UpdateSize() 
        {
            if (hasMaxSize)
            size.y = Mathf.Min(maxSize, size.y);
            bool enabled = size.x > minSize && size.y > minSize;

            if (col != null) 
            {
                col.enabled = enabled;
                if (enabled)
                {
                    col.size = size;
                    col.offset = new Vector2(0f, size.y / 2);
                }
            }

            if (sprite != null)
            {
                sprite.enabled = enabled;
                if (enabled)
                {
                    sprite.size = col.size;
                    sprite.gameObject.transform.localPosition = col.offset;
                }
            }
        }

        public void ChangeSize(Vector2 sizeDelta)
        {
            size += sizeDelta;

            size.x = Mathf.Max(size.x, 0);

            if (hasMaxSize)
                size.y = Mathf.Clamp(size.y, 0, maxSize);
            else
                size.y = Mathf.Max(size.y, 0);
        }

        private void OnValidate()
        {
            if (hasMaxSize)
            {
                size.y = Mathf.Min(maxSize, size.y);
            }
        }

        private void Reset()
        {
            col = GetComponent<BoxCollider2D>();
            sprite = GetComponent<SpriteRenderer>();
        }
    }
}
