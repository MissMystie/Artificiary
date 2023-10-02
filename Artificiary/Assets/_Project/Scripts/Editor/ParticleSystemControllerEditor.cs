using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie.ETM.MurkEditor
{
    [CustomEditor(typeof(ParticleSystemController))]
    public class ParticleSystemControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                ParticleSystemController ps = (ParticleSystemController)target;

                if (ps.setShape)
                {
                    ps.UpdateShape();
                }
            }
        }
    }
}
