using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    InGame,
    ExitingLevel,
    Win,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameState currentGameState;

    public GameObject winScene;
    public GameObject gameOverScreen;

    public Animator doorAnimator;

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

        switch (currentGameState)
        {
            case GameState.ExitingLevel:
                Debug.Log("Exiting Level State!");
                ExitingLevel();
                break;
            case GameState.Win:
                WinLevel();
                break;
            case GameState.GameOver:
                GameOver();
                break;
        }
    }

    // Player exiting the level
    private void ExitingLevel()
    {
        doorAnimator.SetBool("IsDoorOpen", true);

        StartCoroutine(Exiting(1.5f));
    }

    private IEnumerator Exiting(float timeInSec)
    {
        yield return new WaitForSecondsRealtime(timeInSec);
        if (GameStats.NumOfItems != GameStats.totalNumOfItems)
        {
            currentGameState = GameState.GameOver;
        }
        else
        {
            currentGameState = GameState.Win;
        }
    }

    private void WinLevel()
    {
        Time.timeScale = 0;
        winScene.SetActive(true);
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
