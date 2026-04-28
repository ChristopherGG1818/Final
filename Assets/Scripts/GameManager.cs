using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject titleScreen;
    public int lives = 3;
    public int health = 5;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Called when you click the Play button
    public void StartGame()
    {
        titleScreen.SetActive(false);
        Debug.Log("Play button clicked!");
    }
}