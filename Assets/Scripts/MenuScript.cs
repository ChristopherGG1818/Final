using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void gotoGame()
    {
        SceneManager.LoadScene("HollowKnight");
    }
}