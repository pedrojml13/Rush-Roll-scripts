using UnityEngine;
using CandyCoded.HapticFeedback;
using System.Collections;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    public class VibrationManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del VibrationManager.
        /// </summary>
        public static VibrationManager Instance { get; private set; }
        private bool isVibrationEnabled = false;

        /// <summary>
        /// Asegura que solo exista una instancia del GameManager.
        /// </summary>
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            isVibrationEnabled = PlayerPrefs.GetInt("Vibration", 1) == 1;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }


        /// <summary>
        /// Activa/desactiva la vibración y lo guarda en PlayerPrefs.
        /// </summary>
        /// <param name="enabled">Activar o desactivar.</param>
        public void SetVibration(bool enabled)
        {
            isVibrationEnabled = enabled;

            PlayerPrefs.SetInt("Vibration", enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Devuelve si la vibración está activada.
        /// </summary>
        /// <returns>Vibración activada o desactivada.</returns>
        public bool IsVibrationEnabled()
        {
            return isVibrationEnabled;
        }

        /// <summary>
        /// Vibración ligera a través del HapticFeedback.
        /// </summary>
        public void Vibrate()
        {
            if(isVibrationEnabled)
                HapticFeedback.LightFeedback();
        }

        /// <summary>
        /// Vibra varias veces con una corrutina.
        /// </summary>
        /// <param name="times">Número de vibraciones.</param>
        /// <param name="interval">Intervalo entre vibraciones.</param>
        public void VibrateMultiple(int times, float interval = 0.1f)
        {
            if(isVibrationEnabled)
                StartCoroutine(VibrateRoutine(times, interval));
        }

        /// <summary>
        /// Realiza varias vibraciones ligeras a través de HapticFeedback.
        /// </summary>
        /// <param name="times"><Número de vibraciones./param>
        /// <param name="interval">Intervalo entre vibraciones.</param>
        /// <returns></returns>
        private IEnumerator VibrateRoutine(int times, float interval)
        {
            for (int i = 0; i < times; i++)
            {
                HapticFeedback.LightFeedback();
                yield return new WaitForSeconds(interval);
            }
        }

    }
}