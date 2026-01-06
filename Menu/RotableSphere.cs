using UnityEngine;
using UnityEngine.InputSystem;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Controla la rotación interactiva de la esfera en el menú. 
    /// </summary>
    public class RotatableSphereInputSystem : MonoBehaviour
    {
        [Header("Configuración de Movimiento")]
        [Tooltip("Sensibilidad de la rotación al arrastrar.")]
        [SerializeField] private float rotationSpeed = 0.2f;
        
        [Tooltip("Fuerza del frenado tras soltar la esfera. Valores más altos frenan antes.")]
        [SerializeField] private float inertiaDamping = 5f;
        
        [Tooltip("Velocidad de rotación pasiva cuando el usuario no toca la pantalla.")]
        [SerializeField] private float autoRotationSpeed = 10f;

        private bool dragging = false;
        private Vector2 previousTouchPos;
        private Vector2 currentVelocity;


        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            HandleInput();
            ApplyPhysics();
        }

        /// <summary>
        /// Detecta el arrastre en la pantalla táctil y el movimiento del puntero.
        /// </summary>
        private void HandleInput()
        {
            if (Pointer.current == null) return;

            // Detectar si se está presionando
            if (Pointer.current.press.isPressed)
            {
                Vector2 currentPos = Pointer.current.position.ReadValue();

                if (!dragging)
                {
                    // Inicio del arrastre
                    dragging = true;
                    previousTouchPos = currentPos;
                    currentVelocity = Vector2.zero;
                }
                else
                {
                    // Calcular diferencia de movimiento
                    Vector2 delta = currentPos - previousTouchPos;

                    // Rotar el objeto en base al movimiento del puntero
                    RotateObject(delta.x, delta.y, rotationSpeed);

                    // Calcular velocidad para la inercia
                    currentVelocity = delta / Time.deltaTime;
                    previousTouchPos = currentPos;
                }
            }
            else
            {
                dragging = false;
            }
        }

        /// <summary>
        /// Aplica la inercia o la rotación automática.
        /// </summary>
        private void ApplyPhysics()
        {
            if (dragging) return;

            // Si aún queda velocidad, aplicar efecto de inercia
            if (currentVelocity.sqrMagnitude > 0.01f)
            {
                Vector2 frameDelta = currentVelocity * Time.deltaTime;
                RotateObject(frameDelta.x, frameDelta.y, rotationSpeed);

                // Reducir la velocidad progresivamente (Damping)
                currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, inertiaDamping * Time.deltaTime);
            }
            else
            {
                // Si está quieto, aplicar rotación automática sobre el eje Y
                transform.Rotate(Vector3.up, autoRotationSpeed * Time.deltaTime, Space.World);
            }
        }

        /// <summary>
        /// Aplica la rotación física.
        /// </summary>
        /// <param name="horizontal">Desplazamiento en X.</param>
        /// <param name="vertical">Desplazamiento en Y.</param>
        /// <param name="speed">Multiplicador de velocidad.</param>
        private void RotateObject(float horizontal, float vertical, float speed)
        {
            // Invertimos horizontal para que el giro siga la dirección del dedo
            transform.Rotate(Vector3.up, -horizontal * speed, Space.World);
            transform.Rotate(Vector3.right, vertical * speed, Space.World);
        }
    }
}