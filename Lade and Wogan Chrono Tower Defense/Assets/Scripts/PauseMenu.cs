using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

    public class PauseMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false; 
        public GameObject pauseCanvas;           

    private Canvas[] allCanvases;            

    void Start()
    {
        allCanvases = FindObjectsOfType<Canvas>();
    }

    void Update()
        {
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

    public void Resume()
    {
        pauseCanvas.SetActive(false);    
        Time.timeScale = 1f;             
        GameIsPaused = false;            
        SetCanvasesInteractable(true);   
    }

    //when pausing the game we have another function then the regular time stop, this function disables all other canvases from being interactable
    void Pause()
    {
        pauseCanvas.SetActive(true);     
        Time.timeScale = 0f;             
        GameIsPaused = true;             
        SetCanvasesInteractable(false);  // Disable other canvases except for the pause menu
    }

    public void MainMenu()
        {
            Time.timeScale = 1f;             
            SceneManager.LoadScene("MainMenu"); 
        }
    // This is a function that enables or disables whether other canvases are interactable or not
    void SetCanvasesInteractable(bool state)
    {
        foreach (Canvas canvas in allCanvases)
        {
      
            if (canvas.gameObject == pauseCanvas)
            {
                continue;
            }
                
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.interactable = state;
                canvasGroup.blocksRaycasts = state;
            }

            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
            {
                raycaster.enabled = state;
            }
        }
    }
}


