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
        public static Entity Get(GameObject target)
        {
            Entity entity = target.GetComponent<Entity>();
            if (entity == null) entity = target.AddComponent<Entity>();
            return entity;
        }

        [SerializeField] private PhysicsObject _phys;
        public PhysicsObject Phys
        {
            get { return _phys ? _phys : (_phys = GetComponent<PhysicsObject>()); }
            set { _phys = value; }
        }

        [SerializeField] private Collider2D _collider;
        public Collider2D Collider
        {
            get { return _collider ? _collider : (_collider = GetComponent<Collider2D>()); }
            set { _collider = value; }
        }

        [SerializeField] private InputController _controller;
        public InputController Controller
        {
            get { return _controller ? _controller : (_controller = GetComponent<InputController>()); }
            set { _controller = value; }
        }

        [SerializeField] private HealthManager _health;
        public HealthManager Health
        {
            get { return _health ? _health : (_health = GetComponent<HealthManager>()); }
            set { _health = value; }
        }

        [SerializeField] private ChemObject _chem;
        public ChemObject Chem
        {
            get { return _chem ? _chem : (_chem = GetComponent<ChemObject>()); }
            set { _chem = value; }
        }

        [SerializeField] private StatusManager _status;
        public StatusManager Status
        {
            get { return _status ? _status : (_status = GetComponent<StatusManager>()); }
            set { _status = value; }
        }

        [SerializeField] private HurtBox _hurtBox;
        public HurtBox HurtBox
        {
            get { return _hurtBox ? _hurtBox : (_hurtBox = GetComponent<HurtBox>()); }
            set { _hurtBox = value; }
        }

        [SerializeField] private HitBox _hitBox;
        public HitBox HitBox
        {
            get { return _hitBox ? _hitBox : (_hitBox = GetComponent<HitBox>()); }
            set { _hitBox = value; }
        }

        [SerializeField] private SpriteManager _sprite;
        public SpriteManager Sprite
        {
            get { return _sprite ? _sprite : (_sprite = GetComponentInChildren<SpriteManager>()); }
            set { _sprite = value; }
        }

        [SerializeField] private Animator _anim;
        public Animator Anim
        {
            get { return _anim ? _anim : (_anim = GetComponentInChildren<Animator>()); }
            set { _anim = value; }
        }

        [SerializeField] private StateManager _stateManager;
        public StateManager StateManager
        {
            get { return _stateManager ? _stateManager : (_stateManager = GetComponent<StateManager>()); }
            set { _stateManager = value; }
        }

        [SerializeField] private SkillManager _skillManager;
        public SkillManager SkillManager
        {
            get { return _skillManager ? _skillManager : (_skillManager = GetComponent<SkillManager>()); }
            set { _skillManager = value; }
        }
    }
}
