using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject titleScreen;

    // Called when you click the Play button
    public void StartGame()
    {
        titleScreen.SetActive(false); // Hides the menu
        // Add other start game logic here
    }

    // Start is called once before the first frame update
    void Start()
    {
        // You can initialize things here if needed
    }

    // Update is called once per frame
    void Update()
    {
        // Game loop logic can go here if needed
    }
}