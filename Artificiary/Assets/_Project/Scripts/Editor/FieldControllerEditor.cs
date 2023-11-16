using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie.MystEditor
{
    [CustomEditor(typeof(FieldController))]
    public class FieldControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                FieldController field = (FieldController)target;

                field.UpdateSize();
            }
        }
    }
}
