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
    public GameObject rangeIndicatorPrefab;   // Prefab for the range indicator
    public Hourglass hourglass;               // Reference to the Hourglass script

    private bool isPlacing = false;           // Tracks whether placement mode is active
    private List<GameObject> placementMarkers = new List<GameObject>();  // List to store active markers
    private int placedDefenders = 0;          // Counter to track how many defenders have been placed
    public int maxDefenders = 10;             // Maximum number of defenders to be placed

    private GameObject rangeIndicator;        // The instantiated range indicator object

    void Start()
    {
        // Add listener to the place button to activate placement mode when clicked
        defender1Button.onClick.AddListener(ActivatePlacementMode);
        defender1Indicator.enabled = false;  // Hide the indicator initially
    }

    void Update()
    {
        // Update the number of defenders in the scene
        UpdateDefenderCount();

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
        // Only allow placement if less than maxDefenders have been placed
        if (placedDefenders < maxDefenders)
        {
            isPlacing = true;
            defender1Indicator.enabled = true;  // Show the selection indicator
            defender1Button.interactable = false;   // Disable the button during placement

            // Instantiate the range indicator and set its scale based on the defender's attack range
            if (rangeIndicatorPrefab != null)
            {
                rangeIndicator = Instantiate(rangeIndicatorPrefab, Vector3.zero, Quaternion.identity);
                float attackRange = defender1PlacePrefab.GetComponent<DefenderBase>().attackRange;
                float indicatorScale = attackRange * 2f; // Diameter based on the attack range
                rangeIndicator.transform.localScale = new Vector3(indicatorScale, indicatorScale, 1f); // Rotate it appropriately
                rangeIndicator.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Rotate 90 degrees on X-axis
            }

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
        defender1Indicator.transform.position = mousePos + offset;

        // Update the position of the range indicator to follow the mouse cursor in world space
        UpdateRangeIndicatorPosition();
    }

    // This function ensures that the range indicator follows the mouse and stays above the terrain
    void UpdateRangeIndicatorPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the raycast hits anything
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Get the point where the ray hits any surface (terrain or otherwise)
            Vector3 worldPosition = hit.point;

            // Offset the Y position slightly to ensure the indicator stays above the terrain
            worldPosition.y += 0.1f; // Adjust the offset as needed to avoid clipping

            // Set the position of the range indicator
            rangeIndicator.transform.position = worldPosition;
        }
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
                // Check if the raycast hit a valid placement area
                if (Vector3.Distance(hit.point, position) < 0.5f)
                {
                    // Check for collision with other defenders in the position
                    if (!IsCollidingWithDefender(position))
                    {
                        // Adjust the Y position of the defender to be placed 1 unit above the ground
                        Vector3 adjustedPosition = new Vector3(position.x, position.y + 1f, position.z);

                        // Instantiate the defender at the adjusted position
                        GameObject placedDefender = Instantiate(defender1PlacePrefab, adjustedPosition, Quaternion.identity);

                        // If defender is placed, reduce health from the hourglass
                        if (placedDefender.GetComponent<Defender1>() != null)
                        {
                            hourglass.ReduceHealthForDefenderPlacement(30f);
                            Debug.Log("DefenderHealthTaken");
                        }

                        isPlacing = false;
                        defender1Indicator.enabled = false;  // Hide the indicator after placing
                        Destroy(rangeIndicator);             // Destroy the range indicator after placement
                        HidePlacementMarkers();              // Hide the placement markers

                        return;
                    }
                    else
                    {
                        Debug.Log("Cannot place defender here. A defender is already present.");
                    }
                }
            }

            Debug.Log("No valid or available placement position found.");
        }
    }

    // Collision check to see if the position overlaps with an existing defender
    bool IsCollidingWithDefender(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.5f); // Adjust the radius if needed

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Defender"))
            {
                return true;  // Defender already in this position
            }
        }

        return false;  // No defenders in the position
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

    // Dynamically update the number of placed defenders
    void UpdateDefenderCount()
    {
        placedDefenders = FindObjectsOfType<Defender1>().Length;

        // Check if the maximum number of defenders has been placed
        if (placedDefenders >= maxDefenders)
        {
            defender1Button.interactable = false;  // Disable the button when the limit is reached
            Debug.Log("All defender positions are filled.");
        }
        else
        {
            defender1Button.interactable = true;   // Re-enable the button if more placements are allowed
        }
    }
}