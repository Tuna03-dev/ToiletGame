using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu2Player : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Home()
    {
        Destroy(GameManager2PlayerCom.Instance.gameObject); // Xóa GameManager
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void Restars()
    {
        Destroy(GameManager2PlayerCom.Instance.gameObject); // Xóa GameManager
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Load lại Scene
        Time.timeScale = 1;

    }
}
