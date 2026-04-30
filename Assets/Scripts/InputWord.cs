using UnityEngine;
using UnityEngine.EventSystems;

public class InputWord : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler {
    private float firstPos;
    private bool isFirstDrag = true;
    private UpdateBoard updateBoard;
    private InputWordTile[] tiles { get; set; }

    public void Awake() {
        tiles = GetComponentsInChildren<InputWordTile>();
        updateBoard = GetComponentInParent<UpdateBoard>();

        print("~InputWord.Awake updateBoard * (" + updateBoard + "} tiles (" + tiles + "}\n");
    }


    public void OnDrag(PointerEventData eventData) {
        // print("~InputWord.OnDrag " + eventData.position.x + "\n");
        if (isFirstDrag) {
            isFirstDrag = false;
            firstPos = eventData.position.x;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        var distance = eventData.position.x - firstPos;
        print("~InputWord.OnEndDrag pos " + eventData.position.x + " distance " + distance + " firstpos " + firstPos +
              "\n");
        isFirstDrag = true;
        if (distance < -50) {
            if (GetWord().Length > 0)
                updateBoard.ClearInputWord();
            else
                updateBoard.CancelUpdateButton();
        }

        if (distance > 50) updateBoard.SubmitInputWordButton();
    }

    public virtual void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount == 2) {
            Debug.Log("InputWord.OnPointerClick double click");
            updateBoard.SubmitInputWordButton();
        }
    }

    //bcdo, extra call to this?
    public void Initialize() {
        //print("~InputWord.Initialize tiles " + tiles.Length + "\n");

        for (var i = 0; i < tiles.Length; i++) {
            var tile = tiles[i];
            tile.SetLetter("");
            tile.gameObject.SetActive(false);
            tile.SetState(Tile.State.unselectedState);
        }
    }

    public void AddLetters(string letters) {
        for (var i = 0; i < tiles.Length; i++) {
            tiles[i].gameObject.SetActive(true);
            tiles[i].SetLetter(letters.Substring(i, 1));
        }
    }

    public void AddLetter(string letter, int index) {
        tiles[index].gameObject.SetActive(true);
        tiles[index].SetLetter(letter);
    }

    public void AddLetter(BTile tile) {
        var loc = GetWord().Length;
        //print("~InputWord.AddLetter loc " + loc + " word {" + GetWordText() + "} \n");
        tiles[loc].gameObject.SetActive(true);
        tiles[loc].SetLetter(tile.letter);
        tiles[loc].SetOriginTile(tile);
    }

    public void RemoveLetter(BTile originTile) {
        //print("~InputWord.RemoveLetter \n");
        originTile.SetState(Tile.State.unselectedState);
        for (var i = 0; i < tiles.Length; i++) {
            var tile = tiles[i];
            if (ReferenceEquals(originTile, tile.originTile)) {
                print("~InputWord.RemoveLetter ** found ** originTile " + originTile + "\n");
                tile.gameObject.SetActive(false);
                tile.SetOriginTile(null);
                // shift subsequent tiles to the left
                ShiftTilesToLeft(i);
                return;
            }
        }

        print("~InputWord.RemoveLetter ** notfound ** \n");
    }

    private void ShiftTilesToLeft(int startIndex) {
        print("~InputWord.ShiftTilesToLeft " + startIndex + " \n");
        for (var i = startIndex; i < tiles.Length - 1; i++) {
            var currentTile = tiles[i];
            var nextTile = tiles[i + 1];
            if (nextTile.GetOriginTile() == null || !nextTile.isActiveAndEnabled) {
                print("~InputWord.ShiftTilesToLeft return  i " + i + " \n");
                return;
            }

            // move the originTile from the next tile to this tile
            currentTile.SetOriginTile(nextTile
                .GetOriginTile());
            currentTile.gameObject.SetActive(true);
            currentTile.SetLetter(nextTile.letter);
            // blank out the next tile
            nextTile.SetOriginTile(null);
            nextTile.gameObject.SetActive(false);
        }
    }

    public string GetWord() {
        var myWord = "";
        foreach (var tile in tiles) {
            if (tile.isActiveAndEnabled) myWord += tile.letter;
        }

        return myWord;
    }
}