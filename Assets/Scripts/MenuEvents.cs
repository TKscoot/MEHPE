using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEvents : MonoBehaviour
{
    [SerializeField] Animator Ani = null;
    [SerializeField] Canvas MainMenu = null;

    private void Awake()
    {
        Invoke("activatemenu", 2);
    }

    public void Play()
    {
		SceneManager.LoadScene(1);
    }

    public void Leaderboard()
    {
        Ani.SetBool("leaderboardIsActive", true);
        MainMenu.enabled = false;

    }

    public void Credits()
    {
        Ani.SetBool("creditsIsActive", true);
        MainMenu.enabled = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMenuFromLB()
    {
        Ani.SetBool("leaderboardIsActive", false);
        Invoke("activatemenu", 1);
    }

    public void BackToMenuFromCR()
    {
        Ani.SetBool("creditsIsActive", false);
        Invoke("activatemenu", 1);
    }

    public void activatemenu()
    {
        MainMenu.enabled = true;
    }

}
