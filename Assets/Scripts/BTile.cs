using UnityEngine;

public class BTile : BaseTile {
    private InputWord inputWord;

    public override void OnButtonClick(string buttonString) {
        inputWord = GameObject.FindGameObjectWithTag("inputWord").GetComponent<InputWord>();
        // print("~BTile.OnButtonClick  buttonString {" + buttonString + "} IsUnselected " + state.name +
        //       " inputword {" + inputWord.GetWordText() + "}\n");
        if (IsSelected()) {
            inputWord.RemoveLetter(this);
            SetState(Tile.State.unselectedState);
        }
        else {
            SelectLetter();
        }
    }

    public void SelectLetter() {
        inputWord = GameObject.FindGameObjectWithTag("inputWord").GetComponent<InputWord>();
        if (IsUnselected()) {
            inputWord.AddLetter(this);
            // print("~BTile.SelectLetter  inputText " + inputText + " inputText.text {" + inputText.text + "}\n");
            SetState(Tile.State.selectedState);
        }
    }
}