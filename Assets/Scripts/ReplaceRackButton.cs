using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceRackButton : MonoBehaviour {
    public TextMeshProUGUI text;

    private void Awake() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        //print("ReplaceRackButton.Awake \n");
    }

    public void ChangeForEndGame() {
        print("ReplaceRackButton.ChangeForEndGame " + GetComponent<Image>() + text.text + "\n");
        GetComponent<Image>().color = Color.red;
        text.text = "End";
    }

    public void Initialize() {
        //102,45,145
        Color myPurple;
        var my = new Color32(102, 45, 145, 255);
        print("ReplaceRackButton.Initialize " + text.text + "\n");
        GetComponent<Image>().color = my;
        text.text = "RACK";
    }
}