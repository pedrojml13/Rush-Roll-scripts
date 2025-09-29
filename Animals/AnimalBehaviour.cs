using UnityEngine;
using Random = System.Random;

public class AnimalBehavior : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed = 5f;

    public Transform[] landingPoints;
    private int currentLandingIndex = 0;
    private bool isMoving = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isMoving && landingPoints.Length > 0)
        {
            Transform target = landingPoints[currentLandingIndex];
            Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            
            Vector3 lookDirection = (target.position - transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDirection);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                isMoving = false;
                animator.SetBool("isPlayerNear", false);

                Random random = new Random();
                int nextLandingIndex = random.Next(landingPoints.Length) + 1;

                currentLandingIndex = (currentLandingIndex + nextLandingIndex) % landingPoints.Length;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isMoving)
        {
            isMoving = true;
            animator.SetBool("isPlayerNear", true);
        }
    }
}
