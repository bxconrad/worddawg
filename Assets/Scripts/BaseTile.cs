using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseTile : MonoBehaviour {
    public Tile.State state;
    private Image image;
    private TextMeshProUGUI text;
    public Button button { get; set; }

    public string letter { get; private set; }

    public BTile originTile { get; set; }

    protected void Awake() {
        print("BaseTile.Awake\n");
        AwakeMe();
    }

    protected void AwakeMe() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        button = GetComponentInChildren<Button>();
    }

    public bool IsSelected() {
        return state != null && Tile.State.SELECTED.Equals(state.name);
    }

    public bool IsUnselected() {
        return state != null && Tile.State.UNSELECTED.Equals(state.name);
    }


    public bool IsEmpty() {
        return "".Equals(letter);
    }

    public void SetState(Tile.State state) {
        // print("BaseTile.SetState  " + state.name + "\n");
        this.state = state;
        image.color = state.fillColor;
    }

    public void SetOriginTile(BTile tile) {
        originTile = tile;
    }

    public BTile GetOriginTile() {
        return originTile;
    }

    public void SetLetter(string inLetter) {
        //print("BaseTile.SetLetter  inLetter {" + inLetter + "}\n");
        letter = inLetter.ToUpper();
        text.text = letter;

        button.onClick.RemoveAllListeners();
        if (!string.IsNullOrEmpty(inLetter)) { // can happen at end of game
            button.onClick.AddListener(() => OnButtonClick(inLetter));
        }
    }

    public abstract void OnButtonClick(string buttonString);
}