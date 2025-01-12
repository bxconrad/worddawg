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
    [SerializeField] private SelectedWord selectedWord;
    [SerializeField] private TransformShaker transformShaker;
    [SerializeField] private Dealer dealer;
    [SerializeField] private LogoImage logoImage;

    [SerializeField] private BetterRack betterRack;
    [SerializeField] private ReplaceRackButton replaceRackButton;
    [SerializeField] private WordGrid wordGrid;

    public void Start() {
        print("UpdateBoard.Start\n");
        // AwakeIt();
    }

    // // Problems with Awake event on Android
    // public void AwakeIt() {
    //     if (!isAwake) {
    //         isAwake = true;
    //         betterRack = GetComponentInChildren<BetterRack>();
    //         wordGrid = GetComponentInChildren<WordGrid>();
    //         replaceRackButton = GetComponentInChildren<ReplaceRackButton>();
    //         print("UpdateBoard.AwakeIt\n");
    //     }
    // }

    public void NewGame() {
        print("UpdateBoard.NewGame selectedWord {" + selectedWord + "} \n");
        wordGrid.Initialize();
        RemoveSelectedWord();
        inputWord.Initialize();
        dealer.Initialize();
        replaceRackButton.Initialize();
        updateButtons.SetActive(MyPrefs.IsShowButtons());
        Toast.Show(
            "To start the game, create a word by clicking on the letters in the rack. After that, you can create or modify new words.",
            15f, Color.magenta, toastPosition);
    }

    // Called onButtonClick from wordGrid
    public void LoadSelectedWord(string word) {
        print("UpdateBoard.LoadSelectedWord {" + word + "}\n");
        selectedWord.gameObject.SetActive(true);
        ClearInputWord();
        inputWord.Initialize();
        selectedWord.InitializeTiles(gameParameters.ContractDoubleLetter(word)); // quLogic
    }

    private void RemoveSelectedWord() {
        selectedWord.InitializeTiles(string.Empty);
        selectedWord.gameObject.SetActive(false);
    }

    // ---------- buttons --------------------------

    // Called from UpdateBoard button and from drag
    public void ReplaceRackButton() {
        print("UpdateBoard.ReplaceRackButton x\n");
        dealer.DealNewRack();
        HandleNearEndGame();

        ClearInputWord();
        scoreManager.UpdateScoreForReplaceRack();
    }

    //qulogic


    public void SubmitInputWordButton() {
        Toast.Dismiss();
        var validationResult = validatorManager.ValidateInputWord(selectedWord, betterRack, inputWord.GetWord());
        if ("TRUE".Equals(validationResult)) {
            selectedWord.gameObject.SetActive(false);
            scoreManager.UpdateScore(selectedWord.GetWord(), inputWord.GetWord());
            betterRack.RemoveSelectedLetters();
            dealer.Deal();
            HandleNearEndGame();

            wordGrid.UpdateDisplayButton(gameParameters.ExpandDoubleLetter(selectedWord.GetWord()),
                gameParameters.ExpandDoubleLetter(inputWord.GetWord()));
            scrollRect.verticalNormalizedPosition = 1.0f;
            ClearInputWord();
            RemoveSelectedWord();
        }
        else {
            Toast.Show(validationResult, 2f, Color.red, toastPosition);
            transformShaker.BeginShake(inputWord.transform);
        }
    }


    public void ClearInputWord() {
        //print("UpdateBoard.ClearInputWordButton\n");
        inputWord.Initialize();
        selectedWord.ResetStateUnselected();
        betterRack.ResetStateUnselected();
    }

    public void CancelUpdateButton() {
        // print("UpdateBoard.CancelInputWordButton\n");
        ClearInputWord();
        print("UpdateBoard.CancelInputWordButton calling deselect \n");
        wordGrid.DeselectButton();
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