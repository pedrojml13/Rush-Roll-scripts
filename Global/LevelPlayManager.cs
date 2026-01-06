using UnityEngine;
using Unity.Services.LevelPlay;
using System;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona todos los anuncios mediante Unity LevelPlay: intersticiales, recompensados y banners.
    /// </summary>
    public class LevelPlayManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del LevelManager.
        /// </summary>
        public static LevelPlayManager Instance { get; private set; }

        private LevelPlayInterstitialAd interstitial;
        private LevelPlayRewardedAd rewarded;
        private LevelPlayBannerAd banner;

        private Action onRewardCallback;
        private bool initialized;


        /// <summary>
        /// Singleton, asegura que solo exista una instancia
        /// y evita su destrucción al cambiar de escena.
        /// </summary>
        private void Awake()
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
        /// Inicializa los eventos de LevelPlay.
        /// </summary>
        void Start()
        {
            // Eventos de inicialización de LevelPlay
            LevelPlay.OnInitSuccess += OnInitSuccess;
            LevelPlay.OnInitFailed += OnInitFailed;
        }

        /// <summary>
        /// Se desuscribe los eventos de LevelPlay.
        /// </summary>
        private void OnDestroy()
        {
            LevelPlay.OnInitSuccess -= OnInitSuccess;
            LevelPlay.OnInitFailed -= OnInitFailed;

            banner?.DestroyAd();
            interstitial?.DestroyAd();
        }

        /// <summary>
        /// Marca inicializado y llama a SetupAds.
        /// </summary>
        /// <param name="config">Parámetro de configuración.</param>
        private void OnInitSuccess(LevelPlayConfiguration config)
        {
            Debug.Log("[RushAndRoll] LevelPlay inicializado correctamente.");
            initialized = true;
            SetupAds();
        }

        /// <summary>
        /// Si ha fallado se muestra por consola.
        /// </summary>
        /// <param name="error">Error a mostrar.</param>
        private void OnInitFailed(LevelPlayInitError error)
        {
            Debug.LogError($"[RushAndRoll] Error al inicializar LevelPlay: {error}");
        }

        /// <summary>
        /// Configuración de los anuncios.
        /// </summary>
        private void SetupAds()
        {
            interstitial = new LevelPlayInterstitialAd("gdvznzlxnbh2h30d");
            interstitial.OnAdClosed += _ => interstitial.LoadAd();
            interstitial.LoadAd();

            rewarded = new LevelPlayRewardedAd("fzus907wo2vxi9va");
            rewarded.OnAdRewarded += OnRewarded;
            rewarded.OnAdClosed += _ => rewarded.LoadAd();
            rewarded.LoadAd();

            banner = new LevelPlayBannerAd("4ciobn1fpqhrqbcy");
            banner.LoadAd();
        }

        /// <summary>
        /// Muestra un anuncio intersticial si está listo.
        /// </summary>
        public void ShowInterstitial()
        {
            if (initialized && interstitial?.IsAdReady() == true)
                interstitial.ShowAd();
        }

        /// <summary>
        /// Muestra un anuncio recompensado y ejecuta la acción al completarlo.
        /// </summary>
        public void ShowRewarded(Action onReward)
        {
            if (initialized && rewarded?.IsAdReady() == true)
            {
                onRewardCallback = onReward;
                rewarded.ShowAd();
            }
        }

        /// <summary>
        /// Muestra el banner de LevelPlay.
        /// </summary>
        public void ShowBanner()
        {
            banner?.ShowAd();
        }

        /// <summary>
        /// Oculta el banner de LevelPlay.
        /// </summary>
        public void HideBanner()
        {
            banner?.HideAd();
        }

        /// <summary>
        /// Si se llama al callback se invoka, si no devuelve null.
        /// </summary>
        /// <param name="info">Informacion del anuncio.</param>
        /// <param name="reward">Recompensa al ver el anuncio.</param>
        private void OnRewarded(LevelPlayAdInfo info, LevelPlayReward reward)
        {
            onRewardCallback?.Invoke();
            onRewardCallback = null;
        }
    }
}
