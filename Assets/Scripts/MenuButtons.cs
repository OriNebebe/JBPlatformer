using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuButtons : MonoBehaviour
{
    public int level;
    public void _LoadScene()
    {
        SceneManager.LoadScene("Level_" + level.ToString());
    }
    public void _StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void _RestartGame()
    {
        SceneManager.LoadScene("Level_0");
    }
    public void _BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void _ExitGame()
    {
        Application.Quit();

    }
    public AudioSource AudioPlay;
    public void PlaySound()
    {
        AudioPlay.Play();
    }
}
