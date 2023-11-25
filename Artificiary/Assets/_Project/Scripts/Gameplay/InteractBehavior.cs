using FMODUnity;
using Mystie.Core;
using Mystie.Physics;
using Mystie.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Mystie.Gameplay
{
    public class InteractBehavior : MonoBehaviour
    {
        protected Entity _entity;
        protected InputController _controller;
        protected Animator _anim;

        [SerializeField] protected LayerMask _interactMask = -1;
        [SerializeField] protected Vector2 _interactOffset = new Vector2(0, 1);
        [SerializeField] protected Vector2 _interactSize = new Vector2(3,2);
        [SerializeField] protected Transform _grabAnchor;
        [SerializeField] protected float _grabTime = 0f;
        [SerializeField] protected float _dropTime = 0f;
        [SerializeField] protected string _carryableTag = "Carryable";

        [Space]

        [SerializeField] protected float _throwStrength = 20;

        protected List<Collider2D> _interactibles;
        protected Collider2D _carriedObj;
        
        [Foldout("Feedback")]
        [SerializeField] protected string _carryingAnimParam = "carrying";
        [Foldout("Feedback")]
        [SerializeField] protected EventReference _grabSFX;
        [Foldout("Feedback")]
        [SerializeField] protected EventReference _dropSFX;
        [Foldout("Feedback")]
        [SerializeField] protected GameObject _interactiblePrompt;

        [Header("Debug")]

        public bool showDebug;

        protected void Awake()
        {
            _entity = Entity.Get(gameObject);
            _controller = _entity.Controller;
            _anim = _entity.Anim;

            _interactibles = new List<Collider2D>();
        }

        protected void OnEnable()
        {
            _controller.shoot.performed += OnShoot;
            _controller.interact.performed += OnInteract;
        }

        protected void OnDisable()
        {
            _controller.shoot.performed -= OnShoot;
            _controller.interact.performed -= OnInteract;
        }

        protected void FixedUpdate()
        {
            _interactibles.Clear();
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + _interactOffset.xyz(), _interactSize, 0f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.IsInLayerMask(_interactMask))
                {
                    _interactibles.Add(collider);
                }
            }
        }

        private void OnShoot() 
        {
            if (_carriedObj == null) return;

            StartCoroutine(ThrowCoroutine());
        }

        private void OnInteract()
        {
            //if (currentState != PlayerState.WALKING) return;

            if (_carriedObj != null)
            {
                StartCoroutine(DropCoroutine());
                return;
            }

            if (_interactibles.IsNullOrEmpty()) return;

            if (_interactibles[0].gameObject.HasTag(_carryableTag))
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
            PhysicsObject phys = _carriedObj.GetComponent<PhysicsObject>();
            if (phys != null)
            {
                phys.velocity = Vector2.zero;
                phys.simulatePhysics = false;
            }

            SpriteRenderer renderer = _carriedObj.GetComponent<SpriteRenderer>();
            //if (renderer != null) renderer.sortingLayerName = carriedObjectsSortingLayer;

            _anim?.SetBool(_carryingAnimParam, true);
            RuntimeManager.PlayOneShot(_dropSFX, transform.position);

            yield return new WaitForSeconds(_grabTime);

            //currentState = PlayerState.WALKING;

            yield return null;
        }

        private PhysicsObject Drop()
        {
            if (_carriedObj == null) return null;

            _carriedObj.enabled = true;

            PhysicsObject phys = _carriedObj.GetComponent<PhysicsObject>();
            if (phys != null)
            {
                if (_entity.Phys != null)
                    phys.velocity = _entity.Phys.velocity;
                phys.simulatePhysics = true;
            }

            _carriedObj.transform.SetParent(null);

            SpriteRenderer renderer = _carriedObj.GetComponent<SpriteRenderer>();

            _carriedObj = null;

            return phys;
        }

        private IEnumerator ThrowCoroutine()
        {
            if (_carriedObj == null) yield break;

            yield return new WaitForSeconds(_dropTime);

            PhysicsObject physObj = Drop();

            if (physObj != null)
            {
                Vector2 shootV = _entity.Controller.aim.normalized * _throwStrength;
                physObj.SetVelocity(shootV);
            }

            yield return null;
        }

        private IEnumerator DropCoroutine()
        {
            if (_carriedObj == null) yield break;

            //currentState = PlayerState.INTERACTING;

            _anim?.SetBool(_carryingAnimParam, false);

            RuntimeManager.PlayOneShot(_dropSFX);

            yield return new WaitForSeconds(_dropTime);

            Drop();

            yield return null;
        }

        private void OnDrawGizmos()
        {
            if (!showDebug) return;

            Gizmos.color = !_interactibles.IsNullOrEmpty()?
                    Color.green : Color.gray;

            Gizmos.DrawWireCube(transform.position.xy() + _interactOffset, _interactSize);
        }
    }
}
