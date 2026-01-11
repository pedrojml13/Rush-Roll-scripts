using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la interfaz de configuración del juego, permitiendo al usuario
    /// ajustar volúmenes de audio, cambiar el idioma y activar/desactivar la vibración.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        [Header("Controles de Audio")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle vibrationToggle;

        [Header("Paneles y botones")]
        [SerializeField] private GameObject aboutPanel, giveFeedbackButton, mainPanel;

        [Header("Sonidos")]
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip settingsMusic;


        /// <summary>
        /// Inicializa los Sliders de la interfaz con los valores guardados en PlayerPrefs.
        /// Si no existen valores previos, se inicializan al máximo.
        /// </summary>
        void Start()
        {
            AudioManager.Instance.PlayMusic(settingsMusic);

            LevelPlayManager.Instance.ShowBanner();

            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

            vibrationToggle.SetIsOnWithoutNotify(VibrationManager.Instance.IsVibrationEnabled());

            musicSlider.value = musicVol;
            sfxSlider.value = sfxVol;

            LeanTween.scale(mainPanel, Vector3.one * 1.03f, 0.2f)
                .setEaseInOutSine()
                .setLoopPingPong(1);

            if (GameManager.Instance.IsOffline())
            {
                giveFeedbackButton.SetActive(false);
            }
            else
            {
                giveFeedbackButton.SetActive(true);

                LeanTween.scale(giveFeedbackButton, Vector3.one * 1.05f, 0.6f)
                    .setEaseInOutSine()
                    .setLoopPingPong();

            }
        }

        /// <summary>
        /// Actualiza el volumen de la música.
        /// </summary>
        /// <param name="volume">Volumen a aplicar.</param>
        public void SetMusicVolume(float volume)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }

        /// <summary>
        /// Actualiza el volumen de los efectos de sonido en tiempo real.
        /// </summary>
        /// <param name="volume">Volumen a aplicar.</param>
        public void SetSFXVolume(float volume)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }

        /// <summary>
        /// Reproduce el sonido del botón y guarda la configuración actual de audio y regresa a la escena del menú.
        /// </summary>
        public void OnMenuButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();

            AudioManager.Instance.SaveVolumes(musicSlider.value, sfxSlider.value);
            SceneManager.LoadScene("Menu");
        }

        /// <summary>
        /// Vibra si la vibración está activada, guarda la vibración y regresa a la escena del menú.
        /// </summary>
        public void OnVibrationToggle()
        {
            VibrationManager.Instance.SetVibration(vibrationToggle.isOn);
            if(vibrationToggle.isOn)
                VibrationManager.Instance.Vibrate();
        }

        /// <summary>
        /// Garantiza que los cambios de volumen se guarden si el usuario cierra la aplicación
        /// directamente desde la pantalla de ajustes.
        /// </summary>
        private void OnApplicationQuit()
        {
            AudioManager.Instance.SaveVolumes(musicSlider.value, sfxSlider.value);
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y cambia el idioma del sistema a Inglés.
        /// </summary>
        public void OnEnglishButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);

            VibrationManager.Instance.Vibrate();
            LanguageManager.Instance.SetLanguage(0);
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y cambia el idioma del sistema a Español.
        /// </summary>
        public void OnSpanishButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            
            VibrationManager.Instance.Vibrate();
            LanguageManager.Instance.SetLanguage(1);
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y muestra el panel about.
        /// </summary>
        public void OnOpenAboutButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            
            VibrationManager.Instance.Vibrate();
            
            aboutPanel.SetActive(true);
            
            aboutPanel.transform.localScale = Vector3.zero;

            CanvasGroup cg = aboutPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0;

            LeanTween.scale(aboutPanel, Vector3.one, 0.5f)
                    .setEaseOutBack()
                    .setIgnoreTimeScale(true);

            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 1f, 0.4f)
                        .setIgnoreTimeScale(true);
            }
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y lleva al usuario a la Play Store
        /// la API de Google Play Review.
        /// </summary>
        public void OnGiveFeedbackButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            VibrationManager.Instance.Vibrate();

            LeanTween.cancel(giveFeedbackButton);

            Application.OpenURL(
                "market://details?id=" + Application.identifier
            );
        }

        /// <summary>
        /// Reproduce el sonido del botón, vibra y cierra el panel about.
        /// </summary>
        public void OnCloseAboutButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
            
            VibrationManager.Instance.Vibrate();
            
            aboutPanel.SetActive(true);
            
            CanvasGroup cg = aboutPanel.GetComponent<CanvasGroup>();

            LeanTween.scale(aboutPanel, Vector3.zero, 0.4f)
                    .setEaseInBack()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() => aboutPanel.SetActive(false));

            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 0f, 0.3f)
                        .setIgnoreTimeScale(true);
            }
        }
    }
}