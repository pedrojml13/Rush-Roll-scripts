using UnityEngine;

namespace PJML.RushAndRoll
{
    public class AnimalBehavior : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public Transform[] landingPoints;
        private Animator animator;
        private int currentLandingIndex = 0;
        private bool isMoving = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!isMoving || landingPoints.Length == 0) return;

            Transform target = landingPoints[currentLandingIndex];

            // Mantiene la altura actual del animal
            Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);

            // Movimiento hacia el punto objetivo
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // Rotación hacia el punto objetivo
            Vector3 lookDirection = (target.position - transform.position);
            lookDirection.y = 0; // evita inclinación vertical
            if (lookDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDirection.normalized);

            // Si ha llegado al destino
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                isMoving = false;
                animator.SetBool("isPlayerNear", false);

                // Selecciona un nuevo punto aleatorio (evita quedarse en el mismo)
                int offset = Random.Range(1, landingPoints.Length);
                currentLandingIndex = (currentLandingIndex + offset) % landingPoints.Length;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // Activa movimiento si el jugador está cerca
            if (other.CompareTag("Player") && !isMoving)
            {
                isMoving = true;
                animator.SetBool("isPlayerNear", true);
            }
        }
    }
}