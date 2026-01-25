using UnityEngine;
using UnityEngine.InputSystem;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Controla la cámara orbital permitiendo la rotación alrededor del jugador.
    /// </summary>
    public class TouchAndMouseCameraController : MonoBehaviour
    {
        [Header("Configuración de Seguimiento")]
        [Tooltip("La bola o jugador al que la cámara debe orbitar.")]
        [SerializeField] Transform target;
        
        [Tooltip("Distancia fija entre la cámara y la bola.")]
        [SerializeField] float distance = 7f;
        
        [Tooltip("Ajuste vertical del punto de mira (LookAt) para no mirar a los 'pies' de la bola.")]
        [SerializeField] float heightOffset = 0.5f;

        [Header("Configuración de Rotación")]
        [Tooltip("Sensibilidad del movimiento de la cámara.")]
        [SerializeField] float rotationSpeed = 10f;
        private float horizontalAngle;
        private float verticalAngle = 20f;
        private PlayerControls controls;
    

        /// <summary>
        /// Inicializa la clase generada por el Input System.
        /// </summary>
        void Awake()
        {
            controls = new PlayerControls();
            rotationSpeed = PlayerPrefs.GetFloat("CameraSensitivity", 0.5f) * 15f;
        }

        /// <summary>
        /// Activa los controles.
        /// </summary>
        void OnEnable()
        {
            controls.Enable();
        }
        
        /// <summary>
        /// Desactiva los controles.
        /// </summary>
        void OnDisable()
        {
            controls.Disable();
        }
        
        void Start()
        {
            horizontalAngle = transform.eulerAngles.y;
        }

        /// <summary>
        /// Actualiza la cámara dependiendo de si se usa ratón o pantalla táctil.
        /// </summary>
        void Update()
        {
            if (target == null) return;

            // Lee el valor del movimiento del ratón o el deslizamiento táctil (Delta)
            Vector2 lookDelta = controls.Camera.Look.ReadValue<Vector2>();

            // Actualiza ángulos basados en la entrada del usuario
            horizontalAngle += lookDelta.x * rotationSpeed * Time.deltaTime;
            verticalAngle -= lookDelta.y * rotationSpeed * Time.deltaTime;

            // Restricción para que la cámara no dé la vuelta completa verticalmente
            // Impide que el jugador mire desde debajo del suelo o totalmente desde arriba
            verticalAngle = Mathf.Clamp(verticalAngle, 10f, 80f);

            // Convertierte ángulos en una rotación de Unity
            Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);

            // Calcula la dirección hacia la que apunta la rotación
            Vector3 direction = rotation * Vector3.forward;

            // Posiciona la cámara
            transform.position = target.position - direction * distance;

            // Orienta la cámara hacia el objetivo
            transform.LookAt(target.position + Vector3.up * heightOffset);
        }
    }
}