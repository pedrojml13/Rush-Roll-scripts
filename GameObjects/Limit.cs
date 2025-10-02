using UnityEngine;

namespace PJML.RushAndRoll
{
    public class Limit : MonoBehaviour
    {
        // Detecta si el jugador cruza el límite del nivel
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            LevelManager.Instance.GameOver();       // Lanza el evento de fin de partida
            Destroy(other.gameObject);              // Elimina al jugador de la escena
        }
    }
}