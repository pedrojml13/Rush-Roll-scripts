using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.Collections;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Controlador del comportamiento de la bola del jugador.
    /// </summary>
    public class BallController : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del LevelManager.
        /// </summary>
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
        private float touchStartTime = 0f;
        private Matrix4x4 calibrationMatrix;
        private float tiltSensitivity;

        /// <summary>
        /// Singleton, asegura que solo exista una instancia.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            //ballRenderer.material = GameManager.Instance.GetSelectedBallSkin().material;
            Instance = this;

        }

        /// <summary>
        /// Inicialización del controlador de la bola.
        /// </summary>
        void Start()
        {
            rb = GetComponent<Rigidbody>();            
            InputSystem.EnableDevice(Accelerometer.current);

            tiltSensitivity = PlayerPrefs.GetFloat("TiltSensitivity", 0.8f);

            StartCoroutine(DelayedCalibration());
        }

        /// <summary>
        /// Recalibra la inclinación del dispositivo para el control de la bola, espera a que el sensor esté listo.
        /// </summary>
        public IEnumerator DelayedCalibration()
        {
            // Si sigue dando cero, podemos esperar un frame más
            while (Accelerometer.current != null && Accelerometer.current.acceleration.ReadValue() == Vector3.zero)
            {
                yield return null;
            }

            Vector3 rawAcceleration = Accelerometer.current.acceleration.ReadValue();
        
            // Evita calibrar si el sensor devuelve (0,0,0) en el primer frame
            if (rawAcceleration == Vector3.zero) rawAcceleration = new Vector3(0, -1, 0); 

            Quaternion rotate = Quaternion.FromToRotation(rawAcceleration, new Vector3(0, 0, -1));
            calibrationMatrix = Matrix4x4.TRS(Vector3.zero, rotate, Vector3.one);

        }

        /// <summary>
        /// Actualización del controlador de la bola dependiendo del dispositivo.
        /// </summary>
        private void Update()
        {
            if (Accelerometer.current != null)
            {
                Vector3 rawTilt = Accelerometer.current.acceleration.ReadValue();

                // Pasamos el valor crudo por nuestra matriz de calibración para obtener la inclinación corregida
                Vector3 fixedTilt = calibrationMatrix.MultiplyVector(rawTilt);

                movement = new Vector3(fixedTilt.x, 0f, fixedTilt.y);
                movement *= tiltSensitivity;}
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

        /// <summary>
        /// Verifica si la bola está en el suelo.
        /// </summary>
        private void CheckGrounded()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.6f);
        }

        /// <summary>
        /// Intenta realizar un salto si la bola está en el suelo.
        /// </summary>
        private void TryJump()
        {
            if (isGrounded && !GameManager.Instance.IsPaused())
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                AudioManager.Instance.PlaySFX(jumpClip);
            }
        }

        /// <summary>
        /// Actualización física del controlador de la bola.
        /// </summary>
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

        /// <summary>
        /// Manejo de colisiones de la bola.
        /// </summary>
        /// <param name="collision">Objeto de colisión</param>
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