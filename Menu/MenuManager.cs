using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;


namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la interfaz del Menú Principal, la visualización de datos del perfil
    /// del jugador y la navegación entre las diferentes secciones del juego.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del MenuManager.
        /// </summary>
        public static MenuManager Instance;

        [Header("UI Perfil de Usuario")]
        [SerializeField] private GameObject playerNamePanel;
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_Text playerName, playerName1;
        [SerializeField] private TMP_Text coinNumber;
        [SerializeField] private RawImage playerAvatar, playerAvatar1;
        [SerializeField] private GameObject usernameTakenMsg;
        [SerializeField] private TMP_Text sessionPlayedTime, totalTries, totalCoinsCollected, totalCollectedStars;
        [SerializeField] private GameObject tryLogInButton;
        
        [Header("Botones")]
        [SerializeField] GameObject rMShopButton;
        [SerializeField] GameObject leaderBoardsButton;
        [SerializeField] GameObject achievementsButton;
        [SerializeField] GameObject supporterButton, supporterPanel, thanksPanel, supporterImage, gameEndedImage, supporterImage1, gameEndedImage1, extendedPlayerPanel;

        [Header("Elementos de Escena")]
        [SerializeField] private GameObject menuSfere;

        [Header("Sonidos")]
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip supportSound;


        /// <summary>
        /// Singleton, asegura que solo exista una instancia.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Inicialización asíncrona. Espera a que los datos de red estén listos antes de mostrar la UI.
        /// </summary>
        private IEnumerator Start()
        {

            AudioManager.Instance.PlayMusic(menuMusic);

            // Sincronización visual: Espera hasta que el GameManager tenga cargada la skin
            while (GameManager.Instance.GetSelectedBallSkin() == null)
            {
                yield return null; 
            }

            // Aplicar la textura de la skin seleccionada a la esfera del menú
            if (menuSfere != null)
            {
                menuSfere.GetComponent<Renderer>().material = GameManager.Instance.GetSelectedBallSkin().material;              
            }

            coinNumber.text = GameManager.Instance.GetCoins().ToString();
            
            if (!GameManager.Instance.IsOffline())
            {
                tryLogInButton.SetActive(false);
                
#if UNITY_ANDROID
                playerName.text = GPGSManager.Instance.GetUsername();
                playerName1.text = playerName.text;
                while (GPGSManager.Instance.GetUserImage() == null)
                {
                    yield return null;
                }
                playerAvatar.texture = GPGSManager.Instance.GetUserImage();
                playerAvatar1.texture = GPGSManager.Instance.GetUserImage();
                
                if(!GameManager.Instance.IsSupporter())
                {
                    // Muestra el banner de publicidad en el menú si no es Supporter
                    LevelPlayManager.Instance.ShowBanner();
                }
                else{
                    LevelPlayManager.Instance.HideBanner();
                    supporterImage.SetActive(true);
                    supporterImage1.SetActive(true);
                }
#else
                string username = GameManager.Instance.GetUsername();
                if (!string.IsNullOrEmpty(username))
                {
                    playerName.text = username;
                }
                else
                {
                    // Si no tiene nombre definido, abre el panel de creación de perfil
                    playerNamePanel.SetActive(true);
                }
#endif
            }
            // Si no hay internet 
            else
            {
                rMShopButton.SetActive(false);
                leaderBoardsButton.SetActive(false);
                achievementsButton.SetActive(false);
                supporterButton.SetActive(false);
                tryLogInButton.SetActive(true);

                string username = GameManager.Instance.GetUsername();
                if (!string.IsNullOrEmpty(username))
                {
                    playerName.text = username;
                }
                else
                {
                    // Si no tiene nombre definido, abre el panel de creación de perfil
                    //playerNamePanel.SetActive(true);
                }                
            }

            // Si el usuario ha finalizado el juego activamos las imagenes.
            if (GameManager.Instance.IsGameEnded())
            {
                gameEndedImage.SetActive(true);
                gameEndedImage1.SetActive(true);
            }
            
            // Reset de rachas y actualización de economía
            GameManager.Instance.ResetWinStreak();
        }

        /// <summary>
        /// Intenta registrar el nombre de usuario elegido.
        /// </summary>
        public void OnSaveNameButton()
        {
            string desiredName = usernameInput.text;

            if (string.IsNullOrEmpty(desiredName))
            {
                Debug.LogWarning("Intento de guardar nombre vacío.");
                return;
            }

            GameManager.Instance.TrySetUsername(desiredName, success =>
            {
                if (!success)
                {
                    usernameTakenMsg.SetActive(true);
                    return;
                }

                playerName.text = desiredName;
                playerNamePanel.SetActive(false);
            });
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y carga la escena LevelSelector.
        /// </summary>
        public void OnPlayButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();
            SceneManager.LoadScene("LevelSelector");
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y carga la escena Shop.
        /// </summary>
        public void OnShopButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();
            SceneManager.LoadScene("Shop");           
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y carga la escena RMShpo.
        /// </summary>
        public void OnRMShopButton(){
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();
            SceneManager.LoadScene("RMShop");
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y carga la escena Settings.
        /// </summary>
        public void OnSettingsButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();
            SceneManager.LoadScene("Settings");
        }

        /// <summary>
        /// Notifica al GameManager y va a la escena de login
        /// </summary>
        public void OnTryToLogInButton()
        {
            SceneManager.LoadScene("Login");
        }

        /// <summary>
        /// Reproduce el sonido de supporter, vibra y activa el panel de supporter animado con LeanTween.
        /// </summary>
        public void OnOpenSupporterPanelButton()
        {

            if (!GameManager.Instance.IsSupporter())
            {
                AudioManager.Instance.PlaySFX(buttonClickSound);
                VibrationManager.Instance.Vibrate();

                supporterPanel.SetActive(true);

                supporterPanel.transform.localScale = Vector3.zero;
                CanvasGroup cg = supporterPanel.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 0;
                LeanTween.scale(supporterPanel, Vector3.one, 0.5f)
                        .setEaseOutBack()
                        .setIgnoreTimeScale(true);
                if (cg != null)
                {
                    LeanTween.alphaCanvas(cg, 1f, 0.4f)
                            .setIgnoreTimeScale(true);
                }
            }

            else
            {
                AnimateThanksPanel();
            }
            
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y activa el panel de supporter animado con LeanTween.
        /// </summary>
        public void OnCloseSupporterPanelButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();

            CanvasGroup cg = supporterPanel.GetComponent<CanvasGroup>();

            LeanTween.scale(supporterPanel, Vector3.zero, 0.4f)
                    .setEaseInBack()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() => supporterPanel.SetActive(false));

            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 0f, 0.3f)
                        .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Reproduce el sonido de supporter, vibra, guarda que es supporter, añade 1000 monedas,
        /// esconde el botón supporter y activa el panel de supporter animado con LeanTween
        /// y la imagen supporter.
        /// </summary>
        public void OnSupporterCompleted()
        {
            AudioManager.Instance.PlaySFX(supportSound);
            VibrationManager.Instance.Vibrate();

            LevelPlayManager.Instance.HideBanner();

            GameManager.Instance.SaveIsSupporter();
            GameManager.Instance.AddCoins(1000);

            supporterPanel.SetActive(false);
            AnimateThanksPanel();

            supporterImage.SetActive(true);
        }

        /// <summary>
        /// Anima el panel de agradecimiento simulando un latido mediante LeanTween.
        /// </summary>
        public void AnimateThanksPanel()
        {
            thanksPanel.SetActive(true);

            LevelPlayManager.Instance.HideBanner();

            thanksPanel.transform.localScale = Vector3.zero;

            CanvasGroup cg = thanksPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0;

            // Animación de entrada
            LeanTween.scale(thanksPanel, Vector3.one, 0.5f)
                .setEaseOutBack()
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    // Latido en bucle
                    LeanTween.scale(thanksPanel, Vector3.one * 1.05f, 0.6f)
                        .setEaseInOutSine()
                        .setLoopPingPong()
                        .setIgnoreTimeScale(true);
                });

            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                    .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y activa el panel de supporter animado con LeanTween.
        /// </summary>
        public void OnCloseThanksPanelButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();

            CanvasGroup cg = thanksPanel.GetComponent<CanvasGroup>();

            LeanTween.scale(thanksPanel, Vector3.zero, 0.4f)
                    .setEaseInBack()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() => thanksPanel.SetActive(false));

            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 0f, 0.3f)
                        .setIgnoreTimeScale(true);
            }

            LevelPlayManager.Instance.ShowBanner();
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra, carga los datos necesarios
        /// y muestra el panel del jugador animado con LeanTween.
        /// </summary>
        public void OnOpenPlayerDataButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();

            float time = GameManager.Instance.GetSessionTime();
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            sessionPlayedTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            totalCoinsCollected.text = "" + GameManager.Instance.GetTotalCollectedCoins();

            int tries = 0;
            foreach(var level in GameManager.Instance.GetLevels())
            {
                tries += level.tries;
            }

            totalTries.text = tries.ToString();

            totalCollectedStars.text = "" + GameManager.Instance.GetAllStars();

            extendedPlayerPanel.SetActive(true);

            extendedPlayerPanel.transform.localScale = Vector3.zero;

            CanvasGroup cg = extendedPlayerPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0;

            LeanTween.scale(extendedPlayerPanel, Vector3.one, 0.5f)
                    .setEaseOutBack()
                    .setIgnoreTimeScale(true);

            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                        .setIgnoreTimeScale(true);
            }

            LevelPlayManager.Instance.HideBanner();
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y oculta el panel del jugador
        /// animándolo con LeanTween.
        /// </summary>
        public void OnClosePlayerDataButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();

            CanvasGroup cg = extendedPlayerPanel.GetComponent<CanvasGroup>();

            LeanTween.scale(extendedPlayerPanel, Vector3.zero, 0.4f)
                    .setEaseInBack()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() => extendedPlayerPanel.SetActive(false));

            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 0f, 0.3f)
                        .setIgnoreTimeScale(true);
            }

            LevelPlayManager.Instance.ShowBanner();
        }


        /// <summary>
        /// Reproduce el sonido del botón, vibra y carga los Leaderboards si el dispositivo
        /// es Android o carga la escena Rankings si no es Android.
        /// </summary>
        public void OnRankingButton()
        {   
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();
#if UNITY_ANDROID
            LeaderboardManager.Instance.ShowLeaderboardUI();       
#else
            SceneManager.Instance.LoadScene("Rankings");
#endif
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y carga de logros si el dispositivo es Android.
        /// </summary>
        public void OnAchievementsButton()
        {
            #if UNITY_ANDROID
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();
            AchievementManager.Instance.ShowAchievementsUI();
            #endif
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y cierra la aplicación.
        /// </summary>
        public void OnExitButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();
            Application.Quit();
        }

        /// <summary>
        /// Cierra la sesión activa y devuelve al jugador a la pantalla de login.
        /// </summary>
        public void OnLogoutButton()
        {
            GameManager.Instance.LogOut();
            SceneManager.LoadScene("Login");
        }

    }
}