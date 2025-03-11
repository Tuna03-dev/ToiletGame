using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayGameStage1()
    {
        SceneManager.LoadScene(1);
    }
    public void TwoPlayer()
    {
        SceneManager.LoadScene(4);
    }
    public void MapEndLess()
    {
        SceneManager.LoadScene(3);
    }
    public void screatMap()
    {
        SceneManager.LoadScene(5);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
