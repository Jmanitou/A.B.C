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
    [SerializeField] private GameObject gameOverUI;
    public static GameState currentGameState = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentGameState = 0;
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
        gameOverUI.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
