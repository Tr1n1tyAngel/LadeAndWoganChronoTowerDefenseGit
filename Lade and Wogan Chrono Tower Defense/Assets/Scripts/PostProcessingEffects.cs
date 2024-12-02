using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingEffects : MonoBehaviour
{
    public Hourglass hourglass; // Reference to the Hourglass script
    public Volume postProcessVolume; // Reference to the Post Process Volume

    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    void Start()
    {
        if (postProcessVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments) == false)
        {
            colorAdjustments = postProcessVolume.profile.Add<ColorAdjustments>();
        }

        if (postProcessVolume.profile.TryGet<Vignette>(out vignette) == false)
        {
            vignette = postProcessVolume.profile.Add<Vignette>();
        }

        // Initial settings
        colorAdjustments.saturation.value = 0; // Full color
        vignette.active = false;              // Disable vignette
    }

    void Update()
    {
        UpdatePostProcessingEffects();
    }

    void UpdatePostProcessingEffects()
    {
        // Enable and adjust red vignette if health is below 200
        if (hourglass.hourglassHealth < 200f)
        {
            vignette.active = true;
            vignette.color.value = Color.red; // Red vignette

            // Calculate vignette intensity based on remaining health below 200
            float lowHealthPercentage = hourglass.hourglassHealth / 200f; // Health percentage out of 200
            vignette.intensity.value = Mathf.Lerp(1f, 0f, lowHealthPercentage); // Intensity increases as health decreases
            vignette.smoothness.value = 0.6f; // Adjust smoothness of the vignette
        }
        else
        {
            vignette.active = false; // Disable vignette if health is 200 or above
        }

        // Adjust saturation based on hourglass health (lower health = less saturation)
        float healthPercentage = hourglass.hourglassHealth / hourglass.maxHourglassHealth;
        colorAdjustments.saturation.value = Mathf.Lerp(-100, 0, healthPercentage); // -100 = black and white, 0 = full color


    }
}
