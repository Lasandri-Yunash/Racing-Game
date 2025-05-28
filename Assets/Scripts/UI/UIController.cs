using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject settings;
    public GameObject tournaments;

    public void GoToGarage(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToRace(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToAgainstTime(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToPractice(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenTournaments()
    {
        settings.SetActive(false);
        tournaments.SetActive(true);
    }

    public void CloseTournaments()
    {
        tournaments.SetActive(false);
        settings.SetActive(true);
    }
    
}
