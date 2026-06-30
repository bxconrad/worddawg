using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedWord : MonoBehaviour, IDragHandler, IEndDragHandler {
    private float firstPos;
    private bool isFirstDrag = true;
    private UpdateBoardAbstract updateBoard;
    private Word wordObject { get; set; }
    private BTile[] tiles { get; set; }

    public void Awake() {
        tiles = GetComponentsInChildren<BTile>();
        print($"~SelectedWord.Awake tiles {tiles.Length} \n");
    }

    public void Start() {
        //   updateBoard = GetComponentInParent<UpdateBoardAbstract>();
        print($"~SelectedWord.Start updateBoard  {updateBoard}\n");
    }

    // bcdo for some reason doesn't span entire width, it has to be in an active tile ??
    public void OnDrag(PointerEventData eventData) {
        // print("~SelectedWord.OnDrag " + eventData.position.x + "\n");
        if (isFirstDrag) {
            isFirstDrag = false;
            firstPos = eventData.position.x;
            //   print("~SelectedWord.OnDrag setting " + eventData.position.x + isFirstDrag + "\n");
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        var distance = eventData.position.x - firstPos;
        isFirstDrag = true;
        print("~SelectedWord.OnEndDrag " + distance + "\n");
        if (distance > 50) {
            SelectAllLetters();
        }
        else if (distance < -50) {
            if (HasSelectedLetter())
                updateBoard.ClearInputWord();
            else
                updateBoard.CancelUpdateButton();
        }
    }

    public Word GetWordObject() {
        return wordObject;
    }

    public void SetWordObject(Word word) {
        wordObject = word;
    }

    private bool HasSelectedLetter() {
        print("~SelectedWord.SelectAllLetters after drag\n");
        foreach (var tile in tiles) {
            if (tile.IsSelected() && tile.isActiveAndEnabled) return true;
        }

        return false;
    }

    private void SelectAllLetters() {
        print("~SelectedWord.SelectAllLetters after drag\n");
        foreach (var tile in tiles) {
            tile.SelectLetter();
        }
    }

    public void Initialize() {
        updateBoard = ServiceLocator.instance.updateBoard;
        InitializeTiles(string.Empty);
    }

    public void InitializeWord(Word inwordObject) {
        wordObject = inwordObject;
        // Tile willo display Qu for #. 
        InitializeTiles(wordObject.GetCurrentContents());
    }

    public void InitializeTiles(string word) {
        print("~SelectedWord.InitializeTiles tiles " + tiles.Length + " word {" + word + "}\n");
        var wordChars = word.ToCharArray();

        for (var i = 0; i < tiles.Length; i++) {
            var tile = tiles[i];
            if (i < wordChars.Length) {
                tile.gameObject.SetActive(true);
                tile.SetLetter(wordChars[i].ToString());
                tile.SetState(Tile.State.unselectedState);
            }
            else {
                tile.SetLetter("");
                tile.gameObject.SetActive(false);
                tile.SetState(Tile.State.selectedState);
            }
        }
    }

    public void ResetStateUnselected() {
        //print("~SelectedWord.ResetStateUnselected \n");

        foreach (var tile in tiles) {
            if ("".Equals(tile.letter)) return;
            tile.SetState(Tile.State.unselectedState);
        }
    }

    public string GetWord() {
        var myWord = "";
        for (var i = 0; i < tiles.Length; i++) {
            myWord += tiles[i].letter;
        }

        return myWord;
    }

    public bool SelectLetter(string letter) {
        foreach (var tile in tiles) {
            if (!tile.IsSelected() && tile.letter.Equals(letter)) {
                tile.SelectLetter();
                return true;
            }
        }

        return false;
    }

    public bool isAllLettersUsed() {
        for (var i = 0; i < tiles.Length; i++) {
            if (tiles[i].IsUnselected()) {
                print("~SelectedWord.isAllLettersUsed rtn false i=" + i + " letter " + tiles[i].state.name + "\n");
                return false;
            }
        }

        return true;
    }
}