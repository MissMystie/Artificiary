using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    // singleton
    public class DynamicObjects : MonoBehaviour
    {
        private static Transform instance;
        public static Transform Instance
        {
            get
            {
                if (instance != null) return instance;
                else return Create();
            }
        }

        private const string DYNAMIC_OBJECTS_NAME = "Dynamic Objects";

        private static Transform Create()
        {
            if (instance == null)
                instance = new GameObject(DYNAMIC_OBJECTS_NAME).transform;

            return instance;
        }
    }
}
