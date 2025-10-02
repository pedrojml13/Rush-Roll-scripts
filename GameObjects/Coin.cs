using UnityEngine;

namespace PJML.RushAndRoll
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private AudioClip coinClip;

        // Detecta colisión con el jugador
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            // Reproduce sonido de recogida
            AudioManager.Instance.PlaySFX(coinClip);

            // Suma una moneda al contador del nivel
            LevelManager.Instance.AddCoin();

            // Elimina el objeto de la escena
            Destroy(gameObject);
        }

    }
}