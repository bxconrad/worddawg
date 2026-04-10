using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayButton : MonoBehaviour {
    private readonly Color selectedColor = Tile.State.selectedState.fillColor; // new(1f, .5f, 0f);
    private Image image;
    private Color originalColor;
    private TextMeshProUGUI text;
    private Word word { get; set; }
    public Button button { get; set; }

    private void Awake() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponentInChildren<Image>();
        button = GetComponentInChildren<Button>();
        originalColor = image.color;
        //print("DisplayButton.Awake \n");
    }

    public void SetWord(Word inWord) {
        word = inWord;
        text.text = word.GetCurrentContents();
    }

    public override string ToString() {
        return $"{base.ToString()}, {nameof(word)}: {word}";
    }

    public Word GetWord() {
        print("DisplayButton.GetWordText " + word + "\n");
        return word;
    }

    public string GetWordText() {
        print("DisplayButton.GetWordText " + text.text + "\n");
        return word.GetCurrentContents(); // text.text;
    }

    public void SelectButton() {
        print("DisplayButton.SelectButton\n");
        image.color = selectedColor;
    }

    public void DeSelectButton() {
        print("DisplayButton.DeSelectButton\n");
        image.color = originalColor;
    }

    public void SetInteractable(bool isInteractable) {
        button.interactable = isInteractable;
    }
}