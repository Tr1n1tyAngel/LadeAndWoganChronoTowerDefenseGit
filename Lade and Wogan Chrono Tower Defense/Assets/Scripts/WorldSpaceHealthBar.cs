using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField]  private Camera camera;
    
    // Start is called before the first frame update
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        slider.value =  currentHealth / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
