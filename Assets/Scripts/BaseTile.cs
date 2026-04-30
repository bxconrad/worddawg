using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseTile : MonoBehaviour {
    private Image image;
    private Color originalColor;
    public Tile.State state;
    private TextMeshProUGUI text;
    private Button button { get; set; }

    public string letter { get; private set; }

    public BTile originTile { get; set; }

    protected void Awake() {
        //print("~BaseTile.Awake\n");
        AwakeMe();
    }

    public void SetInteractable(bool isInteractable) {
        button.interactable = isInteractable;
    }

    protected void AwakeMe() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        originalColor = image.color;
        button = GetComponentInChildren<Button>();
    }

    public bool IsSelected() {
        return state != null && Tile.State.SELECTED.Equals(state.name);
    }

    public bool IsUnselected() {
        return state != null && Tile.State.UNSELECTED.Equals(state.name);
    }


    public void SetState(Tile.State state) {
        // print("~BaseTile.SetState  " + state.name + "\n");
        this.state = state;
        image.color = Tile.State.UNSELECTED.Equals(state.name) ? originalColor : state.fillColor;
    }

    public void SetOriginTile(BTile tile) {
        originTile = tile;
    }

    public BTile GetOriginTile() {
        return originTile;
    }

    public void SetLetter(string inLetter) {
        //print("~BaseTile.SetLetter  inLetter {" + inLetter + "}\n");
        letter = inLetter.ToUpper();
        // quLogic
        // this just handles how QU and LL appear. It does not affect validation or how it appears in word list
        if ("Q".Equals(letter))
            text.text = "Qu";
        else if ("*".Equals(letter))
            text.text = "LL";
        else
            text.text = letter;

        button.onClick.RemoveAllListeners();
        if (!string.IsNullOrEmpty(inLetter)) // can happen at end of game
            button.onClick.AddListener(() => OnButtonClick(inLetter));
    }

    public abstract void OnButtonClick(string buttonString);
}