using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie
{
    public class Parallax : MonoBehaviour
    {
        public Camera cam;
        public Vector2 screenBounds;

        public List<Layer> layers = new List<Layer>();


        void Start()
        {
            cam = Camera.main;

            foreach (Layer layer in layers)
            {
                layer.Init(screenBounds);
            }
        }

        void LateUpdate()
        {
            foreach (Layer layer in layers)
            {
                layer.ReposChildObjs(cam.transform.position, screenBounds);

                Vector2 dist = (Vector2)cam.transform.position - layer.startPos;

                dist.x *= layer.parallaxFactor.x;
                dist.y *= layer.parallaxFactor.y;

                Vector2 newPos = layer.startPos + dist;

                if (layer.anchor != null)
                    layer.anchor.transform.position = newPos;
            }
        }

        [System.Serializable]
        public class Layer
        {
            public Transform anchor;
            public Vector2 parallaxFactor;
            public float choke = 0.25f;
            public bool repeating;

            [HideInInspector] public Vector2 startPos;

            private float width;
            private float halfWidth;

            private float height;
            private float halfHeight;

            public Layer()
            {
                parallaxFactor = Vector2.one;
                repeating = true;
            }

            public void Init(Vector2 screenBounds)
            {
                if (!anchor) return;

                startPos = anchor.transform.position;

                Transform child = anchor.GetChild(0);
                if (child == null) return;

                SpriteRenderer sprite = child.GetComponent<SpriteRenderer>();

                //SpriteRenderer[] sprites = anchor.GetComponentsInChildren<SpriteRenderer>();
                //if (sprites == null || sprites.Length == 0) return;

                //SpriteRenderer sprite = sprites[0];
                
                if (sprite != null)
                {
                    width = sprite.bounds.size.x - choke;
                    halfWidth = sprite.bounds.extents.x - (choke / 2);

                    height = sprite.bounds.size.y - choke;
                    halfHeight = sprite.bounds.extents.y - (choke / 2);
                }

                int childs = 1;
                if (repeating)
                {
                    childs = (int)Mathf.Ceil(screenBounds.x * 2 / (width));

                    for (int i = 1; i <= childs - 1; i++)
                    {
                        GameObject c = Instantiate(sprite.gameObject, anchor);

                        //GameObject c = null;

                        //Don't instantiate children that are already buffered
                        /*if (!(i < sprites.Count() && (c = sprites[i].gameObject)))
                        {
                            c = Instantiate(sprite.gameObject, anchor);
                        }*/

                        if (repeating)
                        {
                            c.transform.position = new Vector3(width * i, anchor.position.y, anchor.position.z);
                            c.name = anchor.name + "_" + i;
                        }
                    }
                }
            }


            public void ReposChildObjs(Vector2 camPos, Vector2 screenBounds)
            {
                if (repeating && anchor.childCount > 1)
                {
                    Transform first = anchor.GetChild(0);
                    Transform last = anchor.GetChild(anchor.childCount - 1);

                    if (camPos.x + screenBounds.x > last.position.x + halfWidth)
                    {
                        first.SetAsLastSibling();
                        first.position = new Vector3(last.position.x + halfWidth * 2, last.position.y, last.position.z);
                    }
                    else if (camPos.x - screenBounds.x < first.position.x - halfWidth)
                    {
                        last.SetAsFirstSibling();
                        last.position = new Vector3(first.position.x - halfWidth * 2, first.position.y, first.position.z);
                    }
                }
            }
        }
    }
}
