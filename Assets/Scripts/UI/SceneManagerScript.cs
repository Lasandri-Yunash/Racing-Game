using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    private bool isPaused = false;
    public GameObject menuUI;
    public GameObject timerUIObject;

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        timerUIObject.SetActive(false);
        menuUI.SetActive(true);

        AudioListener.pause = true;
    }

    public void ContinueGame()
    {
        Time.timeScale  = 1f;
        isPaused = false;
        timerUIObject.SetActive(true);
        menuUI.SetActive(false);

        AudioListener.pause = false;
    }

    public void TogglePauseContinue()
    {
        if(isPaused)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }
}
