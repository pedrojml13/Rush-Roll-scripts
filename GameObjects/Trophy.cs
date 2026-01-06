using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Representa un trofeo en el nivel que el jugador puede recoger.
    /// Gestiona su destrucción si ya ha sido recogido previamente
    /// y notifica al LevelManager cuando se recoge.
    /// </summary>
    public class Trophy : MonoBehaviour
    {
        [SerializeField] private AudioClip trophyClip;
        [SerializeField] private int trophyNumber;

        /// <summary>
        /// Comprueba al iniciar si el trofeo ya fue recogido en niveles previos
        /// y lo destruye si corresponde.
        /// </summary>
        void Start()
        {
            if (trophyNumber == 0 && GameManager.Instance.GetLevelData(4).trophyCollected)
            {
                Destroy(gameObject);
            }
            else if (trophyNumber == 1 && GameManager.Instance.GetLevelData(13).trophyCollected)
            {
                Destroy(gameObject);
            }
            else if (trophyNumber == 2 && GameManager.Instance.GetLevelData(22).trophyCollected)
            {
                Destroy(gameObject);
            }
            else if (trophyNumber == 3 && GameManager.Instance.GetLevelData(31).trophyCollected)
            {
                Destroy(gameObject);
            }
            else if (trophyNumber == 4 && GameManager.Instance.GetLevelData(40).trophyCollected)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Detecta colisión con el jugador, destruye el trofeo y notifica al LevelManager.
        /// </summary>
        /// <param name="other">Objeto que colisiona.</param>
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                VibrationManager.Instance.VibrateMultiple(2, 0.2f);
                AudioManager.Instance.PlaySFX(trophyClip);
                Destroy(gameObject);
                LevelManager.Instance.CollectTrophy();
            }
        }
    }
}
