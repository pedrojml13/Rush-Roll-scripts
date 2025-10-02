using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PJML.RushAndRoll
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelNumberText;
        [SerializeField] private Image lockIcon;
        //[SerializeField] private RawImage unlockIcon;
        [SerializeField] private Image[] stars;
        [SerializeField] private TMP_Text bestTimeText;
        [SerializeField] private TMP_Text bestTimeNumberText;
        [SerializeField] private Button playButton;

        private int levelIndex;

        public void Setup(int index, bool unlocked, int starCount, float bestTime)
        {
            levelIndex = index;
            levelNumberText.text = (index + 1).ToString();

            lockIcon.gameObject.SetActive(!unlocked);
            //unlockIcon.gameObject.SetActive(unlocked);
            playButton.interactable = unlocked;

            // estrellas
            for (int i = 0; i < stars.Length; i++)
                stars[i].gameObject.SetActive(i < starCount);

            // tiempo
            bestTimeText.gameObject.SetActive(unlocked);

            if (bestTime == 0f && unlocked)
                bestTimeNumberText.text = "X";

            else if (unlocked)
                bestTimeNumberText.text = bestTime.ToString("F2") + "s";
            else
                bestTimeNumberText.text = "";
        }

        public void OnClick()
        {
            if (playButton.interactable)
            {
                LevelSelectorManager.Instance.LoadLevel(levelIndex);
            }
        }
    }
}