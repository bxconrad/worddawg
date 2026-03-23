using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BetterRack : MonoBehaviour {
    [SerializeField] private UpdateBoard updateBoard;
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private BTile bTilePrefab;


    private BTile[] tiles { get; set; }

    public void AutomateRemoveSelectedLetters() {
        print("BetterRack.AutomateRemoveSelectedLetters\n");
        StartCoroutine(RemoveSelectedLettersAutomationSequence());
    }

    private IEnumerator RemoveSelectedLettersAutomationSequence() {
        print("BetterRack.RemoveSelectedLettersAutomationSequence yield\n");
        //yield return new WaitForSeconds(.5f);

        foreach (var tile in tiles) {
            if (tile.IsSelected()) {
                tile.SetLetter("");
                tile.SetState(Tile.State.unselectedState);
            }
        }

        for (var i = 0; i < tiles.Length; i++)
        for (var j = i + 1; j < tiles.Length; j++) {
            // j is the index of the next tile to the right
            if ("".Equals(tiles[i].letter)) {
                //bcdo compare to isSelected
                tiles[i].SetLetter(tiles[j].letter); // move the letter from the next tile to this tile
                tiles[j].SetLetter("");
                print("BetterRack.RemoveSelectedLettersAutomationSequence yield move letter i " + i + " j" + j + "\n");
                yield return new WaitForSeconds(.2f);
            }
        }
    }

    public void Initialize() {
        print("BetterRack.Initialize " + gameParameters.numRackLetters + " \n");
        var tilesx = GetComponentsInChildren<BTile>();
        foreach (var child in tilesx) {
            //print("WordGrid.BetterRack destroying " + child.gameObject.name + "\n");
            Destroy(child.gameObject);
        }

        // old tiles are still there even though they are destroyed so can't use getComponentsInChildren, need to re-create array
        tiles = new BTile[gameParameters.numRackLetters];
        for (var i = 0; i < gameParameters.numRackLetters; i++) {
            var newTile = InstantiateButton();
            tiles[i] = newTile;
        }
    }


    private BTile InstantiateButton() {
        var newDisplayButton = Instantiate(bTilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newDisplayButton.transform.SetParent(transform, false);
        return newDisplayButton;
        //    print("BetterRack.InstantiateDisplayButton " + newDisplayButton + "\n");
    }

    public void InitializeTilesCoroutine(string word) {
        print("BetterRack.InitializeTilesCoroutine\n");
        StartCoroutine(InitializeTilesAutomated(word));
    }

    private IEnumerator InitializeTilesAutomated(string word) {
        var wordChars = word.ToCharArray();
        print("BetterRack.InitializeTilesAutomated word {" + word + "} #tiles " + tiles.Length + "\n");
        for (var i = 0; i < Mathf.Min(wordChars.Length, tiles.Length); i++) {
            //print("BetterRack.InitializeTilesAutomated i " + i + "\n");
            var isYield = false;
            if (tiles[i].letter == "" || tiles[i].letter == null) {
                isYield = true;
            }

            tiles[i].SetLetter(wordChars[i].ToString());
            tiles[i].SetState(Tile.State.unselectedState);
            if (isYield) {
                print("~~BetterRack.InitializeTilesAutomated i " + i + "\n");
                yield return new WaitForSeconds(.3f);
            }
        }

        print("~~BetterRack.InitializeTilesAutomated done\n");
    }


    public void InitializeTiles(string word) {
        var wordChars = word.ToCharArray();
        //print("BetterRack.InitializeTiles word {" + word + "} #tiles " + tiles.Length + "\n");
        for (var i = 0; i < Mathf.Min(wordChars.Length, tiles.Length); i++) {
            //    print("BetterRack.InitializeTiles i " + i + "\n");
            tiles[i].SetLetter(wordChars[i].ToString());
            tiles[i].SetState(Tile.State.unselectedState);
        }
    }

    public void ClearRack() {
        print("BetterRack.InitializeTiles  #tiles " + tiles.Length + "\n");
        for (var i = 0; i < tiles.Length; i++) {
            //print("BetterRack.InitializeTiles i " + i + "\n");
            tiles[i].SetLetter("");
            tiles[i].SetState(Tile.State.unselectedState);
        }
    }


    public void ResetStateUnselected() {
        //print("BetterRack.ResetStateUnselected");
        foreach (var tile in tiles) {
            tile.SetState(Tile.State.unselectedState);
        }
    }

    public string GetWord() {
        var myWord = "";
        foreach (var tile in tiles) {
            myWord += tile.letter;
        }

        myWord = myWord.PadRight(gameParameters.numRackLetters); // ?? why do we pad?
        return myWord;
    }

    public bool isOneLetterUsed() {
        foreach (var tile in tiles) {
            if (tile.IsSelected())
                return true;
        }

        return false;
    }

    public void SelectLetter(string letter) {
        foreach (var tile in tiles) {
            if (!tile.IsSelected() && tile.letter.Equals(letter)) {
                tile.SelectLetter();
                return;
            }
        }
    }

    public void SetColor(Color color) {
        foreach (var tile in tiles) {
            tile.GetComponent<Image>().color = color;
        }
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

        for (var i = 0; i < tiles.Length; i++)
        for (var j = i + 1; j < tiles.Length; j++) {
            // j is the index of the next tile to the right
            if ("".Equals(tiles[i].letter)) {
                //bcdo compare to isSelected
                tiles[i].SetLetter(tiles[j].letter); // move the letter from the next tile to this tile
                tiles[j].SetLetter(""); // set to selected
            }
        }

        return numRemoved;
    }

    public int CountRemoveSelectedLetters() {
        var numRemoved = 0;
        foreach (var tile in tiles) {
            if (tile.IsSelected()) {
                numRemoved++;
            }
        }

        return numRemoved;
    }

    public IEnumerator AutomateRemoveLetter(BTile tile, string letter) {
        tile.SetLetter(letter); //bcdo may be able to get rid of this 
        yield return new WaitForSeconds(.50f);
    }

    public int TestRemoveSelectedLetters() {
        var numRemoved = 0;
        foreach (var tile in tiles) {
            if (tile.IsSelected()) {
                tile.SetLetter(""); //bcdo may be able to get rid of this 
                AutomateRemoveLetter(tile, "");
                numRemoved++;
            }
        }

        for (var i = 0; i < tiles.Length; i++)
        for (var j = i + 1; j < tiles.Length; j++) {
            // j is the index of the next tile to the right
            if ("".Equals(tiles[i].letter)) {
                //bcdo compare to isSelected
                // tiles[i].SetLetter(tiles[j].letter); // move the letter from the next tile to this tile
                AutomateRemoveLetter(tiles[i], tiles[j].letter);
                tiles[j].SetLetter(""); // set to selected
                AutomateRemoveLetter(tiles[j], "");
            }
        }

        return numRemoved;
    }
}