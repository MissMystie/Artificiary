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

        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Collider2D col;
        public bool setShape = true;

        [SerializeField] private float pointSize = 0.0625f;

        [Space]

        [SerializeField] private bool scaleToArea = false;
        [SerializeField, ShowIf("scaleToArea")] private float emissionRate = 10f;
        [SerializeField] private bool scaleMaxParticles = false;
        [SerializeField, ShowIf("scaleMaxParticles")] private float maxParticles = 1;

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

        #region Set Shape 

        public bool ValidateShape()
        {
            switch (shape)
            {
                case Shape.SPRITE:

                    if (sprite == null) return false;

                    break;

                case Shape.CIRCLE:

                    if (col == null) return false;

                    CircleCollider2D circle = (CircleCollider2D)col;
                    if (circle == null) return false;

                    break;
                case Shape.BOX:

                    if (col == null) return false;

                    BoxCollider2D box = (BoxCollider2D)col;
                    if (box == null) return false;

                    break;
                case Shape.LINE:

                    if (col == null) return false;

                    EdgeCollider2D line = (EdgeCollider2D)col;
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

        public void SetShape(SpriteRenderer _sprite)
        {
            if (_sprite == null)
            {
                Debug.LogWarning("Missing particle system.", this);
                SetShape();
                return;
            }

            shape = Shape.SPRITE;
            sprite = _sprite;

            UpdateShape();
        }

        public void SetShape(Collider2D _col)
        {
            if (_col == null)
            {
                Debug.LogWarning("Missing particle system.", this);
                SetShape();
                return;
            }

            col = _col;

            if (col.GetType() == typeof(BoxCollider2D))
            {
                shape = Shape.BOX;
            }
            else if (col.GetType() == typeof(CircleCollider2D))
            {
                shape = Shape.CIRCLE;
            }
            else if (col.GetType() == typeof(EdgeCollider2D))
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
            s.radius = pointSize;
        }

        public void UpdateSpriteShape(ParticleSystem.ShapeModule s)
        {
            s.shapeType = ParticleSystemShapeType.SpriteRenderer;
            s.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            s.spriteRenderer = sprite;
        }

        public void UpdateCircleShape(ParticleSystem.ShapeModule s)
        {
            CircleCollider2D circle = (CircleCollider2D)col;

            s.shapeType = ParticleSystemShapeType.Circle;
            s.radius = circle.radius * col.transform.localScale.x;

            s.position = new Vector2(circle.offset.x * col.transform.localScale.x, circle.offset.y * col.transform.localScale.y);
        }

        public void UpdateBoxShape(ParticleSystem.ShapeModule s)
        {
            BoxCollider2D box = (BoxCollider2D)col;
            s.shapeType = ParticleSystemShapeType.Rectangle;

            Vector3 scale = new Vector2(box.size.x * col.transform.localScale.x, box.size.y * col.transform.localScale.y);
            scale.z = 1;
            s.scale = scale;

            s.position = new Vector2(box.offset.x * col.transform.localScale.x, box.offset.y * col.transform.localScale.y);
        }

        public void UpdateEdgeShape(ParticleSystem.ShapeModule s)
        {
            BoxCollider2D box = (BoxCollider2D)col;
            s.shapeType = ParticleSystemShapeType.BoxEdge;

            Vector3 scale = new Vector2(box.size.x * col.transform.localScale.x, box.size.y * col.transform.localScale.y);
            scale.z = 1;
            s.scale = scale;

            s.position = new Vector2(box.offset.x * col.transform.localScale.x, box.offset.y * col.transform.localScale.y);
        }

        public void UpdateLineShape(ParticleSystem.ShapeModule s)
        {
            EdgeCollider2D line = (EdgeCollider2D)col;

            s.shapeType = ParticleSystemShapeType.SingleSidedEdge;

            Vector2 startPos = line.points[0];
            Vector2 endPos = line.points[line.points.Count() - 1];
            float halfLength = (endPos - startPos).magnitude / 2;

            s.radius = halfLength * col.transform.localScale.x;

            s.position = new Vector2(line.offset.x * col.transform.localScale.x, line.offset.y * col.transform.localScale.y);
        }

        #endregion

        public float GetArea(ParticleSystem.ShapeModule s)
        {
            float area = 1;

            switch (shape)
            {
                case Shape.SPRITE:
                    area = sprite.bounds.size.x * sprite.bounds.size.y;
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

            if (scaleToArea)
            {
                var emission = ps.emission;
                emission.rateOverTimeMultiplier = emissionRate * area;
            }

            if (scaleMaxParticles)
            {
                var main = ps.main;
                main.maxParticles = (int)(maxParticles * area);
            }
        }

        private void Reset()
        {
            effects = new List<ParticleSystem>();
            effects = GetComponentsInChildren<ParticleSystem>().ToList();
        }
    }
}
