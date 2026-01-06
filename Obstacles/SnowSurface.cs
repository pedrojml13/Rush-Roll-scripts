using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Simula una superficie de nieve blanda que ralentiza al jugador
    /// y le hace hundirse progresivamente mientras permanece sobre ella.
    /// </summary>
    public class SnowSurface : MonoBehaviour
    {
        [Tooltip("Resistencia aplicada al movimiento del jugador dentro de la nieve")]
        [SerializeField] private float dragInSnow = 5f;

        [Tooltip("Velocidad a la que la nieve se hunde mientras el jugador permanece sobre ella")]
        [SerializeField] private float sinkSpeed = 0.5f;


        /// <summary>
        /// Mientras el jugador permanece en la nieve:
        /// - Se reduce progresivamente la altura del collider simulando hundimiento
        /// - Se incrementa el drag del jugador para ralentizarlo
        /// </summary>
        /// <param name="other">Collider que permanece en contacto.</param>
        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            BoxCollider col = GetComponent<BoxCollider>();
            col.size = new Vector3(
                col.size.x,
                col.size.y - sinkSpeed * Time.deltaTime,
                col.size.z
            );

            other.GetComponent<Rigidbody>().linearDamping = dragInSnow;

            // Cuando la nieve se hunde demasiado, deja de colisionar
            if (col.size.y < 0.5f)
            {
                col.isTrigger = true;
            }
        }

        /// <summary>
        /// Al salir de la nieve:
        /// - El collider recupera su tamaño original
        /// - El jugador vuelve a su drag normal
        /// </summary>
        /// <param name="other">Collider que entra en contacto.</param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            BoxCollider col = GetComponent<BoxCollider>();
            col.size = new Vector3(col.size.x, 2f, col.size.z);

            other.GetComponent<Rigidbody>().linearDamping = 0.1f;
            col.isTrigger = false;
        }
    }
}
