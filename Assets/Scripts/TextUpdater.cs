using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour {
    private TextMeshProUGUI myText;

    private void Awake() {
        myText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetMyText(string text) // Or other method
    {
        myText.SetText(text);
    }
}