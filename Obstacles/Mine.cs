using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private AudioClip explosionAudio;
    [SerializeField] private float explosionForce = 500f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;



            rb.AddForce(randomDirection*explosionForce, ForceMode.Impulse);
        }
        AudioManager.Instance.PlaySFX(explosionAudio);
        Destroy(gameObject);
    }
}
