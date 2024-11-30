using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingEffects : MonoBehaviour
{
    public Hourglass hourglass; // Reference to the Hourglass script
    public PostProcessVolume postProcessVolume; // Reference to the Post Process Volume

    private ColorGrading colorGrading;
    private Vignette vignette;

    void Start()
    {
        // Retrieve or add Color Grading and Vignette effects to the volume
        if (postProcessVolume.profile.TryGetSettings(out colorGrading) == false)
        {
            colorGrading = postProcessVolume.profile.AddSettings<ColorGrading>();
        }

        if (postProcessVolume.profile.TryGetSettings(out vignette) == false)
        {
            vignette = postProcessVolume.profile.AddSettings<Vignette>();
        }

        // Initial settings
        colorGrading.saturation.value = 0; // Full color
        vignette.enabled.value = false;   // Disable vignette
    }

    void Update()
    {
        UpdatePostProcessingEffects();
    }

    void UpdatePostProcessingEffects()
    {
        // Enable and adjust red vignette if health is below 100
        if (hourglass.hourglassHealth < 200f)
        {
            vignette.enabled.value = true;
            vignette.color.value = Color.red; // Red vignette

            // Calculate vignette intensity based on remaining health below 100
            float lowHealthPercentage = hourglass.hourglassHealth / 200f; // Health percentage out of 100
            vignette.intensity.value = Mathf.Lerp(1f, 0f, lowHealthPercentage); // Intensity increases as health decreases
            vignette.smoothness.value = 0.6f; // Adjust smoothness of the vignette
        }
        else
        {
            vignette.enabled.value = false; // Disable vignette if health is 100 or above
        }
        // Adjust saturation based on hourglass health (lower health = less saturation)
        float healthPercentage = hourglass.hourglassHealth / hourglass.maxHourglassHealth;
        colorGrading.saturation.value = Mathf.Lerp(-100, 0, healthPercentage); // -100 = black and white, 0 = full color

        
    }
}
