using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Orquestador principal de la interfaz de la tienda.
    /// Gestiona la actualización de textos, estados de botones (Comprar/Seleccionar/Equipado)
    /// y la comunicación entre la lógica de monedas del GameManager y la visualización de skins.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [Header("Referencias de Lógica")]
        [SerializeField] private BallMove ballMove;
        [SerializeField] private BuyBall buyBall;

        [Header("UI - Textos de Información")]
        [SerializeField] private TextMeshProUGUI coinsNumberText;
        [SerializeField] private TextMeshProUGUI ballNameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI lockedText;
        [SerializeField] private TextMeshProUGUI unlockedText;
        [SerializeField] private RawImage playerAvatar;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private GameObject supporterImage, gameEndedImage;

        [Header("UI - Botones y Estados")]
        [SerializeField] private TextMeshProUGUI buyButtonText;
        [SerializeField] private TextMeshProUGUI selectButtonText;
        [SerializeField] private TextMeshProUGUI selectedButtonText;
        [SerializeField] private Button buyButton;
        [SerializeField] private GameObject buyCoinsButton;
        [SerializeField] private Button watchAdButton;
        [SerializeField] private GameObject notEnoughtCoinsText, selectedText, boughtText, alreadySelectedText, ownedImage;
        [SerializeField] private TextMeshProUGUI adCooldownText;
        [SerializeField] private float adCooldown = 60f;
        [SerializeField] private bool isRM = false;

        [Header("UI - Paneles Especiales")]
        [SerializeField] private GameObject thanksPanel = null;
        [SerializeField] private GameObject rMCoinsPanel = null;

        [Header("Sonidos")]
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip coinsBuySound;
        [SerializeField] private AudioClip RMSkinUnlockedSound;
        [SerializeField] private AudioClip shopMusic;
        

        /// <summary>
        /// Inicializa la tienda mostrando el saldo actual del jugador y refrescando la UI.
        /// </summary>
        private IEnumerator Start()
        {

            UpdateUI();

            AudioManager.Instance.PlayMusic(shopMusic);

            if(isRM){
                LevelPlayManager.Instance.HideBanner(); // En la tienda RM no mostramos el banner
            }
            else
            {
                LevelPlayManager.Instance.ShowBanner();
            }
            

            if(GameManager.Instance.HasInternet())
            {
                playerName.text = GPGSManager.Instance.GetUsername();
                
                while (GPGSManager.Instance.GetUserImage() == null)
                {
                    yield return null;
                }
                playerAvatar.texture = GPGSManager.Instance.GetUserImage();
            }

            else
            {
                if(buyCoinsButton != null)
                    buyCoinsButton.SetActive(false);
                if(watchAdButton != null)
                watchAdButton.gameObject.SetActive(false);

                string username = GameManager.Instance.GetUsername();
                if (!string.IsNullOrEmpty(username))
                {
                    playerName.text = username;
                }
            } 
            
            if (supporterImage != null && GameManager.Instance.IsSupporter())
            {
                supporterImage.SetActive(true);
            }
            if (gameEndedImage != null && GameManager.Instance.IsGameEnded())
            {
                gameEndedImage.SetActive(true);
            }
            
            
        }

        /// <summary>
        /// Comprueba si ha pasado el tiempo necesario para activar el botón del anuncio.
        /// </summary>
        void Update()
        {
            if(watchAdButton != null)
            {
                float timeLeft = Mathf.Max(0, adCooldown - GameManager.Instance.GetLastAdTime());
                if(timeLeft <= 0f)
                {
                    timeLeft = 0f;
                    adCooldownText.text = "";
                }
                else
                    adCooldownText.text = Mathf.Ceil(timeLeft).ToString() + "s";

                watchAdButton.interactable = timeLeft <= 0;
            }
        }

        /// <summary>
        /// Actualiza todos la interfaz de la tienda y la skin seleccionada.
        /// </summary>
        void UpdateUI()
        {

            if(coinsNumberText != null)
                coinsNumberText.text = "" + GameManager.Instance.GetCoins();

            BallSkin currentBall = ballMove.GetCurrentBall();
            if (currentBall == null) return;

            if(!currentBall.isRMSkin && priceText != null)
                priceText.text = "" + currentBall.price;
            else if (ownedImage != null)
                ownedImage.SetActive(currentBall.isUnlocked);


            unlockedText.enabled = currentBall.isUnlocked;
            lockedText.enabled = !currentBall.isUnlocked;
            
            if (currentBall == GameManager.Instance.GetSelectedBallSkin())
            {
                if(buyButton != null)
                    buyButton.interactable = true;

                selectButtonText.enabled = false;
                buyButtonText.enabled = false;
                selectedButtonText.enabled = true;
                ballNameText.text = currentBall.name;
            }
            else if (currentBall.isUnlocked)
            {
                if(buyButton != null)
                    buyButton.interactable = true;

                ballNameText.text = currentBall.name;
                buyButtonText.enabled = false;
                selectButtonText.enabled = true;
                selectedButtonText.enabled = false;
            }
            else
            {
                if(buyButton != null)
                    buyButton.interactable = false;

                ballNameText.text = "???";
                buyButtonText.enabled = true;
                selectButtonText.enabled = false;
                selectedButtonText.enabled = false;
            }
        }

        /// <summary>
        /// Vibra, reproduce el sonido del botón y carga la escena del menú principal.
        /// </summary>
        public void OnMenuButton()
        {
            VibrationManager.Instance.Vibrate();
            AudioManager.Instance.PlaySFX(buttonClickSound);

            SceneManager.LoadScene("Menu");
        }

        /// <summary>
        /// Reproduce el sonido del botón, selecciona la siguiente skin y refresca la interfaz.
        /// </summary>
        public void OnNextBallButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);

            ballMove.ShowNextBall();
            UpdateUI();
        }

        /// <summary>
        /// Reproduce el sonido del botón, selecciona la skin anterior y refresca la interfaz.
        /// </summary>
        public void OnPreviousBallButton()
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);

            ballMove.ShowPreviousBall();
            UpdateUI();
        }

        /// <summary>
        /// Vibra reproduce el sonido del botón, equipa la skin si ya está desbloqueada, la compra
        /// si el jugador tiene dinero suficiente o muestra que no tiene dinero suficiente.
        /// </summary>
        public void OnBuyButton()
        {
            VibrationManager.Instance.Vibrate();
            AudioManager.Instance.PlaySFX(buttonClickSound);

            // Si ya es propiedad del jugador, simplemente la equipa
            if (GameManager.Instance.IsSkinUnloked(ballMove.GetCurrentBallIndex()))
            {
                if(GameManager.Instance.GetSelectedBallSkin() == GameManager.Instance.GetAllSkins()[ballMove.GetCurrentBallIndex()])
                {
                    alreadySelectedText.SetActive(true);
                    StartCoroutine(HideGameObjectAfterDelay(1f, alreadySelectedText));
                }
                else
                {
                    selectedText.SetActive(true);
                    StartCoroutine(HideGameObjectAfterDelay(1f, selectedText));
                    buyBall.SelectCurrentBall();
                }
                
            }
            // Si es nueva y hay dinero suficiente, procede con la compra
            else if (GameManager.Instance.GetCoins() >= ballMove.GetCurrentBall().price)
            {
                boughtText.SetActive(true);
                StartCoroutine(HideGameObjectAfterDelay(1f, boughtText));
                buyBall.BuyCurrentBall();
                // Actualiza el contador de monedas tras el gasto
                coinsNumberText.text = "" + GameManager.Instance.GetCoins();
            }
            else
            {
                notEnoughtCoinsText.SetActive(true);
                StartCoroutine(HideGameObjectAfterDelay(1f, notEnoughtCoinsText));
            }

            UpdateUI();
        }

        /// <summary>
        /// Oculta un GameObject después de cierto tiempo.
        /// </summary>
        /// <param name="delay">Tiempo a ocultar el objeto.</param>
        /// <param name="gameObject">Objeto a ocultar.</param>
        /// <returns></returns>
        private IEnumerator HideGameObjectAfterDelay(float delay, GameObject gameObject)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Vibra, reproduce el sonido de rmSkin y gestiona la compra de skins con dinero real.
        /// Muestra un panel de agradecimiento animado con Lean tween y desbloquea la skin
        /// independientemente del saldo de monedas.
        /// </summary>
        public void OnRMBuyButton()
        {
            VibrationManager.Instance.Vibrate();
            if(RMSkinUnlockedSound != null)
                AudioManager.Instance.PlaySFX(RMSkinUnlockedSound);
            
            thanksPanel.SetActive(true);

            RectTransform rt = thanksPanel.GetComponent<RectTransform>();

            // Cancelamos animaciones previas
            LeanTween.cancel(rt);

            // Escala inicial
            rt.localScale = Vector3.zero;

            // Animación de entrada
            LeanTween.scale(rt, Vector3.one, 0.4f)
                .setEaseOutBack()
                .setIgnoreTimeScale(true);
            
            if (GameManager.Instance.IsSkinUnloked(ballMove.GetCurrentBallIndex()))
            {
                buyBall.SelectCurrentBall();
            }
            else
            {
                buyBall.BuyCurrentBall();
            }
            
            UpdateUI();
        }

        /// <summary>
        /// Activa el panel de compra de monedas con dinero real animándolo con LeanTween.
        /// </summary>
        public void OnOpenRMCoinsPanel()
        {
            VibrationManager.Instance.Vibrate();
            AudioManager.Instance.PlaySFX(buttonClickSound);

            rMCoinsPanel.SetActive(true);

            RectTransform rt = rMCoinsPanel.GetComponent<RectTransform>();

            // Cancelamos animaciones previas
            LeanTween.cancel(rt);

            // Escala inicial
            rt.localScale = Vector3.zero;

            // Animación de entrada
            LeanTween.scale(rt, Vector3.one, 0.4f)
                .setEaseOutBack()
                .setIgnoreTimeScale(true);
        }

        /// <summary>
        /// Oculta el panel de compra de monedas con dinero real animándolo con LeanTween.
        /// </summary>
        public void OnCloseRMCoinsPanel()
        {
            VibrationManager.Instance.Vibrate();
            AudioManager.Instance.PlaySFX(buttonClickSound);

            RectTransform rt = rMCoinsPanel.GetComponent<RectTransform>();

            LeanTween.cancel(rt);

            LeanTween.scale(rt, Vector3.zero, 0.3f)
                .setEaseInBack()
                .setIgnoreTimeScale(true)
                .setOnComplete(() => rMCoinsPanel.SetActive(false));
        }

        /// <summary>
        /// Oculta el panel de agradecimiento animándolo con LeanTween.
        /// </summary>
        public void OnCloseThanksPanel()
        {
            VibrationManager.Instance.Vibrate();
            AudioManager.Instance.PlaySFX(buttonClickSound);

            RectTransform rt = thanksPanel.GetComponent<RectTransform>();

            LeanTween.cancel(rt);

            LeanTween.scale(rt, Vector3.zero, 0.3f)
                .setEaseInBack()
                .setIgnoreTimeScale(true)
                .setOnComplete(() => thanksPanel.SetActive(false));
        }

        /// <summary>
        /// Vibra, reproduce el sonido de compra de monedas y añade 500 monedas al jugador.
        /// </summary>
        public void OnRMCoinsButtonX500()
        {
            VibrationManager.Instance.Vibrate();
            if(coinsBuySound != null)
                AudioManager.Instance.PlaySFX(coinsBuySound);
            GameManager.Instance.AddCoins(500);
            coinsNumberText.text = "" + GameManager.Instance.GetCoins();
        }

        /// <summary>
        /// Vibra, reproduce el sonido de compra de monedas y añade 2500 monedas al jugador.
        /// </summary>
        public void OnRMCoinsButtonX2500()
        {
            VibrationManager.Instance.Vibrate();
            if(coinsBuySound != null)
                AudioManager.Instance.PlaySFX(coinsBuySound);
            GameManager.Instance.AddCoins(2500);
            coinsNumberText.text = "" + GameManager.Instance.GetCoins();
        }
        
        /// <summary>
        /// Vibra, reproduce el sonido de compra de monedas y añade 6000 monedas al jugador.
        /// </summary>
        public void OnRMCoinsButtonX6000()
        {
            VibrationManager.Instance.Vibrate();
            if(coinsBuySound != null)
                AudioManager.Instance.PlaySFX(coinsBuySound);
            GameManager.Instance.AddCoins(6000);
            coinsNumberText.text = "" + GameManager.Instance.GetCoins();
        }

        /// <summary>
        /// Llama al metodo ShowRewarded del LevelPlayManager y, al finalizar el anuncio,
        /// añade 50 monedas al jugador.
        /// </summary>
        public void OnWatchAdButton()
        {
            LevelPlayManager.Instance.ShowRewarded(() =>
            {
                GameManager.Instance.AddCoins(50);
                UpdateUI();
                if(coinsBuySound != null)
                    AudioManager.Instance.PlaySFX(coinsBuySound);
                // Reinicia el temporizador para el siguiente anuncio
                GameManager.Instance.ResetLastAdTime();
            });
        }
    }
}