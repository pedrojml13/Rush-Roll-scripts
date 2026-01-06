using System;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Representa los datos de un nivel en el juego, incluyendo progreso, estrellas y estado del trofeo.
    /// </summary>
    [Serializable]
    public class LevelData
    {
        #region Fields
        public int levelIndex;
        public bool unlocked;
        public int stars;
        public float bestTime;
        public int tries;
        public bool trophyCollected;
        #endregion

        /// <summary>
        /// Inicializa un nuevo nivel con valores por defecto.
        /// </summary>
        /// <param name="index">Índice del nivel.</param>
        public LevelData(int index)
        {
            levelIndex = index;
            unlocked = false;
            stars = 0;
            bestTime = 0f;
            tries = 0;
            trophyCollected = false;
        }
    }
}
