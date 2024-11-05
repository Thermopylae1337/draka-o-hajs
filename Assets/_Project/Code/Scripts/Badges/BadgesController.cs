using UnityEngine;
using UnityEngine.SceneManagement;

public class BadgesController : MonoBehaviour
{
    public void ViewMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
