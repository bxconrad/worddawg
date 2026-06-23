using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBoardButtonHandler : MonoBehaviour {
    public TextMeshProUGUI text;
    public UpdateBoardAbstract updateBoard;

    private void Awake() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        //  updateBoard = GetComponentInParent<UpdateBoardAbstract>();
        print($"~UpdateBoardButtonHandler.Awake updateBoard {updateBoard}\n");
    }

    public void ChangeForEndGame() {
        print("~UpdateBoardButtonHandler.ChangeForEndGame " + GetComponent<Image>() + text.text + "\n");
        GetComponent<Image>().color = Color.red;
        text.text = "End";
    }

    public void Initialize() {
        updateBoard = ServiceLocator.instance.updateBoard;

        var myPurple = new Color32(102, 45, 145, 255);
        print("~UpdateBoardButtonHandler.InitializeWord " + text.text + "\n");
        GetComponent<Image>().color = myPurple;
        text.text = "RACK";
    }

    public void OnButtonClick(string buttonString) {
        print("~UpdateBoardButtonHandler.OnButtonClick " + buttonString);
        if ("REPLACE".Equals(buttonString) || "RACK".Equals(buttonString) || "END".Equals(buttonString)) {
            updateBoard.ReplaceRackButton();
        }
        else if ("CANCEL".Equals(buttonString)) {
            updateBoard.CancelUpdateButton();
        }
        else if ("SUBMIT".Equals(buttonString)) {
            updateBoard.SubmitInputWordButton();
        }
        else {
            print("~UpdateBoardButtonHandler.OnButtonClick unknown call " + buttonString);
        }
    }
}