using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestor del idioma del juego.
    /// Permite cambiar y persistir la preferencia de idioma del jugador.
    /// Singleton accesible desde cualquier parte del proyecto.
    /// </summary>
    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager Instance { get; private set; }

        /// <summary>
        /// Singleton, asegura que solo exista una instancia
        /// y evita su destrucción al cambiar de escena.
        /// </summary>
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Carga el idioma guardado al iniciar la escena.
        /// </summary>
        void Start()
        {
            StartCoroutine(LoadLanguagePreference());
        }

        /// <summary>
        /// Establece el idioma actual por índice de locale.
        /// </summary>
        /// <param name="localeIndex">Indice de la localización a aplicar.</param>
        public void SetLanguage(int localeIndex)
        {
            StartCoroutine(SetLocale(localeIndex));
        }

        /// <summary>
        /// Coroutine que aplica el idioma seleccionado y lo guarda en PlayerPrefs.
        /// </summary>
        /// <param name="localeIndex">Indice de la localizacion a aplicar.</param>
        /// <returns>Que se ha inicializado.</returns>
        private IEnumerator SetLocale(int localeIndex)
        {
            yield return LocalizationSettings.InitializationOperation;

            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (localeIndex >= 0 && localeIndex < locales.Count)
            {
                LocalizationSettings.SelectedLocale = locales[localeIndex];
                PlayerPrefs.SetInt("Language", localeIndex);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Carga la preferencia de idioma guardada o detecta el idioma del dispositivo al iniciar el juego.
        /// </summary>
        private IEnumerator LoadLanguagePreference()
        {
            // Esperamos a que Localization esté inicializado
            yield return LocalizationSettings.InitializationOperation;

            var locales = LocalizationSettings.AvailableLocales.Locales;

            //Intentamos cargar la preferencia guardada
            int savedIndex = PlayerPrefs.GetInt("Language", -1);

            if (savedIndex < 0 || savedIndex >= locales.Count)
            {
                // Si no hay preferencia, detectamos idioma del dispositivo
                SystemLanguage systemLang = Application.systemLanguage;
                savedIndex = 0; // Idioma por defecto

                string systemCode = SystemLanguageToCode(systemLang);
                
                // Buscamos coincidencia en los locales disponibles
                for (int i = 0; i < locales.Count; i++)
                {
                    if (locales[i].Identifier.CultureInfo.TwoLetterISOLanguageName == systemCode)
                    {
                        savedIndex = i;
                        break;
                    }
                }

                // Guardamos el índice detectado
                PlayerPrefs.SetInt("Language", savedIndex);
                PlayerPrefs.Save();
            }

            // Aplicamos el idioma
            SetLanguage(savedIndex);
        }

        private string SystemLanguageToCode(SystemLanguage lang)
        {
            switch (lang)
            {
                case SystemLanguage.Spanish: return "es";
                case SystemLanguage.English: return "en";
                case SystemLanguage.French: return "fr";
                case SystemLanguage.German: return "de";
                case SystemLanguage.Japanese: return "ja";
                case SystemLanguage.Chinese: return "zh";
                // Agrega los que necesites
                default: return "en"; // idioma por defecto
            }
        }
    }
}
