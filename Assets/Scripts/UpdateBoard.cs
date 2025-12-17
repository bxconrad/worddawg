using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBoard : MonoBehaviour {
    public static ToastPosition toastPosition = ToastPosition.BottomCenter;

    [Header("UI")] [SerializeField] private GameObject updateButtons;
    [SerializeField] private GameParameters gameParameters;

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private InputWord inputWord;
    [SerializeField] private SelectedWord selectedWord;
    [SerializeField] private GameObject dummyPanel;
    [SerializeField] private TransformShaker transformShaker;
    [SerializeField] private Dealer dealer;
    [SerializeField] private BetterRack betterRack;
    [SerializeField] private ReplaceRackButton replaceRackButton;

    [SerializeField] private WordGrid wordGrid;
    //  private PB3 bot;
    //private BrucesBot brucesBot;

    private List<string> stringList;
    private ValidatorManager validatorManager;

    public void Start() {
        print("UpdateBoard.Start\n");
        validatorManager = new ValidatorManager();
        var dictionaryFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads", "dictionary-EN.txt");
        // bot = new PB3(dictionaryFilePath);
    }

    public void NewGame() {
        print("UpdateBoard.NewGame selectedWord {" + selectedWord + "} \n");
        //bcdo only need to reload dictionary at startup and custom goame. vm should use trie
        validatorManager.Initialize(gameParameters.language);
        wordGrid.Initialize();
        RemoveSelectedWord();
        inputWord.Initialize();
        dealer.Initialize();
        replaceRackButton.Initialize();
        updateButtons.SetActive(MyPrefs.GetIsShowButtons());
        Toast.Show(
            "To start the game, create a word by clicking on the letters in the rack. After that, you can create or modify new words.",
            15f, Color.magenta, toastPosition);
    }

    // Called onButtonClick from wordGrid
    public void LoadSelectedWord(string word) {
        print("UpdateBoard.LoadSelectedWord {" + word + "}\n");
        selectedWord.gameObject.SetActive(true);
        dummyPanel.SetActive(false);
        ClearInputWord();
        inputWord.Initialize();
        selectedWord.InitializeTiles(GameHelper.ContractDoubleLetter(word, gameParameters.language)); // quLogic
        Toast.Dismiss();
    }

    private void RemoveSelectedWord() {
        selectedWord.InitializeTiles(string.Empty);
        selectedWord.gameObject.SetActive(false);
        dummyPanel.SetActive(true);
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

    private void CallPlayerBot() {
        var words = wordGrid.FindWordList();
        var letters = betterRack.GetWord().ToCharArray().ToList();
        // var letters = betterRack.GetWord().ToList();
        // var rackList = betterRack.GetWord().Split(',').ToList();
        // var listOfNames = new List<string>(betterRack.GetWord().Split(','));
        stringList = new List<string>();
        var word = betterRack.GetWord();
        for (var i = 0; i < word.Length; i++) {
            stringList.Add(word.Substring(i, 1));
        } //   words = new List<string> { "cat", "dog", "frog" };

        //  letters = new List<char> { 'r', 's', 'e', 'w', 'a', 'b' };
        var letters2 = new List<string> { "r", "s", "e", "w", "a", "b" };
        var bot = new BrucesBot();
        var newWordCombinations = bot.FindAllWords(words, stringList);
        print("  New words formed: " + newWordCombinations);
    }


    public void SubmitInputWordButton() {
        Toast.Dismiss();
        var validationResult = validatorManager.ValidateInputWord(selectedWord, betterRack, inputWord.GetWord());
        if ("TRUE".Equals(validationResult)) {
            selectedWord.gameObject.SetActive(false);
            dummyPanel.SetActive(true);
            scoreManager.UpdateScore(GameHelper.ExpandDoubleLetter(selectedWord.GetWord(), gameParameters.language),
                GameHelper.ExpandDoubleLetter(inputWord.GetWord(), gameParameters.language));
            betterRack.RemoveSelectedLetters();
            dealer.Deal();
            HandleNearEndGame();

            wordGrid.UpdateDisplayButton(
                GameHelper.ExpandDoubleLetter(selectedWord.GetWord(), gameParameters.language),
                GameHelper.ExpandDoubleLetter(inputWord.GetWord(), gameParameters.language));
            scrollRect.verticalNormalizedPosition = 1.0f;
            ClearInputWord();
            RemoveSelectedWord();
            //CallPlayerBot();
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
        //scoreManager.CalculateWordScore("BLOOM", "BLOOMING");
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