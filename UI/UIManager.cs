using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestor de la interfaz de usuario del juego.
    /// Controla paneles, textos de monedas, tiempo, recalibración y navegación entre niveles.
    /// Singleton accesible desde cualquier parte del proyecto.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del AudioManager.
        /// </summary>
        public static UIManager Instance{ get; private set; }

        [Header("Panels")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject UIPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private GameObject endGamePanel;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI coinCountText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI coinsCollectedText;
        [SerializeField] private TextMeshProUGUI finalTimeText;
        [SerializeField] private TextMeshProUGUI triesText;
        [SerializeField] private TextMeshProUGUI VictoryText;
        [SerializeField] private TextMeshProUGUI GameOverText;
        [SerializeField] private TextMeshProUGUI PauseText;
        [SerializeField] private TextMeshProUGUI levelNumberText;

        [SerializeField] private GameObject recalibrateText;

        [Header("Icons")]
         [SerializeField] private Image[] starsIcon;
        [SerializeField] private Image uICoinIcon;
        [SerializeField] private Image panelCoinIcon;
        [SerializeField] private Image uItimeIcon;
        [SerializeField] private Image panelTimeIcon;

        [Header("Sounds")]
        [SerializeField] private AudioClip buttonClickSound;

        [SerializeField] private GameObject doubleCoinsButton;
        [SerializeField] private GameObject star1, star2, star3;

        private bool iconTimeSpinning = false;

        /// <summary>
        /// Singleton, asegura que solo exista una instancia
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
        /// Si tiene internet, activa el botón de monedas dobles.
        /// </summary>
        void Start()
        {
            if (GameManager.Instance.IsOffline())
            {
                doubleCoinsButton.SetActive(false);
            }
        }

        /// <summary>
        /// Actualiza el texto de intentos del nivel.
        /// </summary>
        /// <param name="tries">Número de intentos.</param>
        public void UpdateTriesText(int tries)
        {
            triesText.text = "Try: " + tries;
        }

        /// <summary>
        /// Actualiza el texto del número de nivel.
        /// </summary>
        /// <param name="number">Número de nivel.</param>
        public void UpdateLevelNumberText(int number)
        {
            levelNumberText.text = "" + ((number % 9) + 1);
        }

        /// <summary>
        /// Muestra el panel de victoria y lo anima con LeanTween.
        /// </summary>
        /// <param name="starCount">Número de estrellas.</param>
        public void ShowVictoryPanel(int starCount)
        {
            // Activar panel y ocultar UI principal
            victoryPanel.SetActive(true);
            UIPanel.SetActive(false);

            // Actualizar texto de monedas y tiempo
            coinsCollectedText.text = coinCountText.text;
            finalTimeText.text = timeText.text;

            // Escalas iniciales a 0 para animación
            RectTransform panelRect = victoryPanel.GetComponent<RectTransform>();
            panelRect.localScale = Vector3.zero;
            star1.transform.localScale = Vector3.zero;
            star2.transform.localScale = Vector3.zero;
            star3.transform.localScale = Vector3.zero;
            coinsCollectedText.rectTransform.localScale = Vector3.zero;

            // VictoryText animación desde arriba
            RectTransform victoryTextRect = VictoryText.rectTransform;
            Vector3 originalPos = victoryTextRect.localPosition;
            victoryTextRect.localPosition += new Vector3(0, 200f, 0); // Mover hacia arriba 200 unidades
            victoryTextRect.localScale = Vector3.zero;

            LeanTween.moveLocalY(victoryTextRect.gameObject, originalPos.y, 0.6f)
                .setEaseOutBounce()
                .setDelay(0.2f)
                .setIgnoreTimeScale(true);

            LeanTween.scale(victoryTextRect, Vector3.one, 0.6f)
                .setEaseOutBack()
                .setDelay(0.2f)
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    // Pulso continuo del texto
                    LeanTween.scale(victoryTextRect, Vector3.one * 1.05f, 0.5f)
                        .setEaseInOutSine()
                        .setLoopPingPong()
                        .setIgnoreTimeScale(true);
                });

            // Animar panel de victoria
            LeanTween.scale(panelRect, Vector3.one, 0.5f)
                .setEaseOutBack()
                .setIgnoreTimeScale(true);

            // Animar estrellas según cantidad obtenida
            if (starCount >= 1)
            {
                star1.SetActive(true);
                LeanTween.scale(star1, Vector3.one, 0.4f)
                    .setEaseOutBack()
                    .setDelay(0.2f)
                    .setIgnoreTimeScale(true);
            }

            if (starCount >= 2)
            {
                star2.SetActive(true);
                LeanTween.scale(star2, Vector3.one, 0.4f)
                    .setEaseOutBack()
                    .setDelay(0.4f)
                    .setIgnoreTimeScale(true);
            }

            if (starCount >= 3)
            {
                star3.SetActive(true);
                LeanTween.scale(star3, Vector3.one, 0.4f)
                    .setEaseOutBack()
                    .setDelay(0.6f)
                    .setIgnoreTimeScale(true);
            }

            RectTransform rt = uICoinIcon.rectTransform;

            LeanTween.cancel(rt.gameObject);

            LeanTween.scaleX(rt.gameObject, -1f, 2f)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong()
                .setIgnoreTimeScale(true);

            // Animar texto de monedas finales
            LeanTween.scale(coinsCollectedText.rectTransform, Vector3.one, 0.5f)
                .setEaseOutBack()
                .setDelay(0.8f)
                .setIgnoreTimeScale(true);

            levelNumberText.gameObject.SetActive(true);

            // Fade
            CanvasGroup cg = victoryPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                    .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Muestra el panel de Game Over y lo anima con LeanTween.
        /// </summary>
        public void ShowGameOverPanel()
        {
            gameOverPanel.SetActive(true);
            UIPanel.SetActive(false);


            RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
            panelRect.localScale = Vector3.zero;
            GameOverText.rectTransform.localScale = Vector3.zero;

            // Animar panel
            LeanTween.scale(panelRect, Vector3.one, 0.5f)
                .setEaseOutBack()
                .setIgnoreTimeScale(true);

            // Animar texto "Game Over"
            GameOverText.rectTransform.localScale = Vector3.one; // asegúrate que esté en 1
            LeanTween.scale(GameOverText.rectTransform, Vector3.one * 1.2f, 0.3f)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setLoopPingPong(-1)
                    .setIgnoreTimeScale(true);

            levelNumberText.gameObject.SetActive(true);

            // Fade
            CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                    .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Muestra la UI principal animándola con LeanTween y oculta los paneles de estado.
        /// </summary>
        public void ShowUIPanel()
        {
            UIPanel.SetActive(true);

            CanvasGroup cg = UIPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;

                LeanTween.alphaCanvas(cg, 1f, 0.3f)
                    .setEaseInOutSine()
                    .setIgnoreTimeScale(true);
            }

            UIPanel.transform.localScale = Vector3.zero;

            LeanTween.scale(UIPanel, Vector3.one, 0.3f)
                .setEaseOutBack()
                .setIgnoreTimeScale(true);


            victoryPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            levelNumberText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Oculta todos los paneles de la UI.
        /// </summary>
        public void HideAllPanels()
        {
            CanvasGroup cg = UIPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 0f, 0.3f)
                    .setEaseInOutSine()
                    .setIgnoreTimeScale(true);
            }

            LeanTween.scale(UIPanel, Vector3.zero, 0.3f)
                .setEaseInBack()
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    UIPanel.SetActive(false);
                    cg.alpha = 1f;
                });

            
            victoryPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            pausePanel.SetActive(false);
            levelNumberText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Muestra el panel de tutorial y lo anima con LeanTween.
        /// </summary>
        public void ShowTutorialPanel()
        {
            tutorialPanel.SetActive(true);

            RectTransform rt = tutorialPanel.GetComponent<RectTransform>();

            // Cancelar animaciones previas
            LeanTween.cancel(rt);

            // Reset seguro
            rt.localScale = Vector3.zero;
            rt.localRotation = Quaternion.identity;

            // Escala de entrada
            LeanTween.scale(rt, Vector3.one, 0.6f)
                .setEaseOutExpo()
                .setIgnoreTimeScale(true);

            // Pequeño shake
            LeanTween.rotateZ(rt.gameObject, -5f, 0.08f)
                .setLoopPingPong(4)
                .setIgnoreTimeScale(true);

            // Fade
            CanvasGroup cg = tutorialPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                    .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Muestra el panel de fin de juego y lo anima con LeanTween.
        /// </summary>
        public void ShowEndGamePanel()
        {
            endGamePanel.SetActive(true);

            RectTransform rt = endGamePanel.GetComponent<RectTransform>();

            // Cancelar animaciones previas
            LeanTween.cancel(rt);

            // Reset seguro
            rt.localScale = Vector3.zero;
            rt.localRotation = Quaternion.identity;

            // Escala de entrada
            LeanTween.scale(rt, Vector3.one, 0.6f)
                .setEaseOutExpo()
                .setIgnoreTimeScale(true);

            // Pequeño shake
            LeanTween.rotateZ(rt.gameObject, -5f, 0.08f)
                .setLoopPingPong(4)
                .setIgnoreTimeScale(true);

            // Fade
            CanvasGroup cg = endGamePanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                    .setIgnoreTimeScale(true);
            }
        }
        /// <summary>
        /// Pausa el juego y muestra el panel de pausa animado con LeanTween.
        /// </summary>
        public void OnPauseButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);

            LevelManager.Instance.PauseTime();

            HideAllPanels();
            pausePanel.SetActive(true);
            

            RectTransform panelRect = pausePanel.GetComponent<RectTransform>();
            panelRect.localScale = Vector3.zero;
            PauseText.rectTransform.localScale = Vector3.zero;

            // Animar panel
            LeanTween.scale(panelRect, Vector3.one, 0.4f)
                .setEaseOutBack()
                .setIgnoreTimeScale(true);

            // Animar texto "Pause"
            PauseText.rectTransform.localScale = Vector3.one;
            LeanTween.scale(PauseText.rectTransform, Vector3.one * 1.2f, 0.3f)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setLoopPingPong(2)
                    .setIgnoreTimeScale(true);

            levelNumberText.gameObject.SetActive(true);

            // Fade
            CanvasGroup cg = pausePanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                    .setIgnoreTimeScale(true);
            }

        }

        /// <summary>
        /// Desactiva el panel del tutorial con una animación de LeanTween.
        /// </summary>
        public void OnCloseTutorialPanel()
        {
            VibrationManager.Instance.Vibrate();
            AudioManager.Instance.PlaySFX(buttonClickSound);

            RectTransform rt = tutorialPanel.GetComponent<RectTransform>();

            // Cancelar animaciones previas
            LeanTween.cancel(rt);

            // Rotación neutra antes de salir
            rt.localRotation = Quaternion.identity;

            // Pequeño shake inverso
            LeanTween.rotateZ(rt.gameObject, 5f, 0.06f)
                .setLoopPingPong(3)
                .setIgnoreTimeScale(true);

            // Escala de salida
            LeanTween.scale(rt, Vector3.zero, 0.45f)
                .setEaseInExpo()
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    tutorialPanel.SetActive(false);
                });

            // Fade out
            CanvasGroup cg = tutorialPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 0f, 0.35f)
                    .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Reanuda el juego y oculta el panel de pausa animándolo con LeanTween.
        /// </summary>
        public void OnContinueButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);

            CanvasGroup cg = pausePanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 0f, 0.3f)
                    .setEaseInOutSine()
                    .setIgnoreTimeScale(true);
            }

            LeanTween.scale(pausePanel, Vector3.zero, 0.3f)
                .setEaseInBack()
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    pausePanel.SetActive(false);
                    cg.alpha = 1f;
                });

            ShowUIPanel();

            levelNumberText.gameObject.SetActive(false);
            
            LevelManager.Instance.ResumeTime();
        }

        /// <summary>
        /// Recalibra el control del jugador y muestra un texto temporal.
        /// </summary>
        public void OnRecalibrateButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            if (BallController.Instance == null) return;

            StartCoroutine(BallController.Instance.DelayedCalibration());
            recalibrateText.SetActive(true);
            StartCoroutine(HideText(1f));
        }

        /// <summary>
        /// Oculta el texto de recalibración después de un retraso.
        /// </summary>
        /// <param name="delay">Tiempo de espera antes de ocultar el texto.</param>
        /// <returns></returns>
        private IEnumerator HideText(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            recalibrateText.SetActive(false);
        }

        /// <summary>
        /// Reproduce el sonido del botón, carga el siguiente nivel y oculta los paneles.
        /// </summary>
        public void OnNextLevelButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextLevel >= SceneManager.sceneCountInBuildSettings)
                nextLevel = 1;

            HideAllPanels();
            SceneManager.LoadScene(nextLevel);
        }

        /// <summary>
        /// Reproduce el sonido del botón, oculta los paneles y reinicia el nivel actual.
        /// </summary>
        public void OnReplayButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            HideAllPanels();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Reproduce el sonido del botón, oculta los paneles y regresa al menú principal.
        /// </summary>
        public void OnMenuButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            HideAllPanels();
            LevelManager.Instance.GoToMenu();
        }

        /// <summary>
        /// Dobla las monedas recolectadas mediante un anuncio recompensado.
        /// </summary>
        public void OnDoubleCoinsButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            LevelPlayManager.Instance.ShowRewarded(() =>
            {
                int coinCount = int.Parse(coinsCollectedText.text);
                coinsCollectedText.text = "" + (coinCount * 2);
                GameManager.Instance.AddCoins(coinCount); // Añade las monedas extra al total
                doubleCoinsButton.SetActive(false);
                GameManager.Instance.ResetTriesSiceLastAd();
            });
        }

        /// <summary>
        /// Actualiza el contador de monedas en la UI y anima el icono mediante LeanTween.
        /// </summary>
        /// <param name="coins">Cantidad de monedas.</param>
        public void UpdateCoinText(int coins)
        {
            coinCountText.text = coins.ToString();

            // Vibración rápida del icono de la moneda
            if (uICoinIcon != null)
            {
                RectTransform rt = uICoinIcon.rectTransform;
                Vector3 originalPos = rt.localPosition;

                // Cancelamos animaciones anteriores para que no se acumulen
                LeanTween.cancel(rt);

                // Movimiento ping-pong vertical
                LeanTween.moveLocalY(rt.gameObject, originalPos.y + 10f, 0.15f)
                        .setEase(LeanTweenType.easeOutSine)
                        .setLoopPingPong(1);
            }
        }

        /// <summary>
        /// Actualiza el texto de tiempo en la UI con una animación de LeanTween.
        /// </summary>
        /// <param name="time">Tiempo formateado.</param>
        public void UpdateTimeText(string time)
        {
            timeText.text = time;

            // Giro rápido del icono de tiempo
            if (!iconTimeSpinning && uItimeIcon != null && time != "00:00:00")
            {
                RectTransform rt = uItimeIcon.rectTransform;

                // Giro
                LeanTween.rotateZ(rt.gameObject, rt.localEulerAngles.z + 180f, 1f)
                .setEase(LeanTweenType.easeOutSine)
                .setLoopClamp();;
                iconTimeSpinning = true;

            }
        }
    }
}
