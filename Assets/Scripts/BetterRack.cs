using UnityEngine;
using UnityEngine.EventSystems;

public class BetterRack : MonoBehaviour, IDragHandler, IEndDragHandler {
    [SerializeField] private UpdateBoard updateBoard;

    private float firstPos;
    private bool isFirstDrag = true;
    private BTile[] tiles { get; set; }

    private void Awake() {
        //print("BetterRack.Awake \n");
        tiles = GetComponentsInChildren<BTile>();
        //InitializeTilesOrigin();
        print("BetterRack.Awake tiles" + tiles + "\n");
    }

    public void OnDrag(PointerEventData eventData) {
        //print("BetterRack.OnDrag " + eventData.position.x + "\n");
        if (isFirstDrag) {
            isFirstDrag = false;
            firstPos = eventData.position.x;
            print("BetterRack.OnDrag setting " + eventData.position.x + isFirstDrag + "\n");
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        var distance = eventData.position.x - firstPos;
        print("BetterRack.OnEndDrag pos " + eventData.position.x + " distance " + distance + " firstpos " + firstPos +
              "\n");
        isFirstDrag = true;
        if (Mathf.Abs(distance) > 100) {
            updateBoard.ReplaceRackButton();
        }
    }

    // this only needs to happen once as we reuse the same tiles and origin and index does not change
    // private void InitializeTilesOrigin() {
    //     print("BetterRack.InitializeTilesOrigin\n");
    //     for (var i = 0; i < tiles.Length; i++) {
    //         tiles[i].SetOrigin((BaseTile.ORIGIN_RACK, i));
    //     }
    // }

    public void InitializeTiles(string word) {
        var wordChars = word.ToCharArray();
        print("BetterRack.InitializeTiles word {" + word + "} #tiles " + tiles.Length + "\n");
        for (var i = 0; i < wordChars.Length; i++) {
            //print("BetterRack.InitializeTiles i " + i + "\n");
            tiles[i].SetLetter(wordChars[i].ToString());
            tiles[i].SetState(Tile.State.unselectedState);
            //  tiles[i].SetOrigin((BaseTile.ORIGIN_RACK, i));
        }
    }


    public void ResetStateUnselected() {
        //print("BetterRack.ResetStateUnselected");
        foreach (var tile in tiles) {
            tile.SetState(Tile.State.unselectedState);
        }
    }

    public void SelectTileAtIndex(int index) {
        tiles[index].SetState(Tile.State.selectedState);
    }


    public string GetWord() {
        var myWord = "";
        foreach (var tile in tiles) {
            myWord += tile.letter;
        }

        myWord = myWord.PadRight(MyPrefs.NUM_RACK_LETTERS); // ?? why do we pad?
        return myWord;
    }

    public int GetTrimmedWordLength() {
        return GetWord().Trim().Length;
    }

    public bool isOneLetterUsed() {
        foreach (var tile in tiles) {
            if (tile.IsSelected()) {
                return true;
            }
        }

        return false;
    }

    // called on Submit to determine how many new letters are needed
    // and shifting letters to the left
    public int RemoveSelectedLetters() {
        var numRemoved = 0;
        foreach (var tile in tiles) {
            if (tile.IsSelected()) {
                tile.SetLetter(""); //bcdo may be able to get rid of this 
                numRemoved++;
            }
        }

        for (var i = 0; i < tiles.Length; i++) {
            for (var j = i + 1; j < tiles.Length; j++) {
                // j is the index of the next tile to the right
                if ("".Equals(tiles[i].letter)) {
                    //bcdo compare to isSelected
                    tiles[i].SetLetter(tiles[j].letter); // move the letter from the next tile to this tile
                    tiles[j].SetLetter(""); // set to selected
                }
            }
        }

        return numRemoved;
    }

    public void AddLetters(string letters) {
        var numRemoved = 0;
        foreach (var tile in tiles) {
            if (tile.IsEmpty()) {
                tile.SetLetter(letters.Substring(numRemoved, 1));
                numRemoved++;
            }
        }
    }

    public bool IsLetterAvailable(string letter) {
        foreach (var t in tiles) {
            if (t.IsUnselected() && letter.Equals(t.letter)) {
                return true;
            }
        }

        return false;
    }
}