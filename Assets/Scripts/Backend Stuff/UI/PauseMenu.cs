using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public Player player;
    public AudioSource backgroundMusic;

    private void Awake()
    {
        player.GetComponent<Player>().enabled = true;
    }

    void Update()
    {

        if (InputManager.PauseIsPressed)
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

        backgroundMusic.volume = backgroundMusic.volume * 2;
        
        player.GetComponent<Player>().enabled = true;

    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);

        Time.timeScale = -0f;
        GameIsPaused = true;

        backgroundMusic.volume = backgroundMusic.volume /2;
        backgroundMusic.pitch = 1;

        player.GetComponent<Player>().enabled = false;

    }

    public void LoadMenu()
    {
        StartCoroutine(LoadSceneAsync());
    }


    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
