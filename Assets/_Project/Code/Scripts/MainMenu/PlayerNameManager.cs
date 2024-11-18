using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    TMP_InputField inputField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();

        inputField.text = Utils.CurrentTeam.Name;

        inputField.onEndEdit.AddListener(SavePlayerName);
    }

    public void SavePlayerName(string name)
    {
        Utils.CurrentTeam.Name = name;
    }
}
