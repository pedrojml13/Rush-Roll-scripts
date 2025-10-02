using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    public class LevelSelectorManager : MonoBehaviour
    {
        public static LevelSelectorManager Instance { get; private set; }

        [Header("Escenarios")]
        [SerializeField] private GameObject sceneryBeach;
        [SerializeField] private GameObject sceneryForest;
        [SerializeField] private GameObject sceneryDesert;

        [Header("Botones de nivel")]
        [SerializeField] private GameObject levelButtonPrefab;
        [SerializeField] private Transform beachPanel;
        [SerializeField] private Transform forestPanel;
        [SerializeField] private Transform desertPanel;

        [Header("Música por escenario")]
        [SerializeField] private AudioClip beachMusic;
        [SerializeField] private AudioClip forestMusic;
        [SerializeField] private AudioClip desertMusic;

        private int sceneryNumber = 0;

        void Awake()
        {
            // Singleton: asegura una única instancia
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            // Escenario inicial
            sceneryBeach.SetActive(true);
            sceneryForest.SetActive(false);
            sceneryDesert.SetActive(false);

            GenerateLevelButtons();
            UpdateMusic();
        }

        private void GenerateLevelButtons()
        {
            List<LevelData> levels = GameManager.Instance.GetLevels();

            // Activamos todos los paneles temporalmente para instanciar botones
            sceneryBeach.SetActive(true);
            sceneryForest.SetActive(true);
            sceneryDesert.SetActive(true);

            for (int i = 0; i < levels.Count; i++)
            {
                Transform parentPanel = GetPanelForLevel(i);
                GameObject btnObj = Instantiate(levelButtonPrefab, parentPanel);
                LevelButton btn = btnObj.GetComponent<LevelButton>();

                LevelData level = levels[i];
                btn.Setup(level.levelIndex, level.unlocked, level.stars, level.bestTime);
            }

            // Restauramos el estado inicial
            sceneryBeach.SetActive(true);
            sceneryForest.SetActive(false);
            sceneryDesert.SetActive(false);
        }

        private Transform GetPanelForLevel(int levelIndex)
        {
            if (levelIndex < 9) return beachPanel;
            if (levelIndex < 18) return forestPanel;
            return desertPanel;
        }

        private void UpdateMusic()
        {
            // Cambia la música según el escenario activo
            if (sceneryNumber == 0)
            {
                AudioManager.Instance.PlayMusic(beachMusic);
            }
            else if (sceneryNumber == 1)
            {
                AudioManager.Instance.PlayMusic(forestMusic);

            }
            else if (sceneryNumber == 2)
            {
                AudioManager.Instance.PlayMusic(desertMusic);
            }
        }

        public void LoadLevel(int levelIndex)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(levelIndex + 2); // +2 porque el índice 0 es el login y el 1 es el menú
        }

        public void OnMenuButton()
        {
            SceneManager.LoadScene("Menu");
        }

        private void AnimateScenery(Transform panel)
        {
            // Asegura que tenga CanvasGroup para controlar la opacidad
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();

            // Estado inicial
            panel.localScale = Vector3.zero;
            canvasGroup.alpha = 0f;

            panel.gameObject.SetActive(true);

            // Animación de entrada
            LeanTween.scale(panel.gameObject, Vector3.one, 0.5f).setEaseOutBack();
            LeanTween.alphaCanvas(canvasGroup, 1f, 0.5f);
        }


        public void ChangeScenery(int sceneryIndex)
        {
            // Oculta todos los escenarios raíz
            sceneryBeach.SetActive(false);
            sceneryForest.SetActive(false);
            sceneryDesert.SetActive(false);

            // Oculta todos los paneles visuales
            beachPanel.gameObject.SetActive(false);
            forestPanel.gameObject.SetActive(false);
            desertPanel.gameObject.SetActive(false);

            GameObject targetScenery = sceneryBeach;
            Transform targetPanel = beachPanel;

            switch (sceneryIndex)
            {
                case 0:
                    targetScenery = sceneryBeach;
                    targetPanel = beachPanel;
                    break;
                case 1:
                    targetScenery = sceneryForest;
                    targetPanel = forestPanel;
                    break;
                case 2:
                    targetScenery = sceneryDesert;
                    targetPanel = desertPanel;
                    break;
                default:
                    targetScenery = sceneryBeach;
                    targetPanel = beachPanel;
                    break;
            }

            targetScenery.SetActive(true);
            AnimateScenery(targetPanel);

            sceneryNumber = sceneryIndex;
            UpdateMusic();
        }
    }
}