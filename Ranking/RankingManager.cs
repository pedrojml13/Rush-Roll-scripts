using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la visualización del ranking global, organizando los niveles por biomas
    /// y manejando la transición visual entre los diferentes escenarios.
    /// </summary>
    public class RankingManager : MonoBehaviour
    {
        [Header("Escenarios (Modelos 3D/Visuales)")]
        [SerializeField] private GameObject sceneryBeach;
        [SerializeField] private GameObject sceneryForest;
        [SerializeField] private GameObject sceneryDesert;
        [SerializeField] private GameObject sceneryUnderwater;

        [Header("Contenedores de UI (Paneles)")]
        [SerializeField] private Transform beachPanel;
        [SerializeField] private Transform forestPanel;
        [SerializeField] private Transform desertPanel;
        [SerializeField] private Transform underWaterPanel;

        [Header("Configuración de Entradas")]
        [SerializeField] private GameObject rankingEntryPrefab;
        
        private int sceneryNumber = 0;

        /// <summary>
        /// Inicializa estado visual y genera las entradas del Ranking
        /// </summary>
        private void Start()
        {
            // 
            InitializeUI();

            List<LevelData> levels = GameManager.Instance.GetLevels();
            GameManager.Instance.GetRankingLevelData(rankings =>
            {
                GenerateRankingEntries(levels, rankings);
            });
        }

        /// <summary>
        /// Configura el estado inicial de los escenarios y paneles.
        /// </summary>
        private void InitializeUI()
        {
            sceneryBeach.SetActive(true);
            sceneryForest.SetActive(false);
            sceneryDesert.SetActive(false);
            sceneryUnderwater.SetActive(false);
        }

        /// <summary>
        /// Instancia y configura los elementos de la lista de ranking basándose en los datos obtenidos.
        /// </summary>
        /// <param name="levels">Lista de los niveles.</param>
        /// <param name="rankings">Diccionario con los nombres de jugadores y los mejores tiempos.</param>
        void GenerateRankingEntries(List<LevelData> levels, Dictionary<int, (string playerName, float bestTime)> rankings)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                Transform parentPanel = GetPanelForLevel(i);

                string playerName = rankings.ContainsKey(i) ? rankings[i].playerName : "—";
                float bestTime = rankings.ContainsKey(i) ? rankings[i].bestTime : 0f;

                GameObject entryObj = Instantiate(rankingEntryPrefab, parentPanel);
                RankingEntry entry = entryObj.GetComponent<RankingEntry>();
                
                entry.Setup(i + 1); 
            }
        }

        /// <summary>
        /// Extrae el índice de un nivel de un string.
        /// </summary>
        /// <param name="levelId">String a extraer el nivel</param>
        /// <returns></returns>
        int ExtractLevelIndex(string levelId)
        {
            string[] parts = levelId.Split('_');
            if (parts.Length == 2 && int.TryParse(parts[1], out int index))
                return index - 1;
            return 0;
        }

        /// <summary>
        /// Selecciona el panel dependiendo del índice del nivel.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel para seleccionar el panel</param>
        /// <returns></returns>
        Transform GetPanelForLevel(int levelIndex)
        {
            if (levelIndex < 9) return beachPanel;
            if (levelIndex < 18) return forestPanel;
            if (levelIndex < 27) return desertPanel;
            return underWaterPanel;
        }

        /// <summary>
        /// Cambia el escenario y el panel dependiendo del índice.
        /// <param name="sceneryIndex">Indice del escenario.</param>
        public void ChangeScenery(int sceneryIndex)
        {
            sceneryBeach.SetActive(false);
            sceneryForest.SetActive(false);
            sceneryDesert.SetActive(false);
            sceneryUnderwater.SetActive(false);

            beachPanel.gameObject.SetActive(false);
            forestPanel.gameObject.SetActive(false);
            desertPanel.gameObject.SetActive(false);
            underWaterPanel.gameObject.SetActive(false);

            GameObject targetScenery = sceneryBeach;
            Transform targetPanel = beachPanel;

            switch (sceneryIndex)
            {
                case 0: targetScenery = sceneryBeach; targetPanel = beachPanel; break;
                case 1: targetScenery = sceneryForest; targetPanel = forestPanel; break;
                case 2: targetScenery = sceneryDesert; targetPanel = desertPanel; break;
                case 3: targetScenery = sceneryUnderwater; targetPanel = underWaterPanel; break;
            }

            targetScenery.SetActive(true);
            AnimateScenery(targetPanel);
            sceneryNumber = sceneryIndex;
        }

        /// <summary>
        /// Realiza una animación de entrada para el panel seleccionado mediante LeanTween.
        /// </summary>
        /// <param name="panel">El panel a animar.</param>
        private void AnimateScenery(Transform panel)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();

            panel.localScale = Vector3.zero;
            canvasGroup.alpha = 0f;
            panel.gameObject.SetActive(true);

            LeanTween.scale(panel.gameObject, Vector3.one, 0.5f).setEaseOutBack();
            LeanTween.alphaCanvas(canvasGroup, 1f, 0.5f);
        }

        /// <summary>
        /// Carga la escena del menú principal.
        /// </summary>
        public void OnMenuButton()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}