using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject unlockIcon;
    [SerializeField] private Image[] stars;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private Button playButton;

    private int levelIndex;

    public void Setup(int index, bool unlocked, int starCount, float bestTime)
    {
        levelIndex = index;
        levelNumberText.text = (index + 1).ToString();

        lockIcon.SetActive(!unlocked);
        unlockIcon.SetActive(unlocked);
        playButton.interactable = unlocked;

        // estrellas
        for (int i = 0; i < stars.Length; i++)
            stars[i].enabled = (i < starCount);

        // tiempo
        bestTimeText.text = unlocked ? $"{bestTime:F1}s" : "--";
    }

    public void OnClick()
    {
        if (playButton.interactable)
        {
            LevelSelectorManager.Instance.LoadLevel(levelIndex);
        }
    }
}
