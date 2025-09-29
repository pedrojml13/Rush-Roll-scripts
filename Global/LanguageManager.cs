using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(LoadLanguagePreference());
    }

    public void SetLanguage(int localeIndex)
    {
        StartCoroutine(SetLocale(localeIndex));
    }

    private IEnumerator SetLocale(int localeIndex)
    {
        // Espera a que LocalizationSettings esté inicializado
        yield return LocalizationSettings.InitializationOperation;

        // Cambia el locale seleccionado
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (localeIndex >= 0 && localeIndex < locales.Count)
        {
            LocalizationSettings.SelectedLocale = locales[localeIndex];
            PlayerDataManager.Instance.saveLanguaje(localeIndex);
        }
    }

    private IEnumerator LoadLanguagePreference()
    {
        // Espera a que LocalizationSettings esté inicializado
        yield return LocalizationSettings.InitializationOperation;

        // Carga la preferencia de idioma del jugador
        PlayerDataManager.Instance.LoadLanguaje(langIndex =>
        {
            SetLanguage(langIndex);
        });
    }
}