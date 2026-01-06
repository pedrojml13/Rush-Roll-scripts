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
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject languagePanel;

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
            loadingPanel.SetActive(true);
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            languagePanel.SetActive(false);
        }

        /// <summary>
        /// Intenta iniciar sesión automáticamente con GPGS si el dispositivo es Android
        /// o en Firebase si es otro.
        /// </summary>
        private void AttemptAutoLogin()
        {
            #if UNITY_ANDROID
                // Intento de login con Google Play Games Services
                AuthManager.Instance.LoginWithGPGS((success, error) =>
                {
                    if (success)
                    {

                        NavigateToMenu();
                    }
                    else
                    {
                        ShowLoadingScreen();
                    }
                });
            #else
                FirebaseUser user = GameManager.Instance.GetCurrentUser();

                if (user != null)
                {
                    NavigateToMenu();
                }
                else
                {
                    HandleFirstTimeLanguage();
                }
            #endif
        }

        /// <summary>
        /// Verifica si es la primera vez que el usuario entra para mostrar el selector de idioma.
        /// </summary>
        private void HandleFirstTimeLanguage()
        {
            bool hasLanguage = PlayerPrefs.HasKey("languageIndex");
            languagePanel.SetActive(!hasLanguage);
            ShowLoadingScreen();
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
            loadingPanel.SetActive(false);
            loginPanel.SetActive(true);
        }

        /// <summary>
        /// Muestra el panel de registro.
        /// </summary>
        public void ShowRegisterPanel()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
        }

        /// <summary>
        /// Muestra el panel de login.
        /// </summary>
        public void ShowLoginPanel()
        {
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);
        }


    }
}