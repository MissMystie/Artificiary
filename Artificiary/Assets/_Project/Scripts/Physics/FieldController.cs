using FMODUnity;
using FMOD.Studio;
using LDtkUnity;
using Mystie.Logic;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mystie.Systems;

namespace Mystie
{
    public class FieldController : MonoBehaviour, ILDtkImportedFields
    {
        [SerializeField] protected BoxCollider2D col;
        [SerializeField] protected SpriteRenderer sprite;
        [SerializeField] protected Vector2 _size = Vector2.one;
        [SerializeField] protected float minSize = 2/16f;
        [SerializeField] protected bool hasMaxSize = true;
        [SerializeField, ShowIf("hasMaxSize")] 
        protected float maxSize = 1f;
        [SerializeField] int direction = 1;

        [SerializeField] protected EventReference volumeIncreaseLoop;
        [SerializeField] protected EventReference volumeDecreaseLoop;

        protected EventInstance volumeIncresaeInstance;
        protected EventInstance volumeDecreaseInstance;

        public bool IsFull { get => hasMaxSize && _size.y >= maxSize; }
        public bool IsEmpty { get => _size.y <= minSize; }

        private void Start()
        {
            if (!volumeIncreaseLoop.IsNull)
                volumeIncresaeInstance = AudioManager.Instance.CreateEventInstance(volumeIncreaseLoop);
            if (!volumeDecreaseLoop.IsNull)
                volumeDecreaseInstance = AudioManager.Instance.CreateEventInstance(volumeDecreaseLoop);
        }

        void Update()
        {
            UpdateSize();
        }

        public void UpdateSize() 
        {
            if (hasMaxSize)
                _size.y = Mathf.Min(maxSize, _size.y);
            //bool enabled = _size.x >= minSize && _size.y >= minSize;
            bool enabled = true;

            if (col != null) 
            {
                col.enabled = enabled;
                if (enabled)
                {
                    col.size = _size;
                    col.offset = new Vector2(0f, direction * _size.y / 2);
                }
            }

            if (sprite != null)
            {
                sprite.enabled = enabled;
                if (enabled)
                {
                    sprite.transform.localScale = col.size;
                    //sprite.size = col.size;
                    sprite.gameObject.transform.localPosition = col.offset;
                }
            }
        }

        public float ChangeVolume (float volumeDelta)
        {
            float oldVolume = _size.y;

            _size.y += volumeDelta;

            SetSize(_size);

            volumeDelta = Volume() - oldVolume;
            return volumeDelta;
        }

        public float Volume()
        {
            return _size.y;
        }

        public void SetSize(Vector2 size)
        {
            _size = size;

            _size.x = Mathf.Max(size.x, 0);

            if (hasMaxSize)
                _size.y = Mathf.Clamp(_size.y, 0, maxSize);
            else
                _size.y = Mathf.Max(_size.y, 0);
        }

        public void ChangeSize(Vector2 sizeDelta)
        {
            SetSize(_size + sizeDelta);
        }

        private void OnValidate()
        {
            direction = direction != 0? System.Math.Sign(direction) : 1;

            if (hasMaxSize)
            {
                _size.y = Mathf.Min(maxSize, _size.y);
            }
        }

        private void Reset()
        {
            col = GetComponent<BoxCollider2D>();
        }

        public void OnLDtkImportFields(LDtkFields fields)
        {
            Vector2 size = transform.localScale;
            _size = size;
            transform.localScale = Vector2.one;

            fields.TryGetBool("has_max_size", out hasMaxSize);
            fields.TryGetFloat("max_size", out maxSize);

            UpdateSize();
        }
    }
}
