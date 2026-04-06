using UnityEngine;
using UnityEngine.UI; // Needed for UI images
using UnityEngine.SceneManagement; // Optional, for restarting or going to Game Over scene

public class PlayerHealth : MonoBehaviour
{
    public Image[] hearts;      // Drag your 4 heart images here in the Inspector
    public int maxHealth = 4;   // Total hearts
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }

    // Call this function when the player takes damage
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHearts();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    // Update heart UI
    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }

    void GameOver()
    {
        // Stop time
        Time.timeScale = 0f;

        // Optional: show game over UI or reload scene
        Debug.Log("Game Over!");
    }
}