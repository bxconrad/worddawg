using UnityEngine;

public class BTile : BaseTile {
    // private TextMeshProUGUI inputText;
    private NewInputWord newInputWord;

    private new void Awake() {
        base.Awake();
        // inputText = GameObject.FindGameObjectWithTag("inputText").GetComponent<TextMeshProUGUI>();
        newInputWord = GameObject.FindGameObjectWithTag("newInputWord").GetComponent<NewInputWord>();
//        print("BTile.Awake newInputWord {" + newInputWord + "} inputText " + inputText + "}\n");
    }


    public override void OnButtonClick(string buttonString) {
        print("BTile.OnButtonClick  buttonString {" + buttonString + "} IsUnselected " + state.name +
              " inputword {" + newInputWord.GetWord() + "\n");
        if (IsSelected()) {
            newInputWord.RemoveLetter(this);
            SetState(Tile.State.unselectedState);
        }
        else {
            SelectLetter();
        }
    }

    public void SelectLetter() {
        if (IsUnselected()) {
            //inputText.text += letter;
            newInputWord.AddLetter(this);
            // print("BTile.SelectLetter  inputText " + inputText + " inputText.text {" + inputText.text + "}\n");
            SetState(Tile.State.selectedState);
        }
    }
}