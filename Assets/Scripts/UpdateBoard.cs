using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBoard : MonoBehaviour {
    [Header("UI")] [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject endGameButton;
    [SerializeField] private GameObject updateButtons;
    [SerializeField] private ValidatorManager validatorManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private LetterBag letterBag;
    [SerializeField] private WordGrid wordGrid;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private NewInputWord newInputWord;
    [SerializeField] private ShakeTransform shakeTransform;
    private BetterRack betterRack;

    private SelectedWord selectedWord;

    public void Start() {
        print("UpdateBoard.Start\n");
        AwakeIt();
    }

    // Problems with Awake event on Android
    public void AwakeIt() {
        selectedWord = GetComponentInChildren<SelectedWord>();
        betterRack = GetComponentInChildren<BetterRack>();
        print("UpdateBoard.AwakeIt\n");
    }

    public void NewGame() {
        print("UpdateBoard.NewGame \n");
        wordGrid.Initialize();
        RemoveSelectedWord();
        newInputWord.Initialize();
        letterBag.Initialize();
        var rackLetters = letterBag.DealNewRack();
        betterRack.InitializeTiles(rackLetters);
        endGameButton.SetActive(false);
        updateButtons.SetActive(MyPrefs.IsShowButtons());
        Toast.Show(
            "To start the game, create a word from the letters in the rack. After that, you can continue to create new words.\n\nBUT for more fun and points, modify the words you created.",
            15f, Color.magenta, ToastPosition.BottomCenter);
    }

    // Called 3 different ways:
    // 1) From clicking on a word on DisplayBoard - sends the string of that word
    // 2) From clicking on a tile in the Rack on DisplayBoard - sends index of button
    // 3) From clicking on the New Word button in DisplayBoard - sends an empty string
    public void LoadSelectedWord(string word) {
        print("UpdateBoard.LoadSelectedWord {" + word + "}\n");
        // When tiles are clicked on DisplayBoard rack, they pass their index location
        // Update same tile in BetterRack to Selected and set InputWord to the letter at that location
        // SelectedWord is blank as no word was passed
        ClearInputWordButton();
        newInputWord.Initialize();
        if (word.Length > 1) {
            selectedWord.InitializeTiles(word);
        }
        else {
            var index = int.Parse(word);
            betterRack.SelectTileAtIndex(index);
        }
    }

    private void RemoveSelectedWord() {
        selectedWord.InitializeTiles(string.Empty);
    }

    // ---------- buttons --------------------------

    // Called from UpdateBoard button and from drag
    public void ReplaceRackButton() {
        print("UpdateBoard.ReplaceRackButton\n");
        var rackLetters = letterBag.DealNewRack();
        print("UpdateBoard.ReplaceRackButton check for end game. wordLength " + rackLetters.Trim().Length + "\n");
        betterRack.InitializeTiles(rackLetters);
        if (HandleIfEndGame(rackLetters)) {
            return;
        }
        HandleNearEndGame();
        ClearInputWordButton();
        scoreManager.UpdateScoreForReplaceRack();
    }

    private bool HandleIfEndGame(string rackLetters) {
        if (rackLetters.Trim().Length == 0) {
            print("UpdateBoard.IsEndGame end game. wordLength " + rackLetters.Trim().Length + "\n");
            gameManager.EndGame();
            return true;
        }
        return false;
    }

    private void HandleNearEndGame() {
        if (IsNearEndGame()) {
            print("UpdateBoard.IsNearEndGame end game.  " + letterBag.GetNumLettersLeft() + "\n");
            updateButtons.SetActive(false);
            endGameButton.SetActive(true);
        }
    }

    public bool IsNearEndGame() {
        return letterBag.GetNumLettersLeft() <= MyPrefs.NUM_RACK_LETTERS;
    }

    public void SubmitInputWordButton() {
        Toast.Dismiss();
        var validationResult = validatorManager.ValidateInputWord(selectedWord, betterRack, newInputWord.GetWord());
        if ("TRUE".Equals(validationResult)) {
            scoreManager.UpdateScore(selectedWord.GetWord(), newInputWord.GetWord());

            var numRemoved = betterRack.RemoveSelectedLetters();
            // bc could pass rack letters so letterBag could fix distribution
            betterRack.AddLetters(letterBag.Deal(numRemoved));
            HandleNearEndGame();
            print("UpdateBoard.SubmitInputWordButton ~~~ fix distro again\n");
            var rackLetters = letterBag.FixLetterDistribution(betterRack.GetWord());
            betterRack.InitializeTiles(rackLetters);

            if (HandleIfEndGame(rackLetters)) {
                return;
            }
            wordGrid.UpdateDisplayButton(selectedWord.GetWord(), newInputWord.GetWord());
            scrollRect.verticalNormalizedPosition = 1.0f;
            HandleNearEndGame();
            MessageToModify(newInputWord.GetWord(), selectedWord.GetWord());
            CancelUpdateButton();
        }
        else {
            Toast.Show(validationResult, 2f, Color.red, ToastPosition.BottomCenter);
            shakeTransform.Begin(newInputWord.transform);
        }
    }

    private void MessageToModify(string word, string originalWord) {
        if (scoreManager.numWords < 3 && scoreManager.numChangedWords == 0) {
            var msg = "See if you can modify " + word + ". Select " + word +
                      " from the list of words. You must use ALL the letters in " + word +
                      " plus at least ONE letter from the rack.";
            Toast.Show(msg, 15, ToastColor.Green, ToastPosition.BottomCenter);
        }
        else if (scoreManager.numChangedWords == 1 && originalWord.Length > 0) {
            var msg = "Congratulations! You turned " + originalWord + " into " + word + ". And you scored " +
                      scoreManager.wordScore + " points.\n\n Well done!";
            Toast.Show(msg, 15, ToastColor.Green, ToastPosition.BottomCenter);
        }
    }

    public void ClearInputWordButton() {
        print("UpdateBoard.ClearInputWordButton\n");
        newInputWord.Initialize();
        selectedWord.ResetStateUnselected();
        betterRack.ResetStateUnselected();
    }

    public void CancelUpdateButton() {
        print("UpdateBoard.CancelInputWordButton\n");
        ClearInputWordButton();
        RemoveSelectedWord();
    }

//scoreManager.CalculateWordScore("", "HEARTEN");
//scoreManager.CalculateWordScore("HOOD", "HOODED");

//  letterBag.FixLetterDistribution("BCDFAAA");
// letterBag.FixLetterDistribution("BCDFGHJ");
// letterBag.FixLetterDistribution("AAAAAAA");
// letterBag.FixLetterDistribution("AAAEEEA");
}