using TMPro;
using UnityEngine;

public class DisplayButton : MonoBehaviour {
    public TextMeshProUGUI text;

    private void Awake() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        //print("DisplayButton.Awake \n");
    }

    public string GetWord() {
        print("DisplayButton.GetWord " + text.text + "\n");
        return text.text;
    }

    public void SetWord(string word) {
        //print("DisplayButton.SetWord " + word + "\n");
        text.text = word;
    }
}