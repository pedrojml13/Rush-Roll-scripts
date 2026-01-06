using UnityEngine;
using System.Collections.Generic;
using TMPro;
namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la visualización y navegación de las skins de la bola en la interfaz de la tienda.
    /// Permite filtrar entre skins estándar y skins especiales (RM) y actualiza el material del modelo en tiempo real.
    /// </summary>
    public class BallMove : MonoBehaviour
    {
        [Header("Configuración de Tienda")]
        [SerializeField] private bool isRMShop = false;
        [SerializeField] private List<GameObject> RMBuyButtons = new List<GameObject>(); // Lista de botones para compras con dinero real, ya que cada boton va asociado a una compra
        [SerializeField] private TextMeshProUGUI indexText;
        
        private List<BallSkin> filteredSkins = new List<BallSkin>();
        private int currentLocalIndex = 0;
        private Renderer ballRenderer;

        /// <summary>
        /// Prepara la lista de skins según el tipo de tienda y muestra la primera skin de la lista filtrada al iniciar
        /// </summary>
        private void Start()
        {
            ballRenderer = GetComponent<Renderer>();
            
            RefreshSkinList();
            ShowBall(0);
        }

        /// <summary>
        /// Obtiene todas las skins del GameManager y las divide según sea la tienda
        /// de dinero real o la tienda normal.
        /// </summary>
        private void RefreshSkinList()
        {
            int rmCount = 0; // Cantidad de skins reservadas para Real Money

            List<BallSkin> allSkins = GameManager.Instance.GetAllSkins();
            for(int i=0; i<allSkins.Count; i++)
            {
                if(allSkins[i].isRMSkin)
                    rmCount++;
            }
            
            
            if (isRMShop)
                filteredSkins = allSkins.GetRange(allSkins.Count - rmCount, rmCount);
            else
                filteredSkins = allSkins.GetRange(0, allSkins.Count - rmCount);

            indexText.text = "1/" + filteredSkins.Count;
        }

        /// <summary>
        /// Avanza a la siguiente skin en la lista filtrada y si es la última regresa a la primera.
        /// </summary>
        public void ShowNextBall()
        {
            if (filteredSkins.Count == 0) return;
            if(RMBuyButtons.Count > 0)
                RMBuyButtons[currentLocalIndex].SetActive(false);
            currentLocalIndex = (currentLocalIndex + 1) % filteredSkins.Count;
            if(RMBuyButtons.Count > 0)
                RMBuyButtons[currentLocalIndex].SetActive(true);

            indexText.text = (currentLocalIndex + 1) + "/" + filteredSkins.Count;
            ShowBall(currentLocalIndex);
            
        }

        /// <summary>
        /// Retrocede a la skin anterior en la lista filtrada y si es la primera avanza a la última.
        /// </summary>
        public void ShowPreviousBall()
        {
            if (filteredSkins.Count == 0) return;
            if(RMBuyButtons.Count > 0)
                RMBuyButtons[currentLocalIndex].SetActive(false);
            currentLocalIndex = (currentLocalIndex - 1 + filteredSkins.Count) % filteredSkins.Count;
            if(RMBuyButtons.Count > 0)
                RMBuyButtons[currentLocalIndex].SetActive(true);

            indexText.text = (currentLocalIndex + 1) + "/" + filteredSkins.Count;
            ShowBall(currentLocalIndex);
        }

        /// <summary>
        /// Actualiza la textura principal de la bola.
        /// </summary>
        /// <param name="index">Indice dentro de la lista filtrada.</param>
        private void ShowBall(int index)
        {
            if (index < 0 || index >= filteredSkins.Count) return;
            
            if(filteredSkins[index].material != null)
            {
                ballRenderer.material = filteredSkins[index].material;
                if (isRMShop)
                {
                    if(filteredSkins[index].isUnlocked)
                        RMBuyButtons[index].SetActive(false);
                }
            }
        }

        /// <summary>
        /// Devuelve los datos de la skin que se está mostrando actualmente.
        /// </summary>
        /// <returns>Skin seleccionada.</returns>
        public BallSkin GetCurrentBall()
        {
            return filteredSkins[currentLocalIndex];
        } 

        /// <summary>
        /// Traduce el índice de la lista filtrada al índice de la lista completa de skins.
        /// </summary>
        /// <returns>Indice global de la skin o -1 si no se encuentra.</returns>
        public int GetCurrentBallIndex()
        {
            BallSkin currentSkin = filteredSkins[currentLocalIndex];
            List<BallSkin> allSkins = GameManager.Instance.GetAllSkins();
            
            return allSkins.IndexOf(currentSkin);
        }
    }
}