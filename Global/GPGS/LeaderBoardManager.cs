using UnityEngine;
using GooglePlayGames;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Manager encargado de subir y mostrar las puntuaciones en las tablas de clasificación de Google Play.
    /// Mapea cada índice de nivel de Unity con su ID correspondiente en la Google Play Console.
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del LeaderboardManager.
        /// </summary>
        public static LeaderboardManager Instance { get; private set; }

        private string[] leaderboardIds = new string[]
        {
            // BEACH
            "CgkI-8Oml4oOEAIQWQ", // Beach 1
            "CgkI-8Oml4oOEAIQWg", // Beach 2
            "CgkI-8Oml4oOEAIQWw", // Beach 3
            "CgkI-8Oml4oOEAIQXA", // Beach 4
            "CgkI-8Oml4oOEAIQXQ", // Beach 5
            "CgkI-8Oml4oOEAIQXg", // Beach 6
            "CgkI-8Oml4oOEAIQXw", // Beach 7
            "CgkI-8Oml4oOEAIQMw", // Beach 8
            "CgkI-8Oml4oOEAIQMg", // Beach 9

            // FOREST
            "CgkI-8Oml4oOEAIQPQ", // Forest 1
            "CgkI-8Oml4oOEAIQPg", // Forest 2
            "CgkI-8Oml4oOEAIQPw", // Forest 3
            "CgkI-8Oml4oOEAIQQA", // Forest 4
            "CgkI-8Oml4oOEAIQQQ", // Forest 5
            "CgkI-8Oml4oOEAIQQg", // Forest 6
            "CgkI-8Oml4oOEAIQQw", // Forest 7
            "CgkI-8Oml4oOEAIQRA", // Forest 8
            "CgkI-8Oml4oOEAIQRQ", // Forest 9

            // DESERT
            "CgkI-8Oml4oOEAIQNA", // Desert 1
            "CgkI-8Oml4oOEAIQNQ", // Desert 2
            "CgkI-8Oml4oOEAIQNg", // Desert 3
            "CgkI-8Oml4oOEAIQNw", // Desert 4
            "CgkI-8Oml4oOEAIQOA", // Desert 5
            "CgkI-8Oml4oOEAIQOQ", // Desert 6
            "CgkI-8Oml4oOEAIQOg", // Desert 7
            "CgkI-8Oml4oOEAIQOw", // Desert 8
            "CgkI-8Oml4oOEAIQPA", // Desert 9

            // UNDERWATER
            "CgkI-8Oml4oOEAIQRg", // Underwater 1
            "CgkI-8Oml4oOEAIQRw", // Underwater 2
            "CgkI-8Oml4oOEAIQSA", // Underwater 3
            "CgkI-8Oml4oOEAIQSQ", // Underwater 4
            "CgkI-8Oml4oOEAIQSg", // Underwater 5
            "CgkI-8Oml4oOEAIQSw", // Underwater 6
            "CgkI-8Oml4oOEAIQTA", // Underwater 7
            "CgkI-8Oml4oOEAIQTQ", // Underwater 8
            "CgkI-8Oml4oOEAIQTg", // Underwater 9

            // ICE
            "CgkI-8Oml4oOEAIQTw", // Ice 1
            "CgkI-8Oml4oOEAIQUA", // Ice 2
            "CgkI-8Oml4oOEAIQUQ", // Ice 3
            "CgkI-8Oml4oOEAIQUg", // Ice 4
            "CgkI-8Oml4oOEAIQUw", // Ice 5
            "CgkI-8Oml4oOEAIQVA", // Ice 6
            "CgkI-8Oml4oOEAIQVQ", // Ice 7
            "CgkI-8Oml4oOEAIQVg", // Ice 8
            "CgkI-8Oml4oOEAIQVw"  // Ice 9
        };
        private const long minScore = 1;

        /// <summary>
        /// Singleton, asegura que solo exista una instancia
        /// y evita su destrucción al cambiar de escena.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Envía la puntuación obtenida al servidor de Google Play.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel completado.</param>
        /// <param name="score">Puntuación.</param>
        public void ReportScore(int levelIndex, long score)
        {
            if (GameManager.Instance.HasInternet())
            {
                if (score < 0)
                {
                    Debug.LogError("La puntuación no puede ser negativa.");
                    return;
                }

                if (score < minScore)
                {
                    Debug.LogWarning($"La puntuación {score} es demasiado baja. Ignorando.");
                    return;
                }

                if (GPGSManager.Instance.IsAuthenticated)
                {
                    PlayGamesPlatform.Instance.ReportScore(score, leaderboardIds[levelIndex], (bool success) =>
                    {
                        if (success) Debug.Log("Puntuación enviada correctamente.");
                        else Debug.LogError("Error al enviar la puntuación.");
                    });
                }
                else
                {
                    Debug.LogWarning("Usuario no autenticado. No se puede enviar la puntuación.");
                }
            }
            
        }

        /// <summary>
        /// Abre la interfaz nativa de Google Play que muestra todos los ránkings del juego.
        /// </summary>
        public void ShowLeaderboardUI()
        {
            if (GameManager.Instance.HasInternet())
            {
                if (GPGSManager.Instance.IsAuthenticated)
                {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI();
                }
                else
                {
                    Debug.LogWarning("Usuario no autenticado. No se puede mostrar el ranking.");
                }
            }
        }
    }
}