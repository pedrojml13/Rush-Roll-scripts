using UnityEngine;
using TMPro;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la visualización de una entrada individual en la lista de ránkings.
    /// </summary>
    public class RankingEntry : MonoBehaviour
    {
        [Header("Referencias de UI")]
        [Tooltip("Referencia al componente de texto donde se mostrará el nombre del nivel.")]
        [SerializeField] private TextMeshProUGUI levelText;

        private int levelNumber;

        /// <summary>
        /// Inicializa la entrada de ranking con los datos del nivel.
        /// </summary>
        /// <param name="levelNumber">Indice del nivel.</param>
        public void Setup(int levelNumber)
        {
            this.levelNumber = levelNumber;

            if (levelText != null)
            {
                levelText.text = "Nivel " + levelNumber;
            }
        }
    }
}