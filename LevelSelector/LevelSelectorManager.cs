using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestor del selector de niveles: activa mundos, instancia botones y reproduce música.
    /// </summary>
    public class LevelSelectorManager : MonoBehaviour
    {
        public static LevelSelectorManager Instance { get; private set; }

        [Header("Paneles de mundo")]
        [SerializeField] private Transform[] worldPanels; 
        [SerializeField] private GameObject howToPlayPanel;

        [Header("Botones y colores de los botones")]
        [SerializeField] private Image backButtonImage;
        [SerializeField] private Image nextButtonImage;
        [SerializeField] private Image menuButtonImage;
        [SerializeField] private Image howToPlayButtonImage;
        [SerializeField] private Image howToPlayPanelImage;

        [SerializeField] private Color [] buttonsColors;
        [Header("Prefabs de botones por mundo")]
        [SerializeField] private GameObject[] levelButtonPrefabs; 

        [Header("Música por mundo")]
        [SerializeField] private AudioClip[] musicClips; 

        [Header("Sonidos")]
        [SerializeField] private AudioClip buttonClickSound;

        [Header("Trofeos")]
        [SerializeField] private GameObject[] trophyPrefabs;

        private int currentWorld = 0;
        private Vector2 howToPlayBasePos;


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
        /// Muestra el primer mundo e inicializa los botones.
        /// </summary>
        void Start()
        {
            howToPlayBasePos = howToPlayPanel.GetComponent<RectTransform>().anchoredPosition;

            LoadCurrentPanelIndex();
            GenerateLevelButtons();
        }

        /// <summary>
        /// Instancia los botones de los niveles dentro de sus respectivos paneles.
        /// </summary>
        private void GenerateLevelButtons()
        {
            List<LevelData> levels = GameManager.Instance.GetLevels();

            for (int i = 0; i < levels.Count; i++)
            {
                int worldIndex = Mathf.Clamp(i / 9, 0, worldPanels.Length - 1);
                GameObject btnObj = Instantiate(levelButtonPrefabs[worldIndex], worldPanels[worldIndex]);
                
                LevelData level = levels[i];
                LevelButton btn = btnObj.GetComponent<LevelButton>();
                btn.Setup(level.levelIndex, level.unlocked, level.stars, level.bestTime);
            }
        }

        /// <summary>
        /// Muestra el panel del mundo seleccionado, desactivando los demás.
        /// </summary>
        /// <param name="worldIndex">Indice del mundo</param>
        public void ShowWorld(int worldIndex)
        {
            trophyPrefabs[currentWorld].SetActive(false);
            currentWorld = Mathf.Clamp(worldIndex, 0, worldPanels.Length - 1);

            for (int i = 0; i < worldPanels.Length; i++)
                worldPanels[i].gameObject.SetActive(i == currentWorld);

            backButtonImage.color = buttonsColors[(worldIndex - 1 + buttonsColors.Length) % buttonsColors.Length];
            nextButtonImage.color = buttonsColors[(worldIndex+1) % buttonsColors.Length];
            menuButtonImage.color = buttonsColors[worldIndex];
            howToPlayButtonImage.color = buttonsColors[worldIndex];
            howToPlayPanelImage.color = buttonsColors[worldIndex];

            AnimatePanel(worldPanels[currentWorld]);
            AudioManager.Instance.PlayMusic(musicClips[currentWorld]);
            if (GameManager.Instance.GetLevels()[currentWorld * 9 + 4].trophyCollected)
            trophyPrefabs[currentWorld].SetActive(true);
            
            
                
        }

        /// <summary>
        /// Anima un panel usando LeanTween.
        /// </summary>
        /// <param name="panel">Panel a animar</param>
        private void AnimatePanel(Transform panel)
        {
            CanvasGroup cg = panel.GetComponent<CanvasGroup>();
            if (cg == null) cg = panel.gameObject.AddComponent<CanvasGroup>();

            panel.localScale = Vector3.zero;
            cg.alpha = 0f;
            panel.gameObject.SetActive(true);

            LeanTween.scale(panel.gameObject, Vector3.one, 0.5f).setEaseOutBack();
            LeanTween.alphaCanvas(cg, 1f, 0.5f);
        }

        /// <summary>
        /// Carga la escena correspondiente al nivel.
        /// </summary>
        public void LoadLevel(int levelIndex)
        {
            SaveCurrentPanelIndex();
            Time.timeScale = 1f;
            SceneManager.LoadScene(levelIndex + 2);
        }

        /// <summary>
        /// Guarda el índice del panel actual en PlayerPrefs.
        /// </summary>
        private void SaveCurrentPanelIndex()
        {
            PlayerPrefs.SetInt("LastWorldIndex", currentWorld);
        }

        /// <summary>
        /// Carga el índice del panel actual desde PlayerPrefs.
        /// </summary>
        private void LoadCurrentPanelIndex()
        {
            int savedIndex = PlayerPrefs.GetInt("LastWorldIndex", 0);
            ShowWorld(savedIndex);
        }

        /// <summary>
        /// Guarda el índice del panel actual al salir de la aplicación.
        /// </summary>
        private void OnApplicationQuit()
        {
            SaveCurrentPanelIndex();
        }

        /// <summary>
        /// Muestra el panel de como jugar
        /// </summary>
        public void OnHowToPlayButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();
            howToPlayPanel.SetActive(true);

            RectTransform rt = howToPlayPanel.GetComponent<RectTransform>();
            CanvasGroup cg = howToPlayPanel.GetComponent<CanvasGroup>();

            if (cg != null)
                cg.alpha = 0f;

            rt.anchoredPosition = howToPlayBasePos + Vector2.down * 800f;

            LeanTween.move(rt, howToPlayBasePos, 0.6f)
                .setEaseOutCubic()
                .setIgnoreTimeScale(true);
        }

        /// <summary>
        /// Oculta el panel de como jugar
        /// </summary>
        public void OnCloseHowToPlayButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();

            RectTransform rt = howToPlayPanel.GetComponent<RectTransform>();

            LeanTween.move(rt, howToPlayBasePos + Vector2.down * 800f, 0.6f)
                .setEaseInCubic()
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    howToPlayPanel.SetActive(false);
                    rt.anchoredPosition = howToPlayBasePos;
                });
        }

        /// <summary>
        /// Regresa al menú principal.
        /// </summary>
        public void OnMenuButton()
        {
            SaveCurrentPanelIndex();

            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();
            SceneManager.LoadScene("Menu");
        }

        /// <summary>
        /// Cambia al siguiente mundo
        /// </summary>
        public void NextWorld()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            int next = (currentWorld + 1) % worldPanels.Length;
            ShowWorld(next);
        }

        /// <summary>
        /// Cambia al mundo anterior
        /// </summary>
        public void PreviousWorld()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);

            int prev = (currentWorld - 1 + worldPanels.Length) % worldPanels.Length;
            ShowWorld(prev);
        }
    }
}
