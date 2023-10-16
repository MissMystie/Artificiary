using FMODUnity;
using Mystie.Core;
using Mystie.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public class InteractBehavior : MonoBehaviour
    {
        protected Entity _entity;
        protected InputController _controller;
        protected Animator _anim;

        [SerializeField] protected LayerMask _interactMask = -1;
        [SerializeField] protected Transform _grabAnchor;
        [SerializeField] protected float _grabTime = 0f;
        [SerializeField] protected float _dropTime = 0f;
        [SerializeField] protected string _grabbableTag = "grabbable";

        protected List<Collider2D> _interactibles;
        protected Collider2D _carriedObj;
        
        [Foldout("Feedback")]
        [SerializeField] protected string _grabAnimParam = "grabbing";
        [Foldout("Feedback")]
        [SerializeField] protected EventReference _grabSFX;
        [Foldout("Feedback")]
        [SerializeField] protected EventReference _dropSFX;
        [Foldout("Feedback")]
        [SerializeField] protected GameObject _interactiblePrompt;

        protected void Awake()
        {
            _entity = Entity.Get(gameObject);
            _controller = _entity.Controller;
            _anim = _entity.Anim;

            _interactibles = new List<Collider2D>();
        }

        protected void OnEnable()
        {
            _controller.interact.performed += OnInteract;
        }

        protected void OnDisable()
        {
            _controller.interact.performed -= OnInteract;
        }

        private void OnInteract()
        {
            //if (currentState != PlayerState.WALKING) return;

            if (_carriedObj != null)
            {
                StartCoroutine(Drop());
                return;
            }

            if (_interactibles.IsNullOrEmpty()) return;

            if (_interactibles[0].gameObject.HasTag(_grabbableTag))
            {
                StartCoroutine(Grab(_interactibles[0]));
            }
        }

        private IEnumerator Grab(Collider2D obj)
        {
            //currentState = PlayerState.INTERACTING;

            _carriedObj = obj;

            _carriedObj.transform.SetParent(_grabAnchor);
            _carriedObj.transform.localPosition = Vector2.zero;

            _carriedObj.enabled = false;
            Rigidbody2D rb = _carriedObj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = true;

            SpriteRenderer renderer = _carriedObj.GetComponent<SpriteRenderer>();
            //if (renderer != null) renderer.sortingLayerName = carriedObjectsSortingLayer;

            _anim?.SetBool(_grabAnimParam, true);
            RuntimeManager.PlayOneShot(_dropSFX, transform.position);

            yield return new WaitForSeconds(_grabTime);

            //currentState = PlayerState.WALKING;

            yield return null;
        }

        private IEnumerator Drop()
        {
            if (_carriedObj == null) yield break;

            //currentState = PlayerState.INTERACTING;

            _anim?.SetBool(_grabAnimParam, false);

            _carriedObj.enabled = true;

            yield return new WaitForSeconds(_dropTime);

            RuntimeManager.PlayOneShot(_dropSFX);
            
            Rigidbody2D rb = _carriedObj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = false;
            _carriedObj.transform.SetParent(null);

            SpriteRenderer renderer = _carriedObj.GetComponent<SpriteRenderer>();
            //if (renderer != null) renderer.sortingLayerName = objectsSortingLayer;

            //TimeObject timeObj = carriedObj.GetComponent<TimeObject>();

            _carriedObj = null;

            //currentState = PlayerState.WALKING;

            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.IsInLayerMask(_interactMask))
            {
                _interactibles.Add(collider);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.IsInLayerMask(_interactMask))
            {
                _interactibles.Remove(collider);
            }
        }
    }
}
