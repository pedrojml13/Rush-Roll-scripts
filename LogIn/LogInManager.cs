using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using TMPro;
using System;
using System.Collections;
using Google;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona el flujo de autenticación del usuario, incluyendo Login, Registro 
    /// y persistencia de sesión tanto en Android (GPGS) como en PC (Firebase).
    /// </summary>
    public class LoginManager : MonoBehaviour
    {
        [Header("Paneles de Interfaz")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject loginPanelGPGS;
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject loadingIcon;

        [Header("Inputs de Login")]
        [SerializeField] private TMP_InputField loginEmailInput;
        [SerializeField] private TMP_InputField loginPasswordInput;

        [Header("Inputs de Registro")]
        [SerializeField] private TMP_InputField registerEmailInput;
        [SerializeField] private TMP_InputField registerPasswordInput;
        [SerializeField] private TMP_InputField registerConfirmInput;
        [SerializeField] private TMP_InputField usernameInput;

        [Header("Feedback Visual")]
        [SerializeField] private GameObject succ;
        [SerializeField] private GameObject err;
        [SerializeField] private GameObject usernameTakenMsg;

        /// <summary>
        /// Inicializa la UI, si tiene internet intenta logearse automáticamente y si no se logea offline.
        /// </summary>
        private void Start()
        {
            
            AudioManager.Instance.StopMusic();

            //GameManager.Instance.LogOut();
            InitializeUI();
            // Si hay internet nos logeamos, si no vamos al menu
            if(GameManager.Instance.HasInternet())
                AttemptAutoLogin();
            else
            {
                NavigateToMenu();
            }
                
        }

        /// <summary>
        /// Establece el estado inicial de los paneles de la UI.
        /// </summary>
        private void InitializeUI()
        {
            ShowLoadingScreen();
        }

        /// <summary>
        /// Intenta iniciar sesión automáticamente con GPGS si el dispositivo es Android
        /// o en Firebase si es otro.
        /// </summary>
        private void AttemptAutoLogin()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                // Intento de login con Google Play Games Services
                AuthManager.Instance.LoginWithGPGS((success, error) =>
                {
                    if (success)
                    {

                        NavigateToMenu();
                    }
                    else
                    {
                        HideLoadingScreen();
                        ShowGPGSLoginPanel();
                    }
                });
            #else
            GameManager.Instance.LogOut();
                FirebaseUser user = GameManager.Instance.GetCurrentUser();

                if (user != null)
                {
                    NavigateToMenu();
                }
                else
                {
                    ShowLoginPanel();
                }
            #endif
        }

        /// <summary>
        /// Ejecuta el proceso de inicio de sesión manual con Email.
        /// </summary>
        public void OnLoginButton()
        {
            string email = loginEmailInput.text;
            string password = loginPasswordInput.text;

            loadingPanel.SetActive(true);
            loginPanel.SetActive(false);

            AuthManager.Instance.Login(email, password, (success, error) =>
            {
                if (success)
                {
                    NavigateToMenu();
                }
                else
                {
                    loadingPanel.SetActive(false);
                    loginPanel.SetActive(true);
                }
            });
        }

        /// <summary>
        /// Gestiona el registro de nuevos usuarios, validando disponibilidad de nombre y coincidencia de contraseñas.
        /// </summary>
        public void OnRegisterButton()
        {
            string email = registerEmailInput.text;
            string password = registerPasswordInput.text;
            string confirm = registerConfirmInput.text;
            string username = usernameInput.text;

            if (password != confirm)
            {
                err.SetActive(true);
                return;
            }

            // Validar disponibilidad del nombre de usuario antes de crear la cuenta
            PlayerDataManager.Instance.CheckUsernameAvailability(username, isAvailable =>
            {
                if (!isAvailable)
                {
                    err.SetActive(true);
                    usernameTakenMsg.SetActive(true);
                }
                else
                {
                    AuthManager.Instance.Register(email, password, (success, error) =>
                    {
                        if (success)
                        {
                            succ.SetActive(true);
                            err.SetActive(false);
                            PlayerDataManager.Instance.SaveUsername(username);
                        }
                        else
                        {
                            err.SetActive(true);
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Al pulsar el botón offline va al menú.
        /// </summary>
        public void OnOfflineButton()
        {
            NavigateToMenu();
        }

        
        private void NavigateToMenu()
        {
            loadingPanel.SetActive(true);
            GameManager.Instance.WaitForSessionReady(() =>
            {
                SceneManager.LoadScene("Menu");
            });
        }

        /// <summary>
        /// Muestra la pantalla de carga.
        /// </summary>
        private void ShowLoadingScreen()
        {
            loadingPanel.SetActive(true);

            RectTransform rt = loadingPanel.GetComponent<RectTransform>();
            CanvasGroup cg = loadingPanel.GetComponent<CanvasGroup>();

            if (cg != null)
                cg.alpha = 0f; // Fade inicial

            // Guardamos posición final
            Vector2 finalPos = rt.anchoredPosition;

            // Posición inicial fuera de pantalla (arriba)
            rt.anchoredPosition = finalPos + new Vector2(0f, 800f);

            // Movimiento hacia abajo
            LeanTween.move(rt, finalPos, 0.6f)
                    .setEaseOutCubic() // suave, desacelerando al final
                    .setIgnoreTimeScale(true);

            // Fade in
            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                        .setIgnoreTimeScale(true);
            }

            LeanTween.scale(loadingIcon.gameObject, Vector3.one * 1.1f, 0.5f)
         .setEaseInOutSine()
         .setLoopPingPong()
         .setIgnoreTimeScale(true);
        }

        /// <summary>
        /// Oculta la pantalla de carga.
        /// </summary>
        private void HideLoadingScreen()
        {
            RectTransform rt = loadingPanel.GetComponent<RectTransform>();
            CanvasGroup cg = loadingPanel.GetComponent<CanvasGroup>();

            Vector2 targetPos = rt.anchoredPosition + new Vector2(0f, 800f); // sube hacia arriba

            // Fade out
            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 0f, 0.4f)
                        .setIgnoreTimeScale(true);
            }

            // Movimiento hacia arriba
            LeanTween.move(rt, targetPos, 0.6f)
                    .setEaseInCubic() // sube suavemente, acelerando al inicio
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() => loadingPanel.SetActive(false)); // desactiva al terminar
        }

        /// <summary>
        /// Muestra el panel de registro.
        /// </summary>
        public void ShowRegisterPanel()
        {
            registerPanel.SetActive(true);
            loginPanel.SetActive(false);

            RectTransform rt = registerPanel.GetComponent<RectTransform>();
            CanvasGroup cg = registerPanel.GetComponent<CanvasGroup>();

            if (cg != null)
                cg.alpha = 0f;

            // Posición final (la del layout)
            Vector2 finalPos = rt.anchoredPosition;

            // Posición inicial (fuera por abajo)
            rt.anchoredPosition = finalPos - new Vector2(0f, 800f);

            // Movimiento hacia arriba
            LeanTween.move(rt, finalPos, 0.6f)
                    .setEaseOutCubic()
                    .setIgnoreTimeScale(true);

            // Fade in
            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                        .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Muestra el panel de login.
        /// </summary>
        public void ShowLoginPanel()
        {
            loginPanel.SetActive(true);
            HideLoadingScreen();
            registerPanel.SetActive(false);

            RectTransform rt = loginPanel.GetComponent<RectTransform>();
            CanvasGroup cg = loginPanel.GetComponent<CanvasGroup>();

            if (cg != null)
                cg.alpha = 0f;

            // Posición final (la del layout)
            Vector2 finalPos = rt.anchoredPosition;

            // Posición inicial (fuera por abajo)
            rt.anchoredPosition = finalPos - new Vector2(0f, 800f);

            // Movimiento hacia arriba
            LeanTween.move(rt, finalPos, 0.6f)
                    .setEaseOutCubic()
                    .setIgnoreTimeScale(true);

            // Fade in
            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                        .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Muestra el panel de login con GPGS.
        /// </summary>
        public void ShowGPGSLoginPanel()
        {
            loginPanelGPGS.SetActive(true);

            RectTransform rt = loginPanelGPGS.GetComponent<RectTransform>();
            CanvasGroup cg = loginPanelGPGS.GetComponent<CanvasGroup>();

            if (cg != null)
                cg.alpha = 0f;

            // Posición final (la del layout)
            Vector2 finalPos = rt.anchoredPosition;

            // Posición inicial (fuera por abajo)
            rt.anchoredPosition = finalPos - new Vector2(0f, 800f);

            // Movimiento hacia arriba
            LeanTween.move(rt, finalPos, 0.6f)
                    .setEaseOutCubic()
                    .setIgnoreTimeScale(true);

            // Fade in
            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                        .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Ejecuta el proceso de inicio de sesión con GPGS.
        /// </summary>
        public void OnGPGSLoginButton()
        {
            string email = loginEmailInput.text;
            string password = loginPasswordInput.text;

            loadingPanel.SetActive(true);
            loginPanel.SetActive(false);

                // Intento de login con Google Play Games Services
                AuthManager.Instance.LoginWithGPGS((success, error) =>
                {
                    if (success)
                    {

                        NavigateToMenu();
                    }
                    else
                    {
                        ShowGPGSLoginPanel();
                    }
                });
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y cierra la aplicación.
        /// </summary>
        public void OnExitButton()
        {
            Application.Quit();
        }

    }
}