using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameHandler : MonoBehaviour
{
    Button self;
    public List<bool> ready = new List<bool>();

    void Start()
    {
        self = GetComponent<Button>();
        self.interactable = false;
    }

    void OnStartGame()
    {
    }
}
