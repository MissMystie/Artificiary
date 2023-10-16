using Mystie.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie
{
    public class ParticleSystemController : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> effects = new List<ParticleSystem>();

        public enum Shape { CUSTOM, POINT, SPRITE, CIRCLE, BOX, LINE, EDGE }
        [SerializeField] private Shape shape = Shape.CUSTOM;

        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Collider2D _col;
        public bool setShape = true;

        [SerializeField] private float _pointSize = 0.0625f;

        [Space]

        [SerializeField] private bool _scaleToArea = false;
        [SerializeField, ShowIf("scaleToArea")] private float _emissionRate = 10f;
        [SerializeField] private bool _scaleMaxParticles = false;
        [SerializeField, ShowIf("scaleMaxParticles")] private float _maxParticles = 1;

        private void Update()
        {
            UpdateShape();
        }

        public void UpdateShape()
        {
            if (effects.IsNullOrEmpty() || !ValidateShape()) return;

            foreach (ParticleSystem ps in effects)
            {
                if (ps == null)
                {
                    //Debug.LogWarning("Missing particle system.", this);
                    continue;
                }

                if (setShape)
                {
                    ParticleSystem.ShapeModule s = ps.shape;

                    switch (shape)
                    {
                        case Shape.POINT:
                            UpdatePointShape(s);
                            break;
                        case Shape.SPRITE:
                            UpdateSpriteShape(s);
                            break;
                        case Shape.CIRCLE:
                            UpdateCircleShape(s);
                            break;
                        case Shape.BOX:
                            UpdateBoxShape(s);
                            break;
                        case Shape.LINE:
                            UpdateLineShape(s);
                            break;
                        case Shape.EDGE:
                            UpdateEdgeShape(s);
                            break;
                    }
                }

                ScaleToArea(ps);

                /* 
                 var main = ps.main;
                    main.startLifetimeMultiplier = col.size.y / main.startSpeedMultiplier;

                    var shape = ps.shape;
                    shape.radius = col.bounds.size.x / 2;
                 */
            }
        }

        #region Particle Systems

        public void Play()
        {
            if (effects.IsNullOrEmpty()) return;

            gameObject.SetActive(true);

            foreach (ParticleSystem ps in effects)
            {
                if (ps != null) ps.Play();
                else Debug.LogError("PFXSystem: no particle system assigned. (" + gameObject.name + ")");
            }
        }

        public void Stop()
        {
            foreach (ParticleSystem ps in effects)
                ps.Stop();

            gameObject.SetActive(false);
        }

        public void DetachParticles(bool setAutodestroy = true, bool destroy = false)
        {
            if (effects.IsNullOrEmpty()) return;

            foreach (ParticleSystem ps in effects)
            {
                if (ps == null) continue;
                ps.transform.parent = null; // splits particles off so it doesn't get deleted with the parent
                var main = ps.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
                ps.Stop(); // this stops the particle from creating more bits
                SetShape();
            }

            //gameObject.SetActive(false);
            //Destroy(this);
        }

        #endregion

        #region Set Shape 

        public bool ValidateShape()
        {
            switch (shape)
            {
                case Shape.SPRITE:

                    if (_sprite == null) return false;

                    break;

                case Shape.CIRCLE:

                    if (_col == null) return false;

                    CircleCollider2D circle = (CircleCollider2D)_col;
                    if (circle == null) return false;

                    break;
                case Shape.BOX:

                    if (_col == null) return false;

                    BoxCollider2D box = (BoxCollider2D)_col;
                    if (box == null) return false;

                    break;
                case Shape.LINE:

                    if (_col == null) return false;

                    EdgeCollider2D line = (EdgeCollider2D)_col;
                    if (line == null) return false;

                    break;
            }

            return true;
        }

        public void SetShape()
        {
            shape = Shape.POINT;
            UpdateShape();
        }

        public void SetShape(SpriteRenderer sprite)
        {
            if (sprite == null)
            {
                Debug.LogWarning("Missing particle system.", this);
                SetShape();
                return;
            }

            shape = Shape.SPRITE;
            _sprite = sprite;

            UpdateShape();
        }

        public void SetShape(Collider2D col)
        {
            if (col == null)
            {
                Debug.LogWarning("Missing particle system.", this);
                SetShape();
                return;
            }

            _col = col;

            if (_col.GetType() == typeof(BoxCollider2D))
            {
                shape = Shape.BOX;
            }
            else if (_col.GetType() == typeof(CircleCollider2D))
            {
                shape = Shape.CIRCLE;
            }
            else if (_col.GetType() == typeof(EdgeCollider2D))
            {
                shape = Shape.LINE;
            }

            UpdateShape();
        }

        #endregion

        #region Update Shape Properties

        public void UpdatePointShape(ParticleSystem.ShapeModule s)
        {
            s.shapeType = ParticleSystemShapeType.Circle;
            s.radius = _pointSize;
        }

        public void UpdateSpriteShape(ParticleSystem.ShapeModule s)
        {
            s.shapeType = ParticleSystemShapeType.SpriteRenderer;
            s.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            s.spriteRenderer = _sprite;
        }

        public void UpdateCircleShape(ParticleSystem.ShapeModule s)
        {
            CircleCollider2D circle = (CircleCollider2D)_col;

            s.shapeType = ParticleSystemShapeType.Circle;
            s.radius = circle.radius * _col.transform.localScale.x;

            s.position = new Vector2(circle.offset.x * _col.transform.localScale.x, 
                circle.offset.y * _col.transform.localScale.y);
        }

        public void UpdateBoxShape(ParticleSystem.ShapeModule s)
        {
            BoxCollider2D box = (BoxCollider2D)_col;
            s.shapeType = ParticleSystemShapeType.Rectangle;

            Vector3 scale = new Vector2(box.size.x * _col.transform.localScale.x, 
                box.size.y * _col.transform.localScale.y);
            scale.z = 1;
            s.scale = scale;

            s.position = new Vector2(box.offset.x * _col.transform.localScale.x, 
                box.offset.y * _col.transform.localScale.y);
        }

        public void UpdateEdgeShape(ParticleSystem.ShapeModule s)
        {
            BoxCollider2D box = (BoxCollider2D)_col;
            s.shapeType = ParticleSystemShapeType.BoxEdge;

            Vector3 scale = new Vector2(box.size.x * _col.transform.localScale.x, 
                box.size.y * _col.transform.localScale.y);
            scale.z = 1;
            s.scale = scale;

            s.position = new Vector2(box.offset.x * _col.transform.localScale.x, 
                box.offset.y * _col.transform.localScale.y);
        }

        public void UpdateLineShape(ParticleSystem.ShapeModule s)
        {
            EdgeCollider2D line = (EdgeCollider2D)_col;

            s.shapeType = ParticleSystemShapeType.SingleSidedEdge;

            Vector2 startPos = line.points[0];
            Vector2 endPos = line.points[line.points.Count() - 1];
            float halfLength = (endPos - startPos).magnitude / 2;

            s.radius = halfLength * _col.transform.localScale.x;

            s.position = new Vector2(line.offset.x * _col.transform.localScale.x, 
                line.offset.y * _col.transform.localScale.y);
        }

        #endregion

        public float GetArea(ParticleSystem.ShapeModule s)
        {
            float area = 1;

            switch (shape)
            {
                case Shape.SPRITE:
                    area = _sprite.bounds.size.x * _sprite.bounds.size.y;
                    break;
                case Shape.CIRCLE:
                    area = Mathf.PI * Mathf.Pow(s.radius, 2);

                    break;
                case Shape.BOX:
                    area = s.scale.x * s.scale.y;
                    break;
            }

            return area;
        }

        public void ScaleToArea(ParticleSystem ps)
        {
            float area = GetArea(ps.shape);

            if (_scaleToArea)
            {
                var emission = ps.emission;
                emission.rateOverTimeMultiplier = _emissionRate * area;
            }

            if (_scaleMaxParticles)
            {
                var main = ps.main;
                main.maxParticles = (int)(_maxParticles * area);
            }
        }

        private void Reset()
        {
            effects = new List<ParticleSystem>();
            effects = GetComponentsInChildren<ParticleSystem>().ToList();
        }
    }
}
