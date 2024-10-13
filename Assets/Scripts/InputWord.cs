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

        print("NewInputWord.Awake updateBoard (" + updateBoard + "} tiles (" + tiles + "}\n");
    }


    public void OnDrag(PointerEventData eventData) {
        // print("NewInputWord.OnDrag " + eventData.position.x + "\n");
        if (isFirstDrag) {
            isFirstDrag = false;
            firstPos = eventData.position.x;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        var distance = eventData.position.x - firstPos;
        print("NewInputWord.OnEndDrag pos " + eventData.position.x + " distance " + distance + " firstpos " + firstPos +
              "\n");
        isFirstDrag = true;
        if (distance < -50) {
            if (GetWord().Length > 0) {
                updateBoard.ClearInputWordButton();
            }
            else {
                updateBoard.CancelUpdateButton();
            }
        }
        if (distance > 50) {
            updateBoard.SubmitInputWordButton();
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount == 2) {
            Debug.Log("double click");
            updateBoard.SubmitInputWordButton();
        }
    }

    //bcdo, extra call to this?
    public void Initialize() {
        print("NewInputWord.Initialize tiles " + tiles.Length + "\n");

        for (var i = 0; i < tiles.Length; i++) {
            var tile = tiles[i];
            tile.SetLetter("");
            tile.gameObject.SetActive(false);
            tile.SetState(Tile.State.unselectedState);
        }
    }

    public void AddLetter(BTile tile) {
        var loc = GetWord().Length;
        print("NewInputWord.AddLetter loc " + loc + " word {" + GetWord() + "} \n");
        tiles[loc].gameObject.SetActive(true);
        tiles[loc].SetLetter(tile.letter);
        tiles[loc].SetOriginTile(tile);
    }

    public void RemoveLetter(BTile originTile) {
        print("NewInputWord.RemoveLetter \n");
        originTile.SetState(Tile.State.unselectedState);
        for (var i = 0; i < tiles.Length; i++) {
            var tile = tiles[i];
            if (ReferenceEquals(originTile, tile.originTile)) {
                print("NewInputWord.RemoveLetter ** found ** originTile " + originTile + "\n");
                tile.gameObject.SetActive(false);
                tile.SetOriginTile(null);
                // shift subsequent tiles to the left
                ShiftTilesToLeft(i);
                return;
            }
        }
        print("NewInputWord.RemoveLetter ** notfound ** \n");
    }

    private void ShiftTilesToLeft(int startIndex) {
        print("NewInputWord.ShiftTilesToLeft " + startIndex + " \n");
        for (var i = startIndex; i < tiles.Length - 1; i++) {
            var currentTile = tiles[i];
            var nextTile = tiles[i + 1];
            if (nextTile.GetOriginTile() == null || !nextTile.isActiveAndEnabled) {
                print("NewInputWord.ShiftTilesToLeft return  i " + i + " \n");
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
            if (tile.isActiveAndEnabled) {
                myWord += tile.letter;
            }
        }
        return myWord;
    }
}