using LDtkUnity;
using LDtkUnity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.MystEditor
{
    public class MultiTagLDtkPostporcessor : LDtkPostprocessor
    {
        protected override void OnPostprocessLevel(GameObject root, LdtkJson projectJson)
        {
            Debug.Log($"Post process LDtk level: {root.name}");
            foreach (Transform layer in root.transform)
            {
                foreach (Transform tilemapOrEntity in layer)
                {
                    string tag = tilemapOrEntity.tag;
                    if (tag != "Untagged")
                    {
                        tilemapOrEntity.gameObject.AddTag(tag);
                        Debug.Log("tag: " + tag);
                    }
                }
            }
        }
    }
}
