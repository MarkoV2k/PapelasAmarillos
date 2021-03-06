using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
     
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public int menuID;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuID);
    }

    public void wyjscie()
    {
        Application.Quit();
    }
    
    public void opcje()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1f;
    }

    public void startGame()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }
}
