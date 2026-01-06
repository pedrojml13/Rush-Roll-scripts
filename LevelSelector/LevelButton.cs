using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la interfaz de un botón de nivel en el menú de selección de niveles.
    /// </summary>
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelNumberText;
        [SerializeField] private Image lockIcon;
        //[SerializeField] private RawImage unlockIcon;
        [SerializeField] private Image[] stars;
        [SerializeField] private TMP_Text bestTimeText;
        [SerializeField] private TMP_Text bestTimeNumberText;
        [SerializeField] private Button playButton;

        private int levelIndex;

        /// <summary>
        /// Configura el botón con los datos del nivel.
        /// </summary>
        /// <param name="index">Indice del nivel.</param>
        /// <param name="unlocked">Si está desbloqueado.</param>
        /// <param name="starCount">Número de estrellas.</param>
        /// <param name="bestTime">Mejor tiempo.</param>
        public void Setup(int index, bool unlocked, int starCount, float bestTime)
        {
            levelIndex = index;
            levelNumberText.text = ((index % 9) + 1).ToString();

            lockIcon.gameObject.SetActive(!unlocked);
            playButton.interactable = unlocked;

            // Configura estrellas
            for (int i = 0; i < stars.Length; i++)
                stars[i].gameObject.SetActive(i < starCount);

            // Configura el tiempo
            bestTimeText.gameObject.SetActive(unlocked);

            if (bestTime == 0f && unlocked)
                bestTimeNumberText.text = "X";
            else if (unlocked)
                bestTimeNumberText.text = GetElapsedTimeFormatted(bestTime);
            else
                bestTimeNumberText.text = "";
        }

        /// <summary>
        /// Llama al LevelSelectorManager para cargar el nivel correspondiente.
        /// </summary>
        public void OnClick()
        {
            if (playButton.interactable)
            {
                LevelSelectorManager.Instance.LoadLevel(levelIndex);
            }
        }

        /// <summary>
        /// Devuelve el tiempo formateado.
        /// </summary>
        /// <param name="time">Tiempo a formatear</param>
        /// <returns></returns>
        private string GetElapsedTimeFormatted(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int centiseconds = Mathf.FloorToInt((time * 100f) % 100f);

            return $"{minutes:00}:{seconds:00}.{centiseconds:00}";
        }
    }
}
