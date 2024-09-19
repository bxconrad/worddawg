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
        print("ReplaceRackButton.Initialize " + text.text + " " + GetComponent<Image>() + "\n");
        GetComponent<Image>().color = Color.yellow;
        text.text = "Rack";
    }
}