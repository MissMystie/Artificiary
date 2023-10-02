using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mystie
{
    public class VersionNumberLabel : MonoBehaviour
    {
        [SerializeField] private string prefix = "v";
        [SerializeField] private TextMeshProUGUI label;

        private void Start()
        {
            label.text = prefix + Application.version;
        }
    }

}
