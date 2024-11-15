using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameManager : MonoBehaviour
{
    TMP_InputField inputField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();

        inputField.text = Constants.CurrentTeam.Name;

        inputField.onEndEdit.AddListener(SavePlayerName);
    }

    public void SavePlayerName(string name)
    {
        Constants.CurrentTeam.Name = name;
    }
}
