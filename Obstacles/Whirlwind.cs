using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona un torbellino que atrae al jugador hacia su centro.
    /// </summary>
    public class Whirlwind : MonoBehaviour
    {
        [Header("Fuerzas del Vórtice")]
        [Tooltip("Fuerza constante que atrae al jugador hacia el centro del torbellino.")]
        [SerializeField] private float attractionForce = 50f;

        /// <summary>
        /// Mientras el jugador esté dentro del área del torbellino, 
        /// se le aplica una fuerza hacia el origen del objeto.
        /// </summary>
        /// <param name="other">Objeto que entra en el área.</param>
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (transform.position - rb.position).normalized;
                    rb.AddForce(direction * attractionForce * Time.deltaTime, ForceMode.VelocityChange);
                }
            }
        }

        /// <summary>
        /// Si el jugador colisiona con el torbellino, lo manda hacia abajo.
        /// </summary>
        /// <param name="collision">Objeto que colisiona.</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Teletransporta al jugador ligeramente por debajo del centro del torbellino
                    // Esto simula que el jugador ha sido "tragado" por el vórtice.
                    collision.gameObject.transform.position = transform.position - Vector3.up * 2f;
                }
            }
        }
    }
}