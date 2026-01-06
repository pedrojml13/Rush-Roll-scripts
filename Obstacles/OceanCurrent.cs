using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona zonas con corrientes de agua. 
    /// </summary>
    public class Water : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip waterCurrentAudio;

        [Header("Física de la Corriente")]
        [Tooltip("Magnitud de la fuerza aplicada. Valores más altos crean corrientes más fuertes.")]
        [SerializeField] private float waterForce = 10f;

        /// <summary>
        /// Detecta la entrada del jugador para efectos de sonido.
        /// </summary>
        /// <param name="other">Objeto que entra en el área.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(waterCurrentAudio);
            }
        }

        /// <summary>
        /// Mientras el objeto permanezca en el agua, se le aplica una fuerza continua.
        /// </summary>
        /// <param name="other">Objeto que se mantiene en el área.</param>
        private void OnTriggerStay(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 flowDirection = transform.forward;
                rb.AddForce(flowDirection * waterForce, ForceMode.Acceleration);
            }
        }
    }
}