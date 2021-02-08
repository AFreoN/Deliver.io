using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject mainMenuPanel;
    public GameObject InGamePanel;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        startSetter();
    }

    void startSetter()
    {
        mainMenuPanel.SetActive(true);
        InGamePanel.SetActive(false);
    }

    public void GameStarted()
    {
        mainMenuPanel.SetActive(false);
        InGamePanel.SetActive(true);
    }
}
