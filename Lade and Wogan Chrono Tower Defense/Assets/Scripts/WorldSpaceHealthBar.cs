using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField]  private Camera camera;
    
    //takes in the health values of the object the health bar is on and changes the sliders value
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        slider.value =  currentHealth / maxHealth;
    }
    //changes the rotation of the health bar to be facing the camera
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
