using UnityEngine;
using UnityEngine.InputSystem;

namespace PJML.RushAndRoll
{
    public class TouchAndMouseCameraController : MonoBehaviour
    {
        public Transform target;
        public float distance = 7f;
        public float heightOffset = 0.5f;
        public float rotationSpeed = 10f;

        private float horizontalAngle = 120f;
        private float verticalAngle = 20f;
        private PlayerControls controls;

        void Awake()
        {
            controls = new PlayerControls();
        }

        void OnEnable()
        {
            controls.Enable();
        }

        void OnDisable()
        {
            controls.Disable();
        }

        void Update()
        {
            Vector2 lookDelta = controls.Camera.Look.ReadValue<Vector2>();

            horizontalAngle += lookDelta.x * rotationSpeed * Time.deltaTime;
            verticalAngle -= lookDelta.y * rotationSpeed * Time.deltaTime;

            verticalAngle = Mathf.Clamp(verticalAngle, 10f, 80f);

            Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);

            Vector3 direction = rotation * Vector3.forward;

            transform.position = target.position - direction * distance;

            transform.LookAt(target.position + Vector3.up * heightOffset);
        }
    }
}