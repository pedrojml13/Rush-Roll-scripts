using UnityEngine;
using TMPro;
using System.Collections;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona las animaciones del texto de la tienda de dinero real.
    /// </summary>
    public class ShopTextAnimator : MonoBehaviour
    {
        private TMP_Text text;
        
        [Header("Tiempos de Animación")]
        [Tooltip("Duración de la transición de transparencia (Alpha).")]
        [SerializeField] private float fadeDuration = 0.5f;
        
        [Tooltip("Duración de la transición de tamaño.")]
        [SerializeField] private float scaleDuration = 0.4f;
        
        [Tooltip("Tiempo de espera entre el final de una animación y el inicio de la siguiente.")]
        [SerializeField] private float cycleDelay = 1f;

        private Color originalColor;

        /// <summary>
        /// Inicializa la animación.
        /// </summary>
        void Start()
        {
            text = GetComponent<TMP_Text>();
            originalColor = text.color;

            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            transform.localScale = Vector3.zero;

            StartCoroutine(AnimateTextLoop());
        }

        /// <summary>
        /// Corrutina que controla la animación mediante LeanTween.
        /// </summary>
        IEnumerator AnimateTextLoop()
        {
            while (true)
            {
                LeanTween.value(gameObject, 0f, 1f, fadeDuration)
                    .setOnUpdate((float val) =>
                    {
                        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, val);
                    });

                LeanTween.scale(gameObject, Vector3.one, scaleDuration).setEaseOutBack();

                yield return new WaitForSeconds(fadeDuration + cycleDelay);

                
                LeanTween.value(gameObject, 1f, 0f, fadeDuration)
                    .setOnUpdate((float val) =>
                    {
                        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, val);
                    });

                LeanTween.scale(gameObject, Vector3.zero, scaleDuration).setEaseInBack();

                yield return new WaitForSeconds(fadeDuration + cycleDelay);
            }
        }
    }
}