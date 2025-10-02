using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

namespace PJML.RushAndRoll
{
    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager Instance { get; private set; }

        void Awake()
        {
            // Singleton: asegura una única instancia persistente
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            // Carga el idioma guardado al iniciar
            StartCoroutine(LoadLanguagePreference());
        }

        public void SetLanguage(int localeIndex)
        {
            // Cambia el idioma actual
            StartCoroutine(SetLocale(localeIndex));
        }

        IEnumerator SetLocale(int localeIndex)
        {
            // Espera a que LocalizationSettings esté listo
            yield return LocalizationSettings.InitializationOperation;

            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (localeIndex >= 0 && localeIndex < locales.Count)
            {
                LocalizationSettings.SelectedLocale = locales[localeIndex];
                PlayerDataManager.Instance.saveLanguage(localeIndex);
            }
        }

        IEnumerator LoadLanguagePreference()
        {
            // Espera a que LocalizationSettings esté listo
            yield return LocalizationSettings.InitializationOperation;

            // Carga el idioma guardado y lo aplica
            PlayerDataManager.Instance.LoadLanguage(langIndex =>
            {
                SetLanguage(langIndex);
            });
        }
    }
}