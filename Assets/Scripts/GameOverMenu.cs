using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void RetryGame()
    {
        Time.timeScale = 1f; // VERY IMPORTANT (unpause game)
        SceneManager.LoadScene("HollowKnight"); // your gameplay scene name
        
    }
}