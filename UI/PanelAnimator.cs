using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    public class PanelAnimator : MonoBehaviour
    {
        [SerializeField] GameObject backPanel, homeButton, coins;
        void OnEnable()
        {
            LeanTween.scale(homeButton, new Vector3(1.5f, 1.5f, 1.5f), 2f).setDelay(.5f).setEase(LeanTweenType.easeOutElastic);
        }
    }
}