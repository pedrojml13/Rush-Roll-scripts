using System;
namespace PJML.RushAndRoll
{
    [Serializable]
    public class LevelData
    {
        public int levelIndex;
        public bool unlocked;
        public int stars;
        public float bestTime;

        public LevelData(int index)
        {
            levelIndex = index;
            unlocked = false;
            stars = 0;
            bestTime = 0f;
        }
    }
}