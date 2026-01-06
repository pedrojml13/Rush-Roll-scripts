using UnityEngine;
using System;

namespace PJML.RushAndRoll
    {
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "RushAndRoll/Levels Config")]
    public class LevelsConfig : ScriptableObject
    {
        public LevelStarConfig[] levels;
    }

    [Serializable]
    public class LevelStarConfig
    {
        public int levelIndex;
        public float timeFor3Stars;
        public float timeFor2Stars;
    }
}