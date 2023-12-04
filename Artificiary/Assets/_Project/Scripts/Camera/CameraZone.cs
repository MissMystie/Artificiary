using Cinemachine;
using LDtkUnity;
using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class CameraZone : MonoBehaviour, ILDtkImportedFields
    {
        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private CinemachineConfiner2D confiner;

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

        public void OnLDtkImportFields(LDtkFields fields)
        {
            fields.TryGetBool("follow_player", out followPlayer);

            bool useConfiner;
            fields.TryGetBool("use_confiner", out useConfiner);
            confiner.enabled = useConfiner;
        }

        private void Reset()
        {
            vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        private void OnTriggerEnter2D(Collider2D col) {
            if (vcam == null) return;

            if (col.gameObject.HasTag(Tags.PLAYER_TAG))
            {
                vcam.Priority = priority;
                if (followPlayer) vcam.Follow = col.gameObject.transform;
            }
        }

        private void OnTriggerExit2D(Collider2D col) {
            if (vcam == null) return;

            if (col.gameObject.HasTag(Tags.PLAYER_TAG))
            {
                vcam.Priority = basePriority;
                if (followPlayer) vcam.Follow = null;
            }
        }
    }
}
