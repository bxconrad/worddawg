using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBoard : MonoBehaviour {
    public static ToastPosition toastPosition = ToastPosition.BottomCenter;

    [Header("UI")] [SerializeField] private GameObject updateButtons;
    [SerializeField] private GameParameters gameParameters;

    [SerializeField] private ValidatorManager validatorManager;

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private InputWord inputWord;
    [SerializeField] private TransformShaker transformShaker;
    [SerializeField] private Dealer dealer;
    [SerializeField] private LogoImage logoImage;

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
        dealer.Initialize();
        replaceRackButton.Initialize();
        updateButtons.SetActive(MyPrefs.IsShowButtons());
        Toast.Show(
            "To start the game, create a word from the letters in the rack. After that, you can create or modify new words.",
            15f, Color.magenta, toastPosition);
    }

    // Called onButtonClick from wordGrid
    public void LoadSelectedWord(string word) {
        print("UpdateBoard.LoadSelectedWord {" + word + "}\n");
        ClearInputWordButton();
        inputWord.Initialize();
        selectedWord.InitializeTiles(gameParameters.ContractDoubleLetter(word)); // quLogic
    }

    private void RemoveSelectedWord() {
        selectedWord.InitializeTiles(string.Empty);
    }

    // ---------- buttons --------------------------

    // Called from UpdateBoard button and from drag
    public void ReplaceRackButton() {
        print("UpdateBoard.ReplaceRackButton x\n");
        dealer.DealNewRack();
        HandleNearEndGame();

        ClearInputWordButton();
        scoreManager.UpdateScoreForReplaceRack();
    }

    //qulogic


    public void SubmitInputWordButton() {
        //  transformShaker.BeginWaitSpin(logoImage.transform);
        // transformShaker.ASpin(logoImage.transform);
        // return;
        Toast.Dismiss();
        var validationResult = validatorManager.ValidateInputWord(selectedWord, betterRack, inputWord.GetWord());
        if ("TRUE".Equals(validationResult)) {
            scoreManager.UpdateScore(selectedWord.GetWord(), inputWord.GetWord());
            betterRack.RemoveSelectedLetters();
            dealer.Deal();
            HandleNearEndGame();

            wordGrid.UpdateDisplayButton(gameParameters.ExpandDoubleLetter(selectedWord.GetWord()),
                gameParameters.ExpandDoubleLetter(inputWord.GetWord()));
            scrollRect.verticalNormalizedPosition = 1.0f;
            CancelUpdateButton();
        }
        else {
            Toast.Show(validationResult, 2f, Color.red, toastPosition);
            transformShaker.BeginShake(inputWord.transform);
        }
    }


    public void ClearInputWordButton() {
        //print("UpdateBoard.ClearInputWordButton\n");
        inputWord.Initialize();
        selectedWord.ResetStateUnselected();
        betterRack.ResetStateUnselected();
    }

    public void CancelUpdateButton() {
        // print("UpdateBoard.CancelInputWordButton\n");
        ClearInputWordButton();
        RemoveSelectedWord();
    }

    private void HandleNearEndGame() {
        if (dealer.IsNearEndGame()) {
            print("UpdateBoard.IsNearEndGame end game.  " + dealer.GetTotalNumLettersLeft() + "\n");
            replaceRackButton.ChangeForEndGame();
        }
    }


//scoreManager.CalculateWordScore("", "HEARTEN");
//scoreManager.CalculateWordScore("HOOD", "HOODED");
    //  Stragglers 310disjointed 284
    //  scoreManager.CalculateWordScore("", "XXXXXX");
    //        scoreManager.CalculateWordScore("", "HEARTEN");
    //scoreManager.CalculateWordScore("", "BULLDOG");
}