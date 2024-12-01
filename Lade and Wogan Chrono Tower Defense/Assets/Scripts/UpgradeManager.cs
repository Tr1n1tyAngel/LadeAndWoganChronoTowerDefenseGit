using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public Button upgradeButton;
    public GameObject upgradePanel;    // Reference to the Upgrade menu panel
    public Hourglass hourglass;        // Reference to the Hourglass script

    public float defender1HealthUpgradeCost = 100f;
    public float defender1HealthIncrease = 50f;

    public float defender1DamageUpgradeCost = 200f;
    public float defender1DamageIncrease = 20f;

    public float defender2HealthUpgradeCost = 100f;
    public float defender2HealthIncrease = 50f;

    public float defender2SlowEffectUpgradeCost = 200f;
    public float defender2SlowEffectIncrease = 0.1f;

    public float defender3HealthUpgradeCost = 100f;
    public float defender3HealthIncrease = 50f;

    public float defender3SpeedUpgradeCost = 200f;
    public float defender3SpeedIncrease = 0.2f;

    public float hourglassHealthUpgradeCost = 200f;
    public float hourglassHealthIncrease = 100f;

    public float hourglassRangeUpgradeCost = 200f;
    public float hourglassRangeIncrease = 2f;

    public Button hourglassHealthButton;
    public Button hourglassRangeButton;
    public Button defender1HealthButton;
    public Button defender1DamageButton;
    public Button defender2HealthButton;
    public Button defender2SlowEffectButton;
    public Button defender3HealthButton;
    public Button defender3SpeedButton;

    public GameObject defender1Prefab;
    public GameObject defender2Prefab;
    public GameObject defender3Prefab;

    public ProceduralSoundtrack proceduralSoundtrack;

    private bool isMenuOpen = false;   // Tracks whether the upgrade menu is open

    void Start()
    {
        // Ensure the panel is hidden initially
        upgradePanel.SetActive(false);

        // Initialize button text
        upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade";
    }

    // Toggles the Upgrade menu visibility and updates button text
    public void ToggleUpgradeMenu()
    {
        isMenuOpen = !isMenuOpen; // Toggle the menu state
        upgradePanel.SetActive(isMenuOpen); // Show or hide the panel

        // Update button text based on menu state
        if (isMenuOpen)
        {
            upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close Upgrade";
        }
        else
        {
            upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade";
        }
    }
    
    // Performs the upgrade and disables the button
    private void PerformUpgrade(Button button)
    {
        proceduralSoundtrack.PlayUpgradeEffect();
            button.interactable = false;
            button.GetComponent<Image>().color = Color.green; // Indicate upgrade success
        
    }
    public void UpgradeDefender1Health()
    {
        if (hourglass.hourglassHealth >= defender1HealthUpgradeCost)
        {
            PerformUpgrade(defender1HealthButton);
            hourglass.ReduceHealthForDefenderPlacement(defender1HealthUpgradeCost);

            // Update existing instances
            Defender1[] defenders = FindObjectsOfType<Defender1>();
            foreach (Defender1 defender in defenders)
            {
                defender.UpgradeHealth(defender1HealthIncrease);
            }

            // Update prefab for future placements
            defender1Prefab.GetComponent<Defender1>().UpgradeHealth(defender1HealthIncrease);
        }
    }

    public void UpgradeDefender1Damage()
    {
        if (hourglass.hourglassHealth >= defender1DamageUpgradeCost)
        {
            PerformUpgrade(defender1DamageButton);
            hourglass.ReduceHealthForDefenderPlacement(defender1DamageUpgradeCost);

            // Update existing instances
            Defender1[] defenders = FindObjectsOfType<Defender1>();
            foreach (Defender1 defender in defenders)
            {
                defender.UpgradeDamage(defender1DamageIncrease);
            }

            // Update prefab for future placements
            defender1Prefab.GetComponent<Defender1>().UpgradeDamage(defender1DamageIncrease);
        }
    }

    public void UpgradeDefender2Health()
    {
        if (hourglass.hourglassHealth >= defender2HealthUpgradeCost)
        {
            PerformUpgrade(defender2HealthButton);
            hourglass.ReduceHealthForDefenderPlacement(defender2HealthUpgradeCost);

            // Update existing instances
            Defender2[] defenders = FindObjectsOfType<Defender2>();
            foreach (Defender2 defender in defenders)
            {
                defender.UpgradeHealth(defender2HealthIncrease);
            }

            // Update prefab for future placements
            defender2Prefab.GetComponent<Defender2>().UpgradeHealth(defender2HealthIncrease);
        }
    }

    public void UpgradeDefender2SlowEffect()
    {
        if (hourglass.hourglassHealth >= defender2SlowEffectUpgradeCost)
        {
            PerformUpgrade(defender2SlowEffectButton);
            hourglass.ReduceHealthForDefenderPlacement(defender2SlowEffectUpgradeCost);

            // Update existing instances
            Defender2[] defenders = FindObjectsOfType<Defender2>();
            foreach (Defender2 defender in defenders)
            {
                defender.UpgradeSlowEffect(defender2SlowEffectIncrease);
            }

            // Update prefab for future placements
            defender2Prefab.GetComponent<Defender2>().UpgradeSlowEffect(defender2SlowEffectIncrease);
        }
    }

    public void UpgradeDefender3Health()
    {
        if (hourglass.hourglassHealth >= defender3HealthUpgradeCost)
        {
            PerformUpgrade(defender3HealthButton);
            hourglass.ReduceHealthForDefenderPlacement(defender3HealthUpgradeCost);

            // Update existing instances
            Defender3[] defenders = FindObjectsOfType<Defender3>();
            foreach (Defender3 defender in defenders)
            {
                defender.UpgradeHealth(defender3HealthIncrease);
            }

            // Update prefab for future placements
            defender3Prefab.GetComponent<Defender3>().UpgradeHealth(defender3HealthIncrease);
        }
    }

    public void UpgradeDefender3Speed()
    {
        if (hourglass.hourglassHealth >= defender3SpeedUpgradeCost)
        {
            PerformUpgrade(defender3SpeedButton);
            hourglass.ReduceHealthForDefenderPlacement(defender3SpeedUpgradeCost);

            // Update existing instances
            Defender3[] defenders = FindObjectsOfType<Defender3>();
            foreach (Defender3 defender in defenders)
            {
                defender.UpgradeAttackSpeed(defender3SpeedIncrease);
            }

            // Update prefab for future placements
            defender3Prefab.GetComponent<Defender3>().UpgradeAttackSpeed(defender3SpeedIncrease);
        }
    }

    public void UpgradeHourglassRange()
    {
        if (hourglass.hourglassHealth >= hourglassRangeUpgradeCost)
        {
            PerformUpgrade(hourglassRangeButton);
            hourglass.ReduceHealthForDefenderPlacement(hourglassRangeUpgradeCost);
            hourglass.UpgradeRange(hourglassRangeIncrease);
        }
    }
    public void UpgradeHourglassHealth()
    {
        if (hourglass.hourglassHealth >= hourglassHealthUpgradeCost)
        {
            PerformUpgrade(hourglassHealthButton);
            hourglass.ReduceHealthForDefenderPlacement(hourglassHealthUpgradeCost);
            hourglass.UpgradeHealth(hourglassHealthIncrease);
        }
    }
    
}
