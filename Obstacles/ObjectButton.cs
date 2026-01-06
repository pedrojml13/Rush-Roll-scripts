using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona un botón físico cambiando su posición visual 
    /// y su estado lógico cuando un objeto entra en su Trigger.
    /// </summary>
    public class ObjectButton : MonoBehaviour
    {
        [Header("Estado")]
        [Tooltip("Indica si el botón está actualmente presionado.")]
        public bool pressed = false;

        [Header("Configuración de Movimiento")]
        [Tooltip("Distancia que bajará el botón en el eje Y local al ser presionado.")]
        [SerializeField] private float moveY = 0.2f;

        [Header("Audio")]
        [SerializeField] private AudioClip buttonPressed;
        [SerializeField] private AudioClip buttonReleased;

        /// <summary>
        /// Se activa cuando un objeto con Collider entra en el área del botón.
        /// </summary>
        /// <param name="other">Objeto que entra en el área.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (pressed) return;

            pressed = true;
            AudioManager.Instance.PlaySFX(buttonPressed);

            Vector3 newPos = transform.localPosition;
            newPos.y -= moveY;
            transform.localPosition = newPos;
        }

        /// <summary>
        /// Cuando el objeto sale del área, se desactiva el botón.
        /// </summary>
        /// <param name="other">Objeto que sale del área.</param>
        private void OnTriggerExit(Collider other)
        {
            if (!pressed) return;

            pressed = false;
            AudioManager.Instance.PlaySFX(buttonReleased);

            Vector3 newPos = transform.localPosition;
            newPos.y += moveY;
            transform.localPosition = newPos;
        }
    }
}