using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona el comportamiento de una mina.
    /// </summary>
    public class Mine : MonoBehaviour
    {
        [Header("Configuración de Explosión")]
        [Tooltip("Efecto de sonido que se reproducirá al estallar.")]
        [SerializeField] private AudioClip explosionAudio;

        [Tooltip("Magnitud de la fuerza que se aplicará al jugador tras la colisión.")]
        private float explosionForce = 800f;

        /// <summary>
        /// Detecta la colisión física con el jugador.
        /// </summary>
        /// <param name="collision">Objeto que colisiona.</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Explode(collision.gameObject);
            }
        }

        /// <summary>
        /// Aplica fuerza física, sonido y elimina el objeto.
        /// </summary>
        /// <param name="player">El GameObject del jugador.</param>
        private void Explode(GameObject player)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            
            if (rb != null)
            {

                Vector3 spread = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(0.1f, 0.5f),
                    Random.Range(-0.5f, 0.5f)
                );

                Vector3 launchDirection = (transform.forward + spread).normalized;

                rb.AddForce(launchDirection * explosionForce, ForceMode.Impulse);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(explosionAudio);
            }

            Destroy(gameObject);
        }
    }
}