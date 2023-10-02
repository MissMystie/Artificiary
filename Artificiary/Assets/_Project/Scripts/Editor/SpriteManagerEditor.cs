using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mystie.ETM.MurkEditor
{
    [CustomEditor(typeof(SpriteManager))]
    public class SpriteManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                SpriteManager spriteManager = (SpriteManager)target;
                spriteManager.UpdateSprites();
            }
        }
    }
}
