using UnityEngine;

public class Limit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.GameOver();
            Destroy(other.gameObject);
        }
    }
}
