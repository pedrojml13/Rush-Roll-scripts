using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class BuyBall : MonoBehaviour
{

    public void BuyCurrentBall()
    {
        BallSkin selectedSkin = GetComponent<BallMove>().GetCurrentBall();

        GameManager.Instance.SetSelectedBallSkin(selectedSkin);

        GameManager.Instance.UnlockSkin(selectedSkin.id);

    }
    
    public void SelectCurrentBall()
    {
        BallSkin selectedSkin = GetComponent<BallMove>().GetCurrentBall();

        GameManager.Instance.SetSelectedBallSkin(selectedSkin);

    }
    
}
