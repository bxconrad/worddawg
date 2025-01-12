using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayButton : MonoBehaviour {
    private readonly Color selectedColor = Tile.State.selectedState.fillColor; // new(1f, .5f, 0f);
    private Image image;
    private Color originalColor;
    private TextMeshProUGUI text;

    private void Awake() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponentInChildren<Image>();
        originalColor = image.color;
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

    public void SelectButton() {
        print("DisplayButton.SelectButton\n");
        image.color = selectedColor;
    }

    public void DeSelectButton() {
        print("DisplayButton.DeSelectButton\n");
        image.color = originalColor;
    }
}