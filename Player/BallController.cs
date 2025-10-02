using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using TMPro;

namespace PJML.RushAndRoll
{
    public class BallController : MonoBehaviour
    {
        public static BallController Instance { get; private set; }

        public float speed = 0.8f;
        public float minSpeed = 0.5f;
        public float maxSpeed = 20f;
        public float airResistance = 0f;
        private Rigidbody rb;
        private int gravity;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private AudioClip rollingClip;
        [SerializeField] private AudioClip landingClip;
        [SerializeField] private AudioClip collisionClip;
        private bool wasFalling = false;
        private Vector3 movement = Vector3.zero;
        private Vector3 neutralTilt = Vector3.zero;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            GetComponent<Renderer>().material.mainTexture = GameManager.Instance.GetSelectedBallSkin().texture;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();

            InputSystem.EnableDevice(Accelerometer.current);
        }

        public void RecalibrateTilt()
        {
            if (Accelerometer.current != null)
            {
                Vector3 raw = Accelerometer.current.acceleration.ReadValue();

                Vector3 neutral = new Vector3(raw.x, raw.y, raw.z);

                neutralTilt = neutral;
            }
        }


        void FixedUpdate()
        {
            wasFalling = rb.linearVelocity.y < -0.5f;

            rb.AddForce(Vector3.up * airResistance, ForceMode.Acceleration);

            if (Accelerometer.current != null)
            {
                // Mobile
                Vector3 rawTilt = Accelerometer.current.acceleration.ReadValue();
                Vector3 adjustedTilt = new Vector3(
                    rawTilt.x - neutralTilt.x,
                    0f,
                    rawTilt.z - neutralTilt.z
                );

                movement = new Vector3(adjustedTilt.x, 0f, -adjustedTilt.z);

            }

            else
            {
                // Editor
                float moveX = Keyboard.current.aKey.isPressed ? -1f :
                            Keyboard.current.dKey.isPressed ? 1f : 0f;

                float moveZ = Keyboard.current.sKey.isPressed ? -1f :
                            Keyboard.current.wKey.isPressed ? 1f : 0f;

                movement = new Vector3(moveX, 0, moveZ);
            }

            // Adjust movement based on camera orientation
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            Vector3 adjustedMovement = right * movement.x + forward * movement.z;

            Vector3 horizontalVelocity = adjustedMovement * speed;

            rb.AddForce(horizontalVelocity, ForceMode.VelocityChange);
            rb.AddForce(Vector3.up * Physics.gravity.y, ForceMode.Acceleration);

            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

            float normalizedVolume = Mathf.Clamp(rb.linearVelocity.magnitude / maxSpeed, 0.2f, 1f);

            if (rb.linearVelocity.magnitude >= minSpeed)
            {
                AudioManager.Instance.PlayLoop("Rolling", rollingClip, normalizedVolume);
            }
            else
            {
                AudioManager.Instance.StopLoop("Rolling");
            }

        }

        void OnCollisionEnter(Collision collision)
        {
            if (wasFalling && collision.relativeVelocity.y > 2f)
            {
                AudioManager.Instance.PlaySFX(landingClip);
            }

            if (collision.gameObject.CompareTag("Obstacle"))
            {
                AudioManager.Instance.PlaySFX(collisionClip);
            }
        }
    }
}