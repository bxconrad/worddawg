using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBoard : MonoBehaviour {
    public static ToastPosition toastPosition = ToastPosition.BottomCenter;
    [Header("UI")] [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject updateButtons;
    [SerializeField] private ValidatorManager validatorManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private LetterBag letterBag;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private InputWord inputWord;
    [SerializeField] private TransformShaker transformShaker;

    private BetterRack betterRack;
    private ReplaceRackButton replaceRackButton;
    private SelectedWord selectedWord;
    private WordGrid wordGrid;

    public void Start() {
        print("UpdateBoard.Start\n");
        AwakeIt();
    }

    // Problems with Awake event on Android
    public void AwakeIt() {
        selectedWord = GetComponentInChildren<SelectedWord>();
        betterRack = GetComponentInChildren<BetterRack>();
        wordGrid = GetComponentInChildren<WordGrid>();
        replaceRackButton = GetComponentInChildren<ReplaceRackButton>();
        print("UpdateBoard.AwakeIt\n");
    }

    public void NewGame() {
        print("UpdateBoard.NewGame \n");
        wordGrid.Initialize();
        RemoveSelectedWord();
        inputWord.Initialize();
        letterBag.Initialize();
        var rackLetters = letterBag.DealNewRack();
        betterRack.InitializeTiles(rackLetters);
        replaceRackButton.Initialize();
        updateButtons.SetActive(MyPrefs.IsShowButtons());
        Toast.Show(
            "To start the game, create a word from the letters in the rack. After that, you can create or modify new words.",
            15f, Color.magenta, toastPosition);
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
        inputWord.Initialize();
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
        if (HandleIfEndGame(rackLetters)) return;
        HandleNearEndGame();
        ClearInputWordButton();
        scoreManager.UpdateScoreForReplaceRack();
    }


    public void SubmitInputWordButton() {
        Toast.Dismiss();
        var validationResult = validatorManager.ValidateInputWord(selectedWord, betterRack, inputWord.GetWord());
        if ("TRUE".Equals(validationResult)) {
            scoreManager.UpdateScore(selectedWord.GetWord(), inputWord.GetWord());

            var numRemoved = betterRack.RemoveSelectedLetters();
            // bc could pass rack letters so letterBag could fix distribution
            betterRack.AddLetters(letterBag.Deal(numRemoved));
            HandleNearEndGame();
            print("UpdateBoard.SubmitInputWordButton ~~~ fix distribution again\n");
            var rackLetters = letterBag.FixLetterDistribution(betterRack.GetWord());
            betterRack.InitializeTiles(rackLetters);

            if (HandleIfEndGame(rackLetters)) // bcdo move this up
                return;
            wordGrid.UpdateDisplayButton(selectedWord.GetWord(), inputWord.GetWord());
            scrollRect.verticalNormalizedPosition = 1.0f;
            HandleNearEndGame();
            CancelUpdateButton();
        }
        else {
            Toast.Show(validationResult, 2f, Color.red, toastPosition);
            transformShaker.Begin(inputWord.transform);
        }
    }


    public void ClearInputWordButton() {
        print("UpdateBoard.ClearInputWordButton\n");
        //  scoreManager.CalculateWordScore("", "XXXXXX");
        //        scoreManager.CalculateWordScore("", "HEARTEN");
        //scoreManager.CalculateWordScore("", "BULLDOG");
        // letterBag.FixDoubleDuplicates("EOYEOJH"); //AARRNNE");
        //   letterBag.FixDoubleDuplicates("AABBCDE"); //AARRNNE");

        inputWord.Initialize();
        selectedWord.ResetStateUnselected();
        betterRack.ResetStateUnselected();
    }

    public void CancelUpdateButton() {
        print("UpdateBoard.CancelInputWordButton\n");
        ClearInputWordButton();
        RemoveSelectedWord();
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
            replaceRackButton.ChangeForEndGame();
        }
    }

    private bool IsNearEndGame() {
        return letterBag.GetNumLettersLeft() <= MyPrefs.NUM_RACK_LETTERS;
    }

//scoreManager.CalculateWordScore("", "HEARTEN");
//scoreManager.CalculateWordScore("HOOD", "HOODED");
    //  Stragglers 310disjointed 284

//  letterBag.FixLetterDistribution("BCDFAAA");
// letterBag.FixLetterDistribution("BCDFGHJ");
// letterBag.FixLetterDistribution("AAAAAAA");
// letterBag.FixLetterDistribution("AAAEEEA");
}