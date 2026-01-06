using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona las acciones de compra y selección de skins para el jugador.
    /// Se comunica con el GameManager para procesar transacciones y con el sistema de logros.
    /// </summary>
    [RequireComponent(typeof(BallMove))]
    public class BuyBall : MonoBehaviour
    {
        /// <summary>
        /// Resta el precio de la skin a las monedas del jugador y si el dispositivo es Android,
        /// reporta los logros.
        /// </summary>
        public void BuyCurrentBall()
        {
            // Obtiene la skin seleccionada actualmente desde el visualizador
            BallSkin selectedSkin = GetComponent<BallMove>().GetCurrentBall();

            // Ejecuta la lógica de compra en el GameManager
            if(!selectedSkin.isRMSkin)
                GameManager.Instance.BuySkin(selectedSkin.id, selectedSkin.price);
            else
                GameManager.Instance.BuySkin(selectedSkin.id, 0);

            // Notificación condicional de logros para la plataforma Android
            #if UNITY_ANDROID
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnSkinPurchasedAchievements();
            }
            #endif
        }

        /// <summary>
        /// Selecciona la skin actual como skin del jugador.
        /// </summary>
        public void SelectCurrentBall()
        {
            // Obtiene la skin desde el visualizador
            BallSkin selectedSkin = GetComponent<BallMove>().GetCurrentBall();
            
            // Notifica al GameManager para cambiar la skin equipada
            GameManager.Instance.SelectSkin(selectedSkin.id);
        }

    }
}