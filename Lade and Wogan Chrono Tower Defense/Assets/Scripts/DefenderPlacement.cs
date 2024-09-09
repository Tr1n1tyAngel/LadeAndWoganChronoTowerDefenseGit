using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenderPlacement : MonoBehaviour
{
    public GameObject defender1PlacePrefab;   // The defender1 that will be placed
    public Button defender1Button;            // Button that allows you to place dfenders
    public Image defender1Indicator;          // The image that follows the mouse as a visual indicator for what type of defender is placed

    public GameObject placementMarkerPrefab;  // Prefab for the visual marker that shows where defenders can be placed
    public GameObject rangeIndicatorPrefab;   // Prefab for the range indicator

    //reference to other scripts
    public MeshGenerator meshGenerator;       
    public Hourglass hourglass;               

    //information for the placement of defenders
    private bool isPlacing = false;           
    private List<GameObject> placementMarkers = new List<GameObject>();  
    private int placedDefenders = 0;          
    public int maxDefenders = 10;             

    private GameObject rangeIndicator;  // The instantiated range indicator object that shows the range of the defender

    void Start()
    {
        defender1Indicator.enabled = false;  
    }

    void Update()
    {
        UpdateDefenderCount();

        // Update the position of the selection indicator to follow the mouse and place the object if its avaliable
        if (isPlacing)
        {
            UpdateSelectionIndicator();
            if (Input.GetMouseButtonDown(0))
            {
                PlaceObjectAtPosition();
            }
        }
    }

    // This function is called when the player clicks the button to start placing the object
    public void ActivatePlacementMode()
    {
        // Only allows placement if the max amount of defenders hasnt been reached
        if (placedDefenders < maxDefenders)
        {
            isPlacing = true;
            defender1Indicator.enabled = true;
            defender1Button.interactable = false;

            // Instantiate the range indicator and set its scale based on the defender's attack range
            if (rangeIndicatorPrefab != null)
            {
                rangeIndicator = Instantiate(rangeIndicatorPrefab, Vector3.zero, Quaternion.identity);
                float attackRange = defender1PlacePrefab.GetComponent<DefenderBase>().attackRange;
                float indicatorScale = attackRange * 2f;
                rangeIndicator.transform.localScale = new Vector3(indicatorScale, indicatorScale, 1f);
                //rotate the object to be in the correct rotation for it to look like the players range
                rangeIndicator.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }

            // Show placement markers at the valid positions
            ShowPlacementMarkers();
        }
    }

    // This function updates the position of the selection indicator to follow the mouse in worldspace
    void UpdateSelectionIndicator()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 offset = new Vector3(50f, 50f, 0f);
        defender1Indicator.transform.position = mousePos + offset;
        UpdateRangeIndicatorPosition();
    }

    // This function makes it so that the range indicator follows the mouse and stays above the terrain and only where you can place defenders 
    void UpdateRangeIndicatorPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 worldPosition = hit.point;
            worldPosition.y += 0.1f;
            rangeIndicator.transform.position = worldPosition;
        }
    }

    // This function places the object at one of the valid positions when there is a mouse click on that position, it also prevents more than one defender being placed in the same square aswell as not allowing for 
    void PlaceObjectAtPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            foreach (Vector3 position in meshGenerator.defenderPositions)
            {
                if (Vector3.Distance(hit.point, position) < 0.5f)
                {
                    if (!IsCollidingWithDefender(position))
                    {
                        Vector3 adjustedPosition = new Vector3(position.x, position.y + 1f, position.z);
                        GameObject placedDefender = Instantiate(defender1PlacePrefab, adjustedPosition, Quaternion.identity);

                        // If defender is placed, reduce health from the hourglass, this is the main gameplay loop
                        if (placedDefender.GetComponent<Defender1>() != null)
                        {
                            hourglass.ReduceHealthForDefenderPlacement(30f);
                        }
                        // sets the things that should only be around during placement mode back to false/ not visible
                        isPlacing = false;
                        defender1Indicator.enabled = false; 
                        Destroy(rangeIndicator);            
                        HidePlacementMarkers();              

                        return;
                    }
                }
            }
        }
    }

    // Checks for collision with another defender so that there is only one defender per position at a time
    bool IsCollidingWithDefender(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.5f);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Defender"))
            {
                return true;
            }
        }

        return false;
    }

    // Show visual markers for the valid flattened positions defenders can be placed on
    void ShowPlacementMarkers()
    {
        foreach (Vector3 position in meshGenerator.defenderPositions)
        {
            GameObject marker = Instantiate(placementMarkerPrefab, position, Quaternion.identity);
            placementMarkers.Add(marker);
        }
    }

    // Hide all the placement markers after the object is placed
    void HidePlacementMarkers()
    {
        foreach (GameObject marker in placementMarkers)
        {
            Destroy(marker);
        }

        placementMarkers.Clear();
    }

    // Updates the number of defenders based on how many are in the scene, also checks to see if the max amount of defenders has been reached and deactivates the defender placement or reactivates it if thats not the case
    void UpdateDefenderCount()
    {
        placedDefenders = FindObjectsOfType<Defender1>().Length;
        if (placedDefenders >= maxDefenders)
        {
            defender1Button.interactable = false;
        }
        else
        {
            defender1Button.interactable = true;
        }
    }
}