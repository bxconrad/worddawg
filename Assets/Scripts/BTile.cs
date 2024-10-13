using UnityEngine;

public class BTile : BaseTile {
    private InputWord inputWord;

    private new void Awake() {
        base.Awake();
        inputWord = GameObject.FindGameObjectWithTag("newInputWord").GetComponent<InputWord>();
        //    print("BTile.Awake newInputWord {" + newInputWord + "} inputText " + inputText + "}\n");
    }


    public override void OnButtonClick(string buttonString) {
        print("BTile.OnButtonClick  buttonString {" + buttonString + "} IsUnselected " + state.name +
              " inputword {" + inputWord.GetWord() + "\n");
        if (IsSelected()) {
            inputWord.RemoveLetter(this);
            SetState(Tile.State.unselectedState);
        }
        else {
            SelectLetter();
        }
    }

    public void SelectLetter() {
        if (IsUnselected()) {
            inputWord.AddLetter(this);
            // print("BTile.SelectLetter  inputText " + inputText + " inputText.text {" + inputText.text + "}\n");
            SetState(Tile.State.selectedState);
        }
    }
}