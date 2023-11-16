using Mystie.ChemEngine;
using Mystie.Gameplay;
using Mystie.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Core
{
    public class Entity : MonoBehaviour
    {
        #region components

        public static Entity Get(GameObject target)
        {
            Entity entity = target.GetComponent<Entity>();
            if (entity == null) entity = target.AddComponent<Entity>();
            return entity;
        }

        private PhysicsBody _body;
        public PhysicsBody Body
        {
            get { return _body ? _body : (_body = GetComponent<PhysicsBody>()); }
            set => _body = value;
        }

        private PhysicsObject _phys;
        public PhysicsObject Phys
        {
            get { return _phys ? _phys : (_phys = GetComponent<PhysicsObject>()); }
            set => _phys = value; 
        }

        private Actor _actor;
        public Actor Actor
        {
            get { return _actor ? _actor : (_actor = GetComponent<Actor>()); }
            set => _actor = value; 
        }

        [SerializeField] private Collider2D _collider;
        public Collider2D Collider
        {
            get { return _collider ? _collider : (_collider = GetComponent<Collider2D>()); }
            set => _collider = value; 
        }

        private HealthManager _health;
        public HealthManager Health
        {
            get { return _health ? _health : (_health = GetComponent<HealthManager>()); }
            set => _health = value; 
        }

        private HurtBox _hurtBox;
        public HurtBox HurtBox
        {
            get { return _hurtBox ? _hurtBox : (_hurtBox = GetComponent<HurtBox>()); }
            set => _hurtBox = value;
        }

        private ChemObject _chem;
        public ChemObject Chem
        {
            get { return _chem ? _chem : (_chem = GetComponent<ChemObject>()); }
            set => _chem = value; 
        }

        private StatusManager _status;
        public StatusManager Status
        {
            get { return _status ? _status : (_status = GetComponent<StatusManager>()); }
            set => _status = value; 
        }

        private HitBox _hitBox;
        public HitBox HitBox
        {
            get { return _hitBox ? _hitBox : (_hitBox = GetComponent<HitBox>()); }
            set => _hitBox = value; 
        }

        private SpriteManager _spriteManager;
        public SpriteManager SpriteManager
        {
            get { return _spriteManager ? _spriteManager : (_spriteManager = GetComponentInChildren<SpriteManager>()); }
            set => _spriteManager = value;
        }

        private SpriteRenderer _sprite;
        public SpriteRenderer Sprite
        {
            get => _sprite ? _sprite : 
                    _spriteManager? (_sprite = _spriteManager) : 
                    _sprite = GetComponentInChildren<SpriteRenderer>();
            set => _sprite = value;
        }

        private Animator _anim;
        public Animator Anim
        {
            get { return _anim ? _anim : (_anim = GetComponentInChildren<Animator>()); }
            set => _anim = value; 
        }

        private EmitterBehavior _emitter;
        public EmitterBehavior Emitter
        {
            get { return _emitter ? _emitter : (_emitter = GetComponent<EmitterBehavior>()); }
            set => _emitter = value;
        }

        private InputController _controller;
        public InputController Controller
        {
            get { return _controller ? _controller : (_controller = GetComponent<InputController>()); }
            set => _controller = value;
        }

        private StateManager _stateManager;
        public StateManager StateManager
        {
            get { return _stateManager ? _stateManager : (_stateManager = GetComponent<StateManager>()); }
            set => _stateManager = value;
        }

        private SkillManager _skillManager;
        public SkillManager SkillManager
        {
            get { return _skillManager ? _skillManager : (_skillManager = GetComponent<SkillManager>()); }
            set => _skillManager = value;
        }

        #endregion

        public int faceDir = 1;

        private void Update()
        {
            Animate();
        }

        private void Animate()
        {
            if (Sprite != null)
            {
                Vector2 scale = Sprite.transform.localScale;
                scale.x = faceDir * Mathf.Abs(scale.x);
                Sprite.transform.localScale = scale;
            }
        }
    }
}
