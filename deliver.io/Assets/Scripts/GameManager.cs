using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static GameState gameState { get; private set; }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        instance = this;

        gameState = GameState.Menu;
    }

    public void StartGame()
    {
        UIManager.instance.GameStarted();
        gameState = GameState.InGame;
    }
}

public enum GameState
{
    Menu,
    InGame,
    Lose,
    Win
}
