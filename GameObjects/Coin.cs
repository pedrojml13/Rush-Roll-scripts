using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip coinClip;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(coinClip);
            Destroy(gameObject);
            LevelManager.Instance.AddCoin();
        }
    }
}
