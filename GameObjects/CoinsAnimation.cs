using UnityEngine;
using System.Collections;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Animación simple para monedas.
    /// Permite rotación, flotación vertical y escalado pulsante con easing.
    /// Se puede configurar por eje, velocidad y altura de desplazamiento.
    /// </summary>
    public class CoinsAnim : MonoBehaviour
    {
        [Header("Rotación")]
        public bool isRotating = false;
        public bool rotateX = false;
        public bool rotateY = false;
        public bool rotateZ = false;
        public float rotationSpeed = 90f;

        [Header("Flotación")]
        public bool isFloating = false;
        public bool useEasingForFloating = false;
        public float floatHeight = 1f;
        public float floatSpeed = 1f;
        private Vector3 initialPosition;
        private float floatTimer;

        [Header("Escalado")]
        private Vector3 initialScale;
        public Vector3 startScale;
        public Vector3 endScale;
        public bool isScaling = false;
        public bool useEasingForScaling = false;
        public float scaleLerpSpeed = 1f;
        private float scaleTimer;

        /// <summary>
        /// Inicializa variables y ajustes iniciales.
        /// </summary>
        void Start()
        {
            initialScale = transform.localScale;
            initialPosition = transform.position;

            startScale = initialScale;
            endScale = initialScale * (endScale.magnitude / startScale.magnitude);

            // Rotación aleatoria inicial para dar variedad
            transform.Rotate(new Vector3(0, Random.Range(0f, 360f), 0));
        }

        /// <summary>
        /// Actualiza animaciones cada frame.
        /// </summary>
        void Update()
        {
            // Rotación
            if (isRotating)
            {
                Vector3 rotationVector = new Vector3(
                    rotateX ? 1 : 0,
                    rotateY ? 1 : 0,
                    rotateZ ? 1 : 0
                );

                transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
            }

            if (isFloating)
            {
                floatTimer += Time.deltaTime * floatSpeed;
                float t = Mathf.PingPong(floatTimer, 1f);

                if (useEasingForFloating)
                    t = EaseInOutQuad(t);

                transform.position = initialPosition + new Vector3(0, t * floatHeight, 0);
            }

            if (isScaling)
            {
                scaleTimer += Time.deltaTime * scaleLerpSpeed;
                float t = Mathf.PingPong(scaleTimer, 1f);

                if (useEasingForScaling)
                    t = EaseInOutQuad(t);

                transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }
        }

        /// <summary>
        /// Función de easing cuadrático in-out para suavizar animaciones.
        /// </summary>
        /// <param name="t">Valor normalizado entre 0 y 1</param>
        /// <returns>Valor suavizado entre 0 y 1</returns>
        float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        }
    }
}
