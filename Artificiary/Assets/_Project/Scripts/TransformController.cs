using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class TransformController : MonoBehaviour
    {
        [SerializeField] private bool fixedScale = true;
        [SerializeField] private bool fixedRotation = false;

        [SerializeField] private Vector3 scale = Vector3.one;
        [SerializeField] private Vector3 rotation = Vector3.one;

        private void Start()
        {
            scale = transform.localScale;
            rotation = transform.eulerAngles;
        }

        private void Update()
        {
            if (transform.parent == null) return;

            if (fixedScale)
            {
                Vector3 parentScale = transform.parent.localScale;
                Vector3 selfScale = new Vector3(scale.x / parentScale.x, scale.y / parentScale.y, scale.z / parentScale.z);
                transform.localScale = selfScale;
            }
            
            if (fixedRotation)
                transform.eulerAngles = rotation;
        }
    }
}
