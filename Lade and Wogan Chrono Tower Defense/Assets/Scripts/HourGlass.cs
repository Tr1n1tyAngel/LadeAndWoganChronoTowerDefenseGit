using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hourglass : MonoBehaviour
{
    //hourglass stats
    public float hourglassHealth = 500;
    public float maxHourglassHealth = 500;
    public float radius = 5f;
    public float damage = 10;
    public float damageInterval = 5f;

    //UI elements used by the hourglass
    public Image healthbar;
    public TextMeshProUGUI healthText;
    public GameObject baseModel;
    public GameObject upgradeModel;

    private void Start()
    {
        StartCoroutine(DamageEnemiesOverTime());
        UpdateHealthUI();
    }

    void Update()
    {

    }


    //Damage is done to enemies within a radius every few seconds
    IEnumerator DamageEnemiesOverTime()
    {
        while (hourglassHealth > 0)
        {

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {

                    BasicEnemy enemy = hitCollider.GetComponent<BasicEnemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                }
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }

    //hourglass takes damage and displays it on the healthbar in visual and text 
    public void TakeDamage(float damage)
    {
        hourglassHealth -= damage;
        healthbar.fillAmount = hourglassHealth / maxHourglassHealth;
        UpdateHealthUI();
        if (hourglassHealth <= 0)
        {
            Die();
        }
    }

    //for every defender that is placed it must reduce a certain amount of the hourglass health
    public void ReduceHealthForDefenderPlacement(float healthCost)
    {
        hourglassHealth -= healthCost;
        healthbar.fillAmount = hourglassHealth / maxHourglassHealth;
        UpdateHealthUI();
        if (hourglassHealth <= 0)
        {
            Die();
        }
    }

    // Method to restore health when an enemy is killed
    public void RestoreHealthForEnemyKill(float healthRestored)
    {
        hourglassHealth += healthRestored;
        healthbar.fillAmount = hourglassHealth / maxHourglassHealth;
        UpdateHealthUI();
    }

    // Update the UI Text with the current health
    private void UpdateHealthUI()
    {
        healthText.text = "" + hourglassHealth;
    }

    //when the hourglass is out of health load the game over scene
    void Die()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("GameOver");
    }

    //shows the hourglass radius only in scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    public void UpgradeRange(float amount)
    {
        radius += amount;
    }

    public void UpgradeHealth(float healthIncrease)
    {
        hourglassHealth += healthIncrease; // Increase current health
        maxHourglassHealth += healthIncrease;


        healthbar.fillAmount = hourglassHealth / maxHourglassHealth; // Update the health bar
        UpdateHealthUI();
    }
    public void ChangeModel()
    {
        baseModel.SetActive(false);
        upgradeModel.SetActive(true);
    }
}
