using UnityEngine;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona el guardado local.
    /// </summary>
    public static class LocalSaveSystem
    {
        private const string KEY = "LOCAL_PLAYER_DATA";

        /// <summary>
        /// Guarda los datos en PlayerPrefs
        /// </summary>
        /// <param name="data">Datos a guardar.</param>
        public static void SaveLocal(PlayerProfileData data)
        {
            PlayerPrefs.DeleteAll();
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(KEY, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Carga los datos de usuario desde PlayerPrefs
        /// </summary>
        /// <returns>Datos cargados.</returns>
        public static PlayerProfileData LoadLocal()
        {
            if (!PlayerPrefs.HasKey(KEY))
                return CreateDefaultData();

            string json = PlayerPrefs.GetString(KEY);
            return JsonUtility.FromJson<PlayerProfileData>(json);
        }

        /// <summary>
        /// Crea datos del usuario por defecto
        /// </summary>
        /// <returns>Datos del usuario creados.</returns>
        static PlayerProfileData CreateDefaultData()
        {
            return new PlayerProfileData
            {
                username = "Player",
                coins = 0,
                totalCollectedCoins = 0,
                currentSkinId = 0,
                unlockedSkinIds = new List<int> { 0 },
                levels = CreateDefaultLevels()
            };
        }

        /// <summary>
        /// Crea una lista de niveles por defecto.
        /// </summary>
        /// <returns>Lista de niveles por defecto.</returns>
        static List<LevelData> CreateDefaultLevels()
        {
            var list = new List<LevelData>();

            for (int i = 0; i < 45; i++)
            {
                list.Add(new LevelData (i));
            }
            list[0].unlocked = true; // Primer nivel desbloqueado

            return list;
        }
    }
}