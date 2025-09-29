using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private BallMove ballMove;
    [SerializeField] private BuyBall buyBall;
    [SerializeField] private TextMeshProUGUI coinsNumberText;
    [SerializeField] private TextMeshProUGUI lockedText;
    [SerializeField] private TextMeshProUGUI unlockedText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI ballNameText;
    [SerializeField] private TextMeshProUGUI buyButtonText;
    [SerializeField] private TextMeshProUGUI selectButtonText;
    [SerializeField] private TextMeshProUGUI selectedButtonText;

    void Start()
    {
        coinsNumberText.text = "" + GameManager.Instance.GetCoins();
        UpdateUI();
    }

    void UpdateUI()
    {
        BallSkin currentBall = ballMove.GetCurrentBall();
        if (currentBall != null)
        {
            
            priceText.text = "" + currentBall.price;

            if (currentBall.isUnlocked)
            {
                unlockedText.enabled = true;
                lockedText.enabled = false;
            }

            else
            {
                unlockedText.enabled = false;
                lockedText.enabled = true;
            }

            if (currentBall == GameManager.Instance.GetSelectedBallSkin())
            {
                selectButtonText.enabled = false;
                buyButtonText.enabled = false;
                selectedButtonText.enabled = true;
            }
            else if (currentBall.isUnlocked)
            {
                ballNameText.text = currentBall.name;
                buyButtonText.enabled = false;
                selectButtonText.enabled = true;
                selectedButtonText.enabled = false;
            }
            else
            {
                ballNameText.text = "???";
                buyButtonText.enabled = true;
                selectButtonText.enabled = false;
                selectedButtonText.enabled = false;
            }
        }
    }


    public void OnMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnNextBallButton()
    {
        ballMove.ShowNextBall();
        UpdateUI();
    }

    public void OnPreviousBallButton()
    {
        ballMove.ShowPreviousBall();
        UpdateUI();
    }

    public void OnBuyButton()
    {


        if (GameManager.Instance.IsSkinUnloked(ballMove.GetCurrentBallIndex()))
        {
            buyBall.SelectCurrentBall();
        }

        else if (GameManager.Instance.GetCoins() >= ballMove.GetCurrentBall().price)
        {
            GameManager.Instance.RemoveCoins(ballMove.GetCurrentBall().price);
            buyBall.BuyCurrentBall();
            coinsNumberText.text = "" + GameManager.Instance.GetCoins();
        }


        else
        {
            //NotEnoughCoins
        }
        

        UpdateUI();
    }
}
