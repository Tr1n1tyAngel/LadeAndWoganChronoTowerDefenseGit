using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class MainMenu : MonoBehaviour
    {
        // This function will be called when the "Start Game" button is clicked
        public void StartGame()
        {
            SceneManager.LoadScene("MainGame");
            Debug.Log("Game Started!");
        }

        // This function will be called when the "Quit Game" button is clicked
        public void QuitGame()
        {

            Application.Quit();
            Debug.Log("Game Quit!");
        }
    }

