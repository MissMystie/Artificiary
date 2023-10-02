using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie.ETM.MurkEditor
{
    [CustomEditor(typeof(RopeController))]
    public class RopeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                RopeController rope = (RopeController)target;

                if (rope.line != null)
                {
                    rope.line.coordinateMode = LineController.CoordinateMode.GLOBAL;

                    if (rope.anchorA != null && rope.anchorB != null)
                    {
                        List<Vector3> anchorPositions = new List<Vector3>();
                        anchorPositions.Add(rope.anchorA.position);
                        anchorPositions.Add(rope.anchorB.position);

                        rope.line.positions = anchorPositions;
                    }
                    else
                    {
                        rope.line.positions.Clear();
                    }
                }
            }
        }
    }
}
