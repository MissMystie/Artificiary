using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie.MystEditor
{
    [CustomEditor(typeof(LineController))]
    public class LineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                LineController line = (LineController)target;

                line.UpdateLine();
            }
        }
    }
}
