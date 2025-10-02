using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using TMPro;

namespace PJML.RushAndRoll
{
    public class LoginManager : MonoBehaviour
    {
        [Header("Paneles")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        // [SerializeField] private GameObject loadingPanel;

        [Header("Inputs de login")]
        [SerializeField] private TMP_InputField loginUsernameInput;
        [SerializeField] private TMP_InputField loginPasswordInput;

        [Header("Inputs de registro")]
        [SerializeField] private TMP_InputField registerUsernameInput;
        [SerializeField] private TMP_InputField registerPasswordInput;
        [SerializeField] private TMP_InputField registerConfirmInput;
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private GameObject succ, err;



        private void Start()
        {
            //AudioManager.Instance.PlayMusic(loginMusic);
            /*
                    if (FirebaseAuth.DefaultInstance.CurrentUser != null)
                    {
                        GameManager.Instance.isOffline = false;
                        GameManager.Instance.playerName = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;
                        LoadMenu();
                    }
                    else if (PlayerPrefs.GetInt("OfflineMode", 0) == 1)
                    {
                        GameManager.Instance.isOffline = true;
                        GameManager.Instance.playerName = "Invitado";
                        LoadMenu();
                    }
                    else
                    {
                        loginPanel.SetActive(true);
                    }*/
        }

        public void OnLoginButton()
        {
            string email = loginUsernameInput.text;
            string password = loginPasswordInput.text;
            string username = usernameInput.text;

            //loadingPanel.SetActive(true);

            AuthManager.Instance.Login(email, password, (success, error) =>
            {
                //loadingPanel.SetActive(false);

                if (success)
                {
                    GameManager.Instance.isOffline = false;
                    PlayerDataManager.Instance.SaveUsername(username);
                    LoadMenu();
                }
                else
                {
                    // Mostrar error en UI
                    Debug.LogWarning("Error de login: " + error);
                }
            });
        }

        public void OnRegisterButton()
        {
            string email = registerUsernameInput.text;
            string password = registerPasswordInput.text;
            string confirm = registerConfirmInput.text;

            if (password != confirm)
            {
                Debug.LogWarning("Las contraseñas no coinciden.");
                err.SetActive(true);
                return;
            }

            //loadingPanel.SetActive(true);

            AuthManager.Instance.Register(email, password, (success, name, error) =>
            {
                //loadingPanel.SetActive(false);

                if (success)
                {
                    succ.SetActive(true);
                    err.SetActive(false);
                }
                else
                {
                    err.SetActive(true);
                    Debug.LogWarning("Error de registro: " + error);
                }
            });
        }

        public void OnOfflineButton()
        {
            PlayerPrefs.SetInt("OfflineMode", 1);
            GameManager.Instance.isOffline = true;
            GameManager.Instance.playerName = "Invitado";
            LoadMenu();
        }

        public void ShowRegisterPanel()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
        }

        public void ShowLoginPanel()
        {
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);
        }

        private void LoadMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}