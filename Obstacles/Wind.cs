using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona el empuje del viento.
    /// </summary>
    public class Wind : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip windSound;

        [Header("Configuración del Viento")]
        [Tooltip("Dirección hacia la que sopla el viento en coordenadas globales.")]
        [SerializeField] private Vector3 windDirection = new Vector3(1f, 0f, 0f);

        [Tooltip("Fuerza del empuje. Valores altos pueden dificultar mucho el avance.")]
        [SerializeField] private float windStrength = 5f;

        /// <summary>
        /// Inicia en bucle el sonido del viento cuando el jugador entra en la zona.
        /// </summary>
        /// <param name="other">Objeto que entra en el área.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                AudioManager.Instance.PlayLoop("wind", windSound);
            }
        }

        /// <summary>
        /// Aplica la fuerza de empuje de forma continua mientras se esté dentro del área.
        /// </summary>
        /// <param name="other">Objeto que permanece en el área.</param>
        private void OnTriggerStay(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(windDirection.normalized * windStrength, ForceMode.Force);
            }
        }

        /// <summary>
        /// Detiene el sonido ambiental cuando el jugador sale de la corriente de aire.
        /// </summary>
        /// <param name="other">Objeto que sale del área,</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                AudioManager.Instance.StopLoop("wind");
            }
        }
    }
}