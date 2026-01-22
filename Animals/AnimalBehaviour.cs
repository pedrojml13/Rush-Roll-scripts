using UnityEngine;
using UnityEngine.AI;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Tipos de enemigos disponibles en el juego.
    /// Se utiliza para logros.
    /// </summary>
    public enum EnemyType { Crab, Spider, Scorpion, Jellyfish, Snowman, None }

    /// <summary>
    /// Controla el comportamiento base de los enemigos tipo animal:
    /// detección del jugador, persecución, ataque y muerte.
    /// </summary>
    public class AnimalBehavior : MonoBehaviour
    {
        [Header("Movement Settings")]

        [Tooltip("Distancia a la que el enemigo detecta y persigue al jugador")]
        [SerializeField] private float detectionRadius = 5f;

        [Tooltip("Referencia al transform del jugador")]
        [SerializeField] private Transform playerTransform;

        [Tooltip("Fuerza con la que el enemigo empuja al jugador al atacar")]
        [SerializeField] private float pushForce = 500f;

        [Tooltip("Tipo de enemigo (usado para logros y lógica externa)")]
        [SerializeField] private EnemyType enemyType;

        [SerializeField] private AudioClip attackClip;

        [Tooltip("Sonido que se reproduce al morir el enemigo")]
        [SerializeField] private AudioClip deathClip;

        private NavMeshAgent agent;
        private Animator animator;
        private Rigidbody targetRigidbody;

        private bool isDeath = false;

        /// <summary>
        /// Asigna el NavMeshAgent y el animator
        /// </summary>
        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Calcula si el jugador está dentro del rango de detección del enemigo
        /// </summary>
        private void Update()
        {
            if (playerTransform == null || isDeath) return;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= detectionRadius)
            {
                agent.SetDestination(playerTransform.position);
                animator.SetBool("isPlayerNear", true);
            }
            else
            {
                animator.SetBool("isPlayerNear", false);
                agent.ResetPath();
            }
        }

        /// <summary>
        /// Comprueba desde donde ha sido la colisión
        /// </summary>
        /// <param name="collision">Objeto que colisiona</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Player") || isDeath) return;

            ContactPoint contact = collision.contacts[0];
            
            bool hitFromAbove = contact.normal.y < -0.5f;

            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            
            bool isFalling = playerRb.linearVelocity.y < -0.1f; 

            if (hitFromAbove || (transform.position.y + 0.3f < collision.transform.position.y && isFalling))
            {
                HandleDeath(playerRb);
            }
            else
            {
                animator.SetTrigger("attack");
                targetRigidbody = playerRb;
            }
        }

        /// <summary>
        /// Aplica el empuje al jugador cuando se ejecuta la animación de ataque.
        /// Se llama desde el Animation Event.
        /// </summary>
        public void Attack()
        {
            if (targetRigidbody == null) return;

            VibrationManager.Instance.Vibrate();

            AudioManager.Instance.PlaySFX(attackClip);

            Vector3 pushDirection = transform.forward + Vector3.up * 0.3f;
            targetRigidbody.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
        }

        /// <summary>
        /// Maneja la muerte del enemigo cuando el jugador lo derrota desde arriba.
        /// </summary>
        /// <param name="playerRb">Rigidbody del jugador para aplicar rebote</param>
        private void HandleDeath(Rigidbody playerRb)
        {
            // Rebote del jugador
            playerRb.AddForce(Vector3.up * 100f, ForceMode.Impulse);

            isDeath = true;

            #if UNITY_ANDROID
            AchievementManager.Instance.OnEnemyKilledAchievements(enemyType);
            #endif

            if (agent != null)
            {
                agent.isStopped = true;
                agent.updateRotation = false;
            }

            animator.SetTrigger("die");
            AudioManager.Instance.PlaySFX(deathClip);

            GetComponent<Collider>().enabled = false;

            Destroy(gameObject, 1f);
        }

        /// <summary>
        /// Dibuja en el editor el radio de detección del enemigo.
        /// Solo visible cuando el objeto está seleccionado.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
