using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Representa una moneda coleccionable.
    /// Al entrar en contacto con el jugador:
    /// - Reproduce un sonido
    /// - Incrementa el contador de monedas del nivel
    /// - Se destruye a sí misma
    /// </summary>
    public class Coin : MonoBehaviour
    {
        [Tooltip("Sonido que se reproduce al recoger la moneda")]
        [SerializeField] private AudioClip coinClip;

        /// <summary>
        /// Detecta la recogida de la moneda por parte del jugador.
        /// </summary>
        /// <param name="other">Objeto que colisiona.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            AudioManager.Instance.PlaySFX(coinClip);
            LevelManager.Instance.AddCoin();
            Destroy(gameObject);
        }
    }
}
