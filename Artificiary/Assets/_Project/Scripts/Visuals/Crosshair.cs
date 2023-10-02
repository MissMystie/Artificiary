using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField] private InputController controller;
        [SerializeField] private Transform aimPoint;
        [SerializeField] private SpriteRenderer crosshairPrefab;
        [SerializeField] private float distance = 8;

        private SpriteRenderer crosshair;
        [SerializeField] private float sensitivity = 1;

        void Awake()
        {
            controller = GetComponent<InputController>();
            if (aimPoint == null) aimPoint = transform;

            if (crosshairPrefab != null) 
                crosshair = Instantiate(crosshairPrefab, 
                    Vector2.zero, Quaternion.identity, 
                    DynamicObjects.Instance).GetComponent<SpriteRenderer>();

            //isOnGamepad = GetComponent<InputController>()?.input.currentControlScheme == InputController.GAMEPAD_CTRL;
        }

        void Update()
        {
            if (crosshair != null)
                UpdateCrosshair();
        }

        private void UpdateCrosshair()
        {
            if (crosshair == null) return;

            if (controller.Input.currentControlScheme == InputController.KEYBOARD_CTRL)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(controller.aimAction.ReadValue<Vector2>());
                crosshair.transform.position = mousePos;
                crosshair.gameObject.SetActive(true);
            }
            else if(controller.Input.currentControlScheme == InputController.GAMEPAD_CTRL)
            {
                Vector2 newPos = aimPoint.position + (Vector3)controller.aim.normalized * distance;

                float step = sensitivity * Time.deltaTime;

                crosshair.transform.position = Vector2.MoveTowards(crosshair.transform.position, newPos, step);

                crosshair.gameObject.SetActive(controller.aim != Vector2.zero);
            }
        }
    }
}
