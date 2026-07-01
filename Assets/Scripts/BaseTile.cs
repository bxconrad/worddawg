using TMPro;
using UnityEngine;
using UnityEngine.UI;

// note: there are three different tile prefabs probably because i didn't know what i was doing
public abstract class BaseTile : MonoBehaviour {
    // had to do this field as serializedField
    [SerializeField] private CanvasGroup canvasGroup;
    private Image image;
    private Color originalColor;
    public Tile.State state;
    private TextMeshProUGUI text;
    private Button button { get; set; }
    public string letter { get; private set; }
    public BTile originTile { get; private set; }

    protected virtual void Awake() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        originalColor = image.color;
        button = GetComponentInChildren<Button>();
    }

    // using canvasGroup as setting button.interactable changed the color
    public void SetInteractable(bool isInteractable) {
        canvasGroup.blocksRaycasts = isInteractable;
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
        // quLogic this handles how QU and LL appear. It does not affect validation or how it appears in word list
        if ("#".Equals(letter))
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