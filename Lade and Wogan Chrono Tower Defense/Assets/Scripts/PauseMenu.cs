using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

    public class PauseMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false; // Tracks whether the game is currently paused
        public GameObject pauseMenuUI;           // Reference to the Pause Menu UI panel

    private Canvas[] allCanvases;            // Array to hold references to all canvases in the scene

    void Start()
    {
        // Find all canvases in the scene at the start
        allCanvases = FindObjectsOfType<Canvas>();
    }

    void Update()
        {
            // Check if the player presses the escape key to toggle the pause menu
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

    // Resume the game by disabling the pause menu and setting time scale to 1
    public void Resume()
    {
        pauseMenuUI.SetActive(false);    // Hide the pause menu UI
        Time.timeScale = 1f;             // Resume game time
        GameIsPaused = false;            // Set the game as not paused
        SetCanvasesInteractable(true);   // Re-enable other canvases
        Debug.Log("Game Resumed");
    }

    // Pause the game by enabling the pause menu and setting time scale to 0
    void Pause()
    {
        pauseMenuUI.SetActive(true);     // Show the pause menu UI
        Time.timeScale = 0f;             // Freeze game time
        GameIsPaused = true;             // Set the game as paused
        SetCanvasesInteractable(false);  // Disable other canvases except for the pause menu
        Debug.Log("Game Paused");
    }

    // Load the main menu scene
    public void MainMenu()
        {
            Time.timeScale = 1f;             // Ensure game time is running before changing scenes
            SceneManager.LoadScene("MainMenu"); // Load the main menu scene
            Debug.Log("Returning to Main Menu");
        }
    // Function to enable or disable interaction on all canvases except the pause menu
    void SetCanvasesInteractable(bool state)
    {
        foreach (Canvas canvas in allCanvases)
        {
            // Skip the pause menu canvas
            if (canvas.gameObject == pauseMenuUI)
                continue;

            // Disable interaction with all canvases using CanvasGroup
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.interactable = state;
                canvasGroup.blocksRaycasts = state;
            }

            // Alternatively, disable raycasts using GraphicRaycaster
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
            {
                raycaster.enabled = state;
            }
        }
    }
}


