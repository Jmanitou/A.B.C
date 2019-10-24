using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    InGame,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameState currentGameState;

    public GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        currentGameState = GameState.InGame;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (currentGameState == GameState.GameOver)
        {
            GameOver();
        }
    }

    // Display the game over screen, score
    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverScreen.SetActive(true);
    }

    // Restart the level
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
