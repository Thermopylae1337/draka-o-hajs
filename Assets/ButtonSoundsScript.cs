using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundsScript : MonoBehaviour
{
    public AudioSource audioButton;
    void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
            });
        }
    }

    private void PlayButtonClickSound()
    {
        audioButton.Play();
    }
}
