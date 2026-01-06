using System;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Almacena los datos del perfil del jugador.
    /// </summary>
    public class PlayerProfileData
    {
        public string username;
        public int coins;
        public int totalCollectedCoins;
        public bool isSupporter;
        public int currentSkinId;
        public List<int> unlockedSkinIds;
        public List<LevelData> levels;
        public bool gameEnded;
        public float totalPlayedTime;
    }
}