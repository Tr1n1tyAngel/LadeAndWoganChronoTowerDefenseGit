using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenderPlacement : MonoBehaviour
{
    public GameObject defender1PlacePrefab;   // The defender1 that will be placed
    public Button defender1Button;            // Button that allows you to place dfenders
    public Image defenderIndicator;
    public Sprite[] defenderSprites = new Sprite[3];// The image that follows the mouse as a visual indicator for what type of defender is placed

    public GameObject placementMarkerPrefab;  // Prefab for the visual marker that shows where defenders can be placed
    public GameObject rangeIndicatorPrefab;   // Prefab for the range indicator

    public GameObject defender2PlacePrefab;
    public Button defender2Button;

    public GameObject defender3PlacePrefab;
    public Button defender3Button;

    private GameObject selectedDefenderPrefab;

    //reference to other scripts
    public MeshGenerator meshGenerator;       
    public Hourglass hourglass;               

    //information for the placement of defenders
    private bool isPlacing = false;           
    private List<GameObject> placementMarkers = new List<GameObject>();  
    public int placedDefenders = 0;          
    public int maxDefenders = 10;             

    private GameObject rangeIndicator;  // The instantiated range indicator object that shows the range of the defender

    void Start()
    {
        defenderIndicator.enabled = false;
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
    public void ActivatePlacementMode(int defenderType)
    {
        if (placedDefenders < maxDefenders)
        {
            isPlacing = true;
            defenderIndicator.enabled = true;  // Adjust to show specific indicator per defender if needed
            DisablePlacementButtons();
            // Set selected prefab based on defender type
            switch (defenderType)
            {
                case 0:
                    defenderIndicator.sprite = defenderSprites[defenderType];
                    selectedDefenderPrefab = defender1PlacePrefab;
                    defender1Button.interactable = false;
                    if (rangeIndicatorPrefab != null)
                    {
                        rangeIndicator = Instantiate(rangeIndicatorPrefab, Vector3.zero, Quaternion.identity);
                        float attackRange = defender1PlacePrefab.GetComponent<DefenderBase>().attackRange;
                        float indicatorScale = attackRange * 2f;
                        rangeIndicator.transform.localScale = new Vector3(indicatorScale, indicatorScale, 1f);
                        //rotate the object to be in the correct rotation for it to look like the players range
                        rangeIndicator.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    }
                    break;
                case 1:
                    defenderIndicator.sprite = defenderSprites[defenderType];
                    selectedDefenderPrefab = defender2PlacePrefab;
                    defender2Button.interactable = false;
                    if (rangeIndicatorPrefab != null)
                    {
                        rangeIndicator = Instantiate(rangeIndicatorPrefab, Vector3.zero, Quaternion.identity);
                        float attackRange = defender2PlacePrefab.GetComponent<DefenderBase>().attackRange;
                        float indicatorScale = attackRange * 2f;
                        rangeIndicator.transform.localScale = new Vector3(indicatorScale, indicatorScale, 1f);
                        //rotate the object to be in the correct rotation for it to look like the players range
                        rangeIndicator.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    }
                    break;
                case 2:
                    defenderIndicator.sprite = defenderSprites[defenderType];
                    selectedDefenderPrefab = defender3PlacePrefab;
                    defender3Button.interactable = false;
                    if (rangeIndicatorPrefab != null)
                    {
                        rangeIndicator = Instantiate(rangeIndicatorPrefab, Vector3.zero, Quaternion.identity);
                        float attackRange = defender3PlacePrefab.GetComponent<DefenderBase>().attackRange;
                        float indicatorScale = attackRange * 2f;
                        rangeIndicator.transform.localScale = new Vector3(indicatorScale, indicatorScale, 1f);
                        //rotate the object to be in the correct rotation for it to look like the players range
                        rangeIndicator.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    }
                    break;
            }

            // Show placement markers and other indicators as before
            ShowPlacementMarkers();
        }
    }
    // Disable all placement buttons
    private void DisablePlacementButtons()
    {
        defender1Button.interactable = false;
        defender2Button.interactable = false;
        defender3Button.interactable = false;
    }

    // Enable all placement buttons
    private void EnablePlacementButtons()
    {
        defender1Button.interactable = true;
        defender2Button.interactable = true;
        defender3Button.interactable = true;
    }

    // This function updates the position of the selection indicator to follow the mouse in worldspace
    void UpdateSelectionIndicator()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 offset = new Vector3(50f, 50f, 0f);
        defenderIndicator.transform.position = mousePos + offset;
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
    public void PlaceObjectAtPosition()
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
                        GameObject placedDefender = Instantiate(selectedDefenderPrefab, adjustedPosition, Quaternion.identity);

                        // Deduct different costs based on the defender type
                        if (placedDefender.GetComponent<Defender1>() != null)
                        {
                            hourglass.ReduceHealthForDefenderPlacement(30f);
                        }
                        else if (placedDefender.GetComponent<Defender2>() != null)
                        {
                            hourglass.ReduceHealthForDefenderPlacement(50f);  // Higher cost for Defender2
                        }
                        else if (placedDefender.GetComponent<Defender3>() != null)
                        {
                            hourglass.ReduceHealthForDefenderPlacement(150f);  // Adjust cost for Defender3
                        }

                        // Reset placement state
                        isPlacing = false;
                        defenderIndicator.enabled = false;
                        Destroy(rangeIndicator);
                        HidePlacementMarkers();
                        EnablePlacementButtons();
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
            defender2Button.interactable = false;
            defender3Button.interactable = false;
        }
        else
        {
            defender1Button.interactable = true;
            defender2Button.interactable = true;
            defender3Button.interactable = true;
        }
    }
}