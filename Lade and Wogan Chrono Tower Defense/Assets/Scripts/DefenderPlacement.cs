using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenderPlacement : MonoBehaviour
{
    public GameObject defender1PlacePrefab;   // The object prefab to be placed
    public Button defender1Button;            // Reference to the UI Button to activate placement
    public Image defender1Indicator;          // The image that follows the mouse as a visual indicator
    public MeshGenerator meshGenerator;       // Reference to the MeshGenerator to access flattened positions
    public GameObject placementMarkerPrefab;  // Prefab for the visual marker (e.g., a transparent sphere)

    private bool isPlacing = false;           // Tracks whether placement mode is active
    private List<GameObject> placementMarkers = new List<GameObject>();  // List to store active markers
    private int placedDefenders = 0;          // Counter to track how many defenders have been placed
    private int maxDefenders = 10;            // Maximum number of defenders to be placed

    void Start()
    {
        // Add listener to the place button to activate placement mode when clicked
        defender1Button.onClick.AddListener(ActivatePlacementMode);
        defender1Indicator.enabled = false;  // Hide the indicator initially
    }

    void Update()
    {
        // Update the position of the selection indicator to follow the mouse
        if (isPlacing)
        {
            UpdateSelectionIndicator();

            // If player clicks and we're in placement mode, attempt to place the object
            if (Input.GetMouseButtonDown(0))
            {
                PlaceObjectAtPosition();
            }
        }
    }

    // This function is called when the player clicks the button to start placing the object
    void ActivatePlacementMode()
    {
        // Only allow placement if less than 10 defenders have been placed
        if (placedDefenders < maxDefenders)
        {
            isPlacing = true;
            defender1Indicator.enabled = true;  // Show the selection indicator
            defender1Button.interactable = false;   // Disable the button during placement

            // Show placement markers at the valid positions
            ShowPlacementMarkers();
        }
    }

    // This function updates the position of the selection indicator to follow the mouse
    void UpdateSelectionIndicator()
    {
        Vector3 mousePos = Input.mousePosition;

        // Adjust the indicator to be slightly to the top-right of the mouse cursor
        Vector3 offset = new Vector3(50f, 50f, 0f);  // You can adjust the offset values as needed

        // Set the position of the indicator to the top-right of the mouse position
        defender1Indicator.transform.position = mousePos + offset;
    }

    // This function places the object at one of the valid positions
    void PlaceObjectAtPosition()
    {
        // Raycast from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            foreach (Vector3 position in meshGenerator.flattenedPositions)
            {
                if (Vector3.Distance(hit.point, position) < 0.5f)
                {
                    // Adjust the Y position of the defender to be placed 1 unit above the ground
                    Vector3 adjustedPosition = new Vector3(position.x, position.y + 1f, position.z);

                    // Instantiate the defender at the adjusted position
                    Instantiate(defender1PlacePrefab, adjustedPosition, Quaternion.identity);

                    isPlacing = false;
                    defender1Indicator.enabled = false;  // Hide the indicator after placing
                    HidePlacementMarkers();               // Hide the placement markers

                    placedDefenders++;  // Increase the placed defenders counter

                    // Check if the maximum number of defenders has been placed
                    if (placedDefenders >= maxDefenders)
                    {
                        defender1Button.interactable = false;  // Disable the button permanently
                        Debug.Log("All defender positions are filled.");
                    }
                    else
                    {
                        defender1Button.interactable = true;   // Re-enable the button if more placements are allowed
                    }

                    return;
                }
            }

            Debug.Log("No valid placement position found.");
        }
    }

    // Show visual markers for the valid flattened positions
    void ShowPlacementMarkers()
    {
        foreach (Vector3 position in meshGenerator.flattenedPositions)
        {
            // Instantiate a marker at each valid position
            GameObject marker = Instantiate(placementMarkerPrefab, position, Quaternion.identity);
            placementMarkers.Add(marker);
        }
    }

    // Hide all the placement markers after the object is placed
    void HidePlacementMarkers()
    {
        foreach (GameObject marker in placementMarkers)
        {
            Destroy(marker);  // Remove the marker from the scene
        }

        placementMarkers.Clear();  // Clear the list of markers
    }
}