using TMPro;
using UnityEngine;

public class HandleInputContent : MonoBehaviour {
    public TMP_InputField myField;

    public void DisplayText() {
        var userInput = myField.text;

        Debug.Log("The user typed: " + userInput);
    }
}