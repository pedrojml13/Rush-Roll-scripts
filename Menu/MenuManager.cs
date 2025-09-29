using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject menuSfere;
    private bool skinAplied = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        //LanguageManager.Instance.SetLanguage(0);

        AudioManager.Instance.StopMusic();
        if (menuSfere != null && GameManager.Instance.GetSelectedBallSkin() != null)
            menuSfere.GetComponent<Renderer>().material.mainTexture = GameManager.Instance.GetSelectedBallSkin().texture;
        
    }

    private void Update()
    {
        if (menuSfere != null)
            //menuSfere.transform.Rotate(Vector3.up * 20f * Time.deltaTime, Space.World);
        
        if (!skinAplied && GameManager.Instance.GetSelectedBallSkin() != null)
        {
            menuSfere.GetComponent<Renderer>().material.mainTexture = GameManager.Instance.GetSelectedBallSkin().texture;
            skinAplied = true;
        }
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("LevelSelector");
    }

    public void OnShopButton()
    {
        SceneManager.LoadScene("Shop");
    }

    public void OnSettingsButton()
    {
        SceneManager.LoadScene("Settings");
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

}
