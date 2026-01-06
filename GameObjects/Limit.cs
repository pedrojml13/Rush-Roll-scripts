using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Representa el límite del nivel. 
    /// Si el jugador cruza este límite, se dispara el evento de Game Over y se elimina al jugador.
    /// </summary>
    public class Limit : MonoBehaviour
    {

        [SerializeField] private AudioClip gameOver;

        /// <summary>
        /// Detecta cuando el jugador entra en el límite del nivel y finaliza la partida.
        /// </summary>
        /// <param name="other">Objeto que colisiona.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            AudioManager.Instance.PlaySFX(gameOver);

            VibrationManager.Instance.Vibrate();
            
            LevelManager.Instance.GameOver(); // Lanza el evento de fin de partida
            Destroy(other.gameObject);        // Elimina al jugador de la escena
        }
    }
}
