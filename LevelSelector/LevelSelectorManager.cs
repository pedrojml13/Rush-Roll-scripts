using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la lógica del selector de niveles, incluyendo la instanciación de botones,
    /// el cambio de escenarios visuales y la actualización de la música de fondo.
    /// </summary>
    public class LevelSelectorManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del LevelManager.
        /// </summary>
        public static LevelSelectorManager Instance { get; private set; }

        [Header("Escenarios")]
        [Tooltip("Objetos raíz que contienen el arte de cada mundo.")]
        [SerializeField] private GameObject sceneryBeach;
        [SerializeField] private GameObject sceneryForest;
        [SerializeField] private GameObject sceneryDesert;
        [SerializeField] private GameObject sceneryUnderwater;
        [SerializeField] private GameObject sceneryIce;

        [Header("Botones de nivel")]
        [Tooltip("Prefabs de botones personalizados para cada bioma.")]
        [SerializeField] private GameObject beachLevelButtonPrefab;
        [SerializeField] private GameObject forestLevelButtonPrefab;
        [SerializeField] private GameObject desertLevelButtonPrefab;
        [SerializeField] private GameObject underwaterLevelButtonPrefab;
        [SerializeField] private GameObject iceLevelButtonPrefab;

        [Header("Paneles de Contenedor")]
        [Tooltip("Transform donde se agruparán los botones de cada mundo.")]
        [SerializeField] private Transform beachPanel;
        [SerializeField] private Transform forestPanel;
        [SerializeField] private Transform desertPanel;
        [SerializeField] private Transform underWaterPanel;
        [SerializeField] private Transform icePanel;

        [Header("Música por escenario")]
        [SerializeField] private AudioClip beachMusic;
        [SerializeField] private AudioClip forestMusic;
        [SerializeField] private AudioClip desertMusic;
        [SerializeField] private AudioClip underwaterMusic;
        [SerializeField] private AudioClip iceMusic;

        [Header("Sonidos")]
        [SerializeField] private AudioClip buttonClickSound;

        private int sceneryNumber = 0;

        /// <summary>
        /// Singleton, asegura que solo exista una instancia.
        /// </summary>
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// Activa el escenario de la playa, genera los botones de los niveles y actualiza la música.
        /// </summary>
        void Start()
        {
            sceneryBeach.SetActive(true);
            sceneryForest.SetActive(false);
            sceneryDesert.SetActive(false);
            sceneryUnderwater.SetActive(false);
            sceneryIce.SetActive(false);

            GenerateLevelButtons();
            UpdateMusic();
        }

        /// <summary>
        /// Recupera los datos de niveles del GameManager e instancia los botones
        /// en sus respectivos paneles según el índice del nivel.
        /// </summary>
        private void GenerateLevelButtons()
        {
            List<LevelData> levels = GameManager.Instance.GetLevels();

            sceneryBeach.SetActive(true);
            sceneryForest.SetActive(true);
            sceneryDesert.SetActive(true);
            sceneryUnderwater.SetActive(true);
            sceneryIce.SetActive(true);

            for (int i = 0; i < levels.Count; i++)
            {
                Transform parentPanel = GetPanelForLevel(i);
                GameObject btnObj;

                // Selección de Prefab por rango de niveles (9 niveles por mundo)
                if (i < 9)
                    btnObj = Instantiate(beachLevelButtonPrefab, parentPanel);
                else if (i < 18)
                    btnObj = Instantiate(forestLevelButtonPrefab, parentPanel);
                else if (i < 27)
                    btnObj = Instantiate(desertLevelButtonPrefab, parentPanel);
                else if (i < 36)
                    btnObj = Instantiate(underwaterLevelButtonPrefab, parentPanel);
                else
                    btnObj = Instantiate(iceLevelButtonPrefab, parentPanel);

                LevelButton btn = btnObj.GetComponent<LevelButton>();
                LevelData level = levels[i];
                
                // Configura el estado visual y datos del botón
                btn.Setup(level.levelIndex, level.unlocked, level.stars, level.bestTime);
            }

            sceneryBeach.SetActive(true);
            sceneryForest.SetActive(false);
            sceneryDesert.SetActive(false);
            sceneryUnderwater.SetActive(false);
            sceneryIce.SetActive(false);
        }

        /// <summary>
        /// Devuelve el panel dependiendo del índice.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel.</param>
        /// <returns>El panel correspondiente.</returns>
        private Transform GetPanelForLevel(int levelIndex)
        {
            if (levelIndex < 9) return beachPanel;
            if (levelIndex < 18) return forestPanel;
            if (levelIndex < 27) return desertPanel;
            if (levelIndex < 36) return underWaterPanel;
            return icePanel;
        }

        /// <summary>
        /// Actualiza la música según el panel actual.
        /// </summary>
        private void UpdateMusic()
        {
            switch (sceneryNumber)
            {
                case 0: AudioManager.Instance.PlayMusic(beachMusic); break;
                case 1: AudioManager.Instance.PlayMusic(forestMusic); break;
                case 2: AudioManager.Instance.PlayMusic(desertMusic); break;
                case 3: AudioManager.Instance.PlayMusic(underwaterMusic); break;
                case 4: AudioManager.Instance.PlayMusic(iceMusic); break;
            }
        }

        /// <summary>
        /// Carga la escena de juego correspondiente al nivel.
        /// </summary>
        /// <param name="levelIndex">Índice del nivel.</param>
        public void LoadLevel(int levelIndex)
        {
            Time.timeScale = 1f;
            // Escena 0 = Login, Escena 1 = Menú Principal
            SceneManager.LoadScene(levelIndex + 2);
        }

        /// <summary>
        /// Reproduce el sonido del botón y regresa al menú principal.
        /// </summary>
        public void OnMenuButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            SceneManager.LoadScene("Menu");
        }

        /// <summary>
        /// Anima el panel mediante LeanTween.
        /// </summary>
        /// <param name="panel">Panel a animar.</param>
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
        /// Cambia el panel dependiendo del índice.
        /// </summary>
        /// <param name="sceneryIndex">Indice del escenario.</param>
        public void ChangeScenery(int sceneryIndex)
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);

            sceneryBeach.SetActive(false);
            sceneryForest.SetActive(false);
            sceneryDesert.SetActive(false);
            sceneryUnderwater.SetActive(false);
            sceneryIce.SetActive(false);

            beachPanel.gameObject.SetActive(false);
            forestPanel.gameObject.SetActive(false);
            desertPanel.gameObject.SetActive(false);
            underWaterPanel.gameObject.SetActive(false);
            icePanel.gameObject.SetActive(false);

            GameObject targetScenery = sceneryBeach;
            Transform targetPanel = beachPanel;

            switch (sceneryIndex)
            {
                case 0: targetScenery = sceneryBeach; targetPanel = beachPanel; break;
                case 1: targetScenery = sceneryForest; targetPanel = forestPanel; break;
                case 2: targetScenery = sceneryDesert; targetPanel = desertPanel; break;
                case 3: targetScenery = sceneryUnderwater; targetPanel = underWaterPanel; break;
                case 4: targetScenery = sceneryIce; targetPanel = icePanel; break;
                default: targetScenery = sceneryBeach; targetPanel = beachPanel; break;
            }

            targetScenery.SetActive(true);
            AnimateScenery(targetPanel);

            sceneryNumber = sceneryIndex;
            UpdateMusic();
        }
    }
}