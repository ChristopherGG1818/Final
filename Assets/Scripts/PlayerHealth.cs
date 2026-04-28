using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public Image[] hearts;
    public int maxHealth = 4;

    [SerializeField] private int currentHealth;

    //
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; UpdateHearts(); }
    }

    void Start()
    {
        // FROM GAMEMANAGER if it exists
        if (GameManager.instance != null)
        {
            currentHealth = GameManager.instance.health;
        }
        else
        {
            currentHealth = maxHealth;
        }

        UpdateHearts();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHearts();

        //SAVE TO GAMEMANAGER
        if (GameManager.instance != null)
        {
            GameManager.instance.health = currentHealth;
        }

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentHealth;
        }
    }

    void GameOver()
    {
        Time.timeScale = 1f;

        //handle lives system
        if (GameManager.instance != null)
        {
            GameManager.instance.lives--;

            if (GameManager.instance.lives > 0)
            {
                GameManager.instance.health = maxHealth;

                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().name
                );
            }
            else
            {
                SceneManager.LoadScene("GameOver");
            }
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}