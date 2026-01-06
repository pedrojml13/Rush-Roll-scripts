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
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private Renderer ballRenderer;
        private bool wasFalling = false;
        private Vector3 movement = Vector3.zero;
        private Vector3 neutralTilt = Vector3.zero;
        private bool isGrounded = false;
        [SerializeField] private float jumpForce = 5f;
        private float lastTapTime = 0f;
        private float doubleTapThreshold = 0.3f;
        private int tapCount = 0;
        private float touchStartTime = 0f;

        /// <summary>
        /// Inicializa la instancia del singleton de esta clase.
        /// </summary>
        /// <remarks>
        /// Este método asegura que solo exista una única instancia de este objeto en la escena.
        /// Si ya existe otra instancia, el objeto duplicado se destruye automáticamente.
        /// </remarks>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            ballRenderer.material = GameManager.Instance.GetSelectedBallSkin().material;
            Instance = this;

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

        private void Update()
        {
            if (Accelerometer.current != null)
            {
                // Móvil: movimiento con giroscopio
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
                // Editor: movimiento con teclado
                float moveX = 0f;
                float moveZ = 0f;

                if (Keyboard.current != null)
                {
                    if (Keyboard.current.wKey.isPressed) moveZ += 1f;
                    if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
                    if (Keyboard.current.dKey.isPressed) moveX += 1f;
                    if (Keyboard.current.aKey.isPressed) moveX -= 1f;
                }

                movement = new Vector3(moveX, 0f, moveZ);
            }

            // También aquí: salto con espacio o doble toque
            CheckGrounded();

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                TryJump();
            }

            if (Touchscreen.current != null)
            {

                var touch = Touchscreen.current.primaryTouch;

                var phase = touch.phase.ReadValue();

                if (phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    touchStartTime = Time.unscaledTime;
                }

                if (phase == UnityEngine.InputSystem.TouchPhase.Ended)
                {
                    float touchDuration = Time.unscaledTime - touchStartTime;

                    if (touchDuration < 0.2f)
                    {
                        TryJump();
                    }

                    touchStartTime = 0f; // Reinicia para el siguiente toque
                }

            }
        }

        private void CheckGrounded()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.6f);
        }

        private void TryJump()
        {
            if (isGrounded && !GameManager.Instance.IsPaused())
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                AudioManager.Instance.PlaySFX(jumpClip);
            }
        }

        private void FixedUpdate()
        {
            wasFalling = rb.linearVelocity.y < -0.5f;

            rb.AddForce(Vector3.up * airResistance, ForceMode.Acceleration);

            // Ajustar movimiento según cámara
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            Vector3 adjustedMovement = right * movement.x + forward * movement.z;
            Vector3 horizontalVelocity = adjustedMovement * speed;

            rb.AddForce(horizontalVelocity, ForceMode.Acceleration);
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