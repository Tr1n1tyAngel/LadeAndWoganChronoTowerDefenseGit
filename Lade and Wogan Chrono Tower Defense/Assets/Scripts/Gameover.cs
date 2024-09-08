using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameover : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Game Started!");
    }

    // This function will be called when the "Quit Game" button is clicked
    public void QuitGame()
    {

        Application.Quit();
        Debug.Log("Game Quit!");
    }
}
