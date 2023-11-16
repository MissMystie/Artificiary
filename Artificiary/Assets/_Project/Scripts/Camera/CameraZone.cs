using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class CameraZone : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vcam;

        private int basePriority = 10;
        [SerializeField] private int priority = 15;
        [SerializeField] private bool followPlayer;

        private void Start()
        {
            if (vcam == null)
                vcam = GetComponentInChildren<CinemachineVirtualCamera>();

            if (vcam != null)
                basePriority = vcam.Priority;
            else
                Debug.LogWarning("No vCam associated. (" + gameObject.name + ")", this);
        }

        private void Reset()
        {
            vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        private void OnTriggerEnter2D(Collider2D col) {
            if (vcam == null) return;

            if (col.gameObject.HasTag("Player"))
            {
                vcam.Priority = priority;
                if (followPlayer) vcam.Follow = col.gameObject.transform;
            }
        }

        private void OnTriggerExit2D(Collider2D col) {
            if (vcam == null) return;

            if (col.gameObject.HasTag("Player"))
            {
                vcam.Priority = basePriority;
                if (followPlayer) vcam.Follow = null;
            }
        }
    }
}
