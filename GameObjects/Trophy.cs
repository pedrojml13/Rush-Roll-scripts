using UnityEngine;

public class Trophy : MonoBehaviour
{
    //[SerializeField] private AudioClip trophyClip;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //AudioManager.Instance.PlaySFX(trophyClip);
            Destroy(gameObject);
            //LevelManager.Instance.AddCoin();
        }
    }
}
