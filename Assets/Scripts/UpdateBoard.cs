using System.Collections;
using System.Collections.Generic;
using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBoard : MonoBehaviour {
    public static ToastPosition toastPosition = ToastPosition.BottomCenter;

    [Header("UI")] [SerializeField] private GameObject updateButtons;

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private InputWord inputWord;
    [SerializeField] private SelectedWord selectedWord;
    [SerializeField] private GameObject dummyPanel;
    [SerializeField] private TransformShaker transformShaker;
    [SerializeField] private Dealer dealer;
    [SerializeField] private BetterRack betterRack;
    [SerializeField] private ReplaceRackButton replaceRackButton;
    [SerializeField] private GameParameters gameParameters;

    [SerializeField] private WordGrid wordGrid;
    [SerializeField] private WordGrid wordGrid2;
    [SerializeField] private ScoreGrid scoreGrid1;
    [SerializeField] private ScoreGrid scoreGrid2;
    [SerializeField] private GameObject wordGridScrollPanel2;
    [SerializeField] private GameObject botButton;
    [SerializeField] private CanvasGroup scorePanelCanvasGroup;
    [SerializeField] private GameObject scorePanel;
    private BrucesBot brucesBot;
    private Player currentPlayer;
    private int maxLetters = 1;
    private int playerNumber;
    private Player[] players;
    private int saveBotLevel = -1;
    private string saveLanguage;
    private ScoreCalculator scoreCalculator;
    private List<string> stringList;
    private int turnNumber;
    private ValidatorManager validatorManager;

    public void Start() {
        print("UpdateBoard.Start " + botButton + " " + gameParameters + "\n");
        validatorManager = new ValidatorManager();
        scoreCalculator = new ScoreCalculator(gameParameters);
    }


    public void NewGame(Player player1) {
        //bcdo only need to reload dictionary at startup and custom goame. vm should use trie
        print("UpdateBoard.NewGame botlevel {" + gameParameters.botLevel + "(" + botButton + "}} \n");
        turnNumber = 0;
        BuildDictionaries();

        wordGrid.Initialize();
        //var player1 = new Player(gameParameters.userName, wordGrid, scoreGrid1);
        player1.wordGrid = wordGrid;
        player1.scoreGrid = scoreGrid1;
        print("UpdateBoard.NewGame player {" + player1 + "} \n");

        wordGrid.UpdateGridLayoutConstraint(2);
        if (gameParameters.isTwoPlayer) {
            var player2 = new Player("Bot" + gameParameters.botLevel, wordGrid2, scoreGrid2);
            players = new[] { player1, player2 };
            player2.isBot = true;
            wordGrid.UpdateGridLayoutConstraint(1);
            wordGrid2.Initialize();
        }
        else {
            players = new[] { player1 };
        }

        // scorePanelCanvasGroup.alpha = gameParameters.isTwoPlayer ? 0.0f : 1.0f;
        scorePanel.SetActive(gameParameters.isTwoPlayer);
        wordGridScrollPanel2.SetActive(gameParameters.isTwoPlayer);
        scoreGrid1.gameObject.SetActive(gameParameters.isTwoPlayer);
        scoreGrid2.gameObject.SetActive(gameParameters.isTwoPlayer);

        currentPlayer = player1;
        currentPlayer.Activate(true);
        currentPlayer.Initialize();
        OtherPlayer().Initialize();

        RemoveSelectedWord();
        inputWord.Initialize();
        dealer.Initialize();
        replaceRackButton.Initialize();
        updateButtons.SetActive(MyPrefs.GetIsShowButtons());
        print("UpdateBoard.NewGame done  " + scorePanelCanvasGroup.alpha + "\n");

        Toast.Show(
            "To start the game, create a word by clicking on the letters in the rack. After that, you can create or modify new words.",
            15f, Color.magenta, toastPosition);
    }

    private void BuildDictionaries() {
        if (gameParameters.language.Equals(saveLanguage) && gameParameters.botLevel == saveBotLevel) {
            return;
        }

        var dictionaryName = "dictionary-" + gameParameters.language;
        if (!gameParameters.language.Equals(saveLanguage)) {
            var dictionaryTrie = BuildDictionaryTrie(dictionaryName);
            validatorManager.trieDictionary = dictionaryTrie;
        }

        if (!gameParameters.language.Equals(saveLanguage) || gameParameters.botLevel != saveBotLevel) {
            if (gameParameters.botLevel < 4 && MyPrefs.PREFS_LANG_EN.Equals(gameParameters.language)) {
                dictionaryName = "dictionarySmall-" + gameParameters.language;
            }

            var botDictionaryTrie = BuildDictionaryTrie(dictionaryName);
            brucesBot = new BrucesBot(botDictionaryTrie);
            maxLetters = gameParameters.botLevel;
            if (gameParameters.botLevel == 0) {
                maxLetters = 1;
            }
            else if (gameParameters.botLevel == 4) {
                maxLetters = 99;
            }
        }

        if (saveLanguage == null) {
            var textFile = Resources.Load("dogwords") as TextAsset;
            var dogBonusWords = textFile.text.Split();
            scoreCalculator.dogBonusWords = dogBonusWords;
        }

        print("UpdateBoard.BuildDictionaries maxLetters {" + maxLetters + "  dictionaryName " + dictionaryName + " \n");
        saveLanguage = gameParameters.language;
        saveBotLevel = gameParameters.botLevel;
    }

    private static TrieDictionary BuildDictionaryTrie(string name) {
        print("UpdateBoard.BuildDictionaryTrie {" + name + "}\n");
        var dictionaryTrie = new TrieDictionary();
        var textFile1 = Resources.Load(name) as TextAsset;
        var words = textFile1.text.Split();
        print("UpdateBoard.BuildDictionaryTrie {" + name + " # " + words.Length + "}\n");
        dictionaryTrie.LoadDictionary(words);
        return dictionaryTrie;
    }


    // Called onButtonClick from wordGrid
    public void LoadSelectedWord(Word wordObject) {
        if (wordObject != null) {
            print("UpdateBoard.LoadSelectedWord {" + wordObject.GetCurrentContents() + "}\n");
            selectedWord.gameObject.SetActive(true);
            dummyPanel.SetActive(false);
            wordGrid.DeselectMismatchButton(wordObject);
            wordGrid2.DeselectMismatchButton(wordObject);
            ClearInputWord();
            inputWord.Initialize();
            selectedWord.Initialize(wordObject); // quLogic
            Toast.Dismiss();
        }
    }

    private void RemoveSelectedWord() {
        selectedWord.InitializeTiles(string.Empty);
        selectedWord.gameObject.SetActive(false);
        dummyPanel.SetActive(true);
    }

// ---------- buttons --------------------------

// Called from UpdateBoard button and from drag
    public void ReplaceRackButton() {
        print("UpdateBoard.ReplaceRackButton " + dealer.IsNearEndGame() + "\n");

        var points = 50;
        if (dealer.IsNearEndGame()) {
            points = scoreCalculator.CalculateWordScore(betterRack.GetWord().Trim(), betterRack.GetWord().Trim());
            // Toast.Show("Subtracting " + points + " points for unused rack letters " + betterRack.GetWord(),
            //     5f, Color.magenta, toastPosition);
        }

        currentPlayer.UpdateScoreForReplaceRack(points);

        ClearInputWord();
        dealer.DealNewRack();
        HandleNearEndGame();
    }

    public void EndGame() {
        print("UpdateBoard.EndGame {" + betterRack.GetWord() + "} \n");
    }

    public void SubmitInputWordButton() {
        print("UpdateBoard.SubmitInputWordButton {" + currentPlayer + "} \n");
        Toast.Dismiss();
        var expandedInputString = GameHelper.ExpandDoubleLetter(inputWord.GetWord());
        var validationResult = validatorManager.ValidateInputWord(selectedWord, betterRack, expandedInputString);
        if ("TRUE".Equals(validationResult)) {
            var wordScore = scoreCalculator.CalculateWordScore(GameHelper.ExpandDoubleLetter(selectedWord.GetWord()),
                GameHelper.ExpandDoubleLetter(inputWord.GetWord()));

            UpdateBoardForValidSubmit(expandedInputString, wordScore);
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
        ClearInputWord();
        print("UpdateBoard.CancelInputWordButton calling deselect \n");
        currentPlayer.wordGrid.DeselectButton();
        OtherPlayer().wordGrid.DeselectButton();
        RemoveSelectedWord();
    }

    private void HandleNearEndGame() {
        if (dealer.IsNearEndGame()) {
            print("UpdateBoard.IsNearEndGame end game.  " + dealer.GetTotalNumLettersLeft() + "\n");
            replaceRackButton.ChangeForEndGame();
        }
    }


    private void UpdateBoardForValidSubmit(string expandedInputString, int score) {
        var word = selectedWord.GetWordObject();
        var isNewWord = !selectedWord.gameObject.activeInHierarchy;
        var originalContents = isNewWord ? "" : word.contents;
        score = score > -1
            ? score
            : scoreCalculator.CalculateWordScore(originalContents, expandedInputString);
        // if there is an active selectedWord we are modifying
        var isSamePlayer = currentPlayer.wordGrid.FindDisplayButtonForWordObject(word) != null;

        if (isNewWord) {
            print("UpdateBoard.UpdateBoardForValidSubmit new word\n");
            word = new Word(expandedInputString, currentPlayer, score);
            currentPlayer.AddNewWord(word);
            currentPlayer.wordGrid.InstantiateDisplayButton(word);
        }
        else if (isSamePlayer) {
            currentPlayer.UpdateExistingWord(word, expandedInputString, score);
            currentPlayer.wordGrid.UpdateDisplayButton(word);
        }
        else {
            // It's a steal!
            print("UpdateBoard.UpdateBoardForValidSubmit  STEAL!!! \n");
            currentPlayer.UpdateExistingWord(word, expandedInputString, score);
            currentPlayer.wordGrid.InstantiateDisplayButton(word);

            var button = OtherPlayer().wordGrid.FindDisplayButtonForWordObject(word);
            if (button != null) {
                Destroy(button.gameObject);
            }
            else {
                print("UpdateBoard.UpdateBoardForValidSubmit couldnt find it with other player}\n");
            }
        }

        selectedWord.gameObject.SetActive(false);
        dummyPanel.SetActive(true);
        scoreManager.UpdateScoreText(currentPlayer);
        currentPlayer.UpdateScoreText();
        //if (currentPlayer.isBot) {
        currentPlayer.currentWord.currentWordHistory.isDogBonusWord =
            scoreCalculator.CalculateDogBonusWord(word.contents); // repeat for bot for toast message
        // // }

        scoreManager.ShowToastMessage(currentPlayer);
        betterRack.RemoveSelectedLetters();
        //betterRack.TestRemoveSelectedLetters();
        //betterRack.CountRemoveSelectedLetters();
        //betterRack.AutomateRemoveSelectedLetters();
        dealer.Deal();
        HandleNearEndGame();


        scrollRect.verticalNormalizedPosition = 1.0f;
        ClearInputWord();
        RemoveSelectedWord();

        turnNumber++;
        playerNumber = gameParameters.isTwoPlayer ? turnNumber % 2 : turnNumber % 1;
        currentPlayer = players[playerNumber];
        print("~~UpdateBoard.SubmitInputWordButton {" + playerNumber + "} \n");
        OtherPlayer().Activate(false);
        currentPlayer.Activate(true);
        if (currentPlayer.isBot) {
            CallPlayerBot();
        }
    }

    private Player OtherPlayer() {
        var otherPlayer = players[0];
        if (playerNumber == 0) {
            if (gameParameters.isTwoPlayer) otherPlayer = players[1];
        }

        return otherPlayer;
    }

    public void CallPlayerBot() {
        var words = new List<Word>();
        foreach (var player in players) {
            words.AddRange(player.wordGrid.FindWordObjects());
        }

        var newWordCombinations = brucesBot.GetCombinations(words, betterRack.GetWord(), 3, maxLetters);
        print("~~CallPlayerBot  New words formed: " + newWordCombinations.Count + " rack " + betterRack.GetWord() +
              "\n");
        var bestResultMatch = brucesBot.FindBestWord(newWordCombinations, scoreCalculator, gameParameters.botLevel);

        if (bestResultMatch.GeneratedWord != null) {
            AutomateWordEntry(bestResultMatch.SourceObject, bestResultMatch.GeneratedWord);
        }
    }

    // Call this method to start a pause for a specific duration
    private void AutomateWordEntry(Word word, string contents) {
        print("AutomateWordEntry\n");
        StartCoroutine(AutomationSequence(word, contents));
    }

    private IEnumerator AutomationSequence(Word word, string contents) {
        print("AutomationSequence yield \n");
        yield return new WaitForSeconds(1.5f);
        if (word != null) {
            LoadSelectedWord(word);
            var button = currentPlayer.wordGrid.FindMatchingButton(word);
            if (button != null) {
                button.gameObject.SetActive(true);
                button.SelectButton();
                currentPlayer.wordGrid.selectedButton = button;
            }
            else {
                button = OtherPlayer().wordGrid.FindMatchingButton(word);
                if (button != null) {
                    button.gameObject.SetActive(true);
                    button.SelectButton();
                    OtherPlayer().wordGrid.selectedButton = button;
                }
            }
        }

        yield return new WaitForSeconds(2.0f);

        print("AutomationSequence selectLetters \n");
        for (var i = 0; i < contents.Length; i++) {
            var letter = contents.Substring(i, 1);
            if (!selectedWord.SelectLetter(letter)) {
                betterRack.SelectLetter(letter);
            }

            inputWord.AddLetter(letter, i);
            print("AutomationSequence selectLetter " + letter + " \n");
            yield return new WaitForSeconds(.75f);
        }

        yield return new WaitForSeconds(1.0f);

        UpdateBoardForValidSubmit(contents, -1);
    }
}
// print("UpdateBoard.CancelInputWordButton\n");
//scoreManager.CalculateWordScore("BLOOM", "BLOOMING");
//scoreManager.CalculateWordScore("", "HEARTEN");
//scoreManager.CalculateWordScore("HOOD", "HOODED");
//  Stragglers 310disjointed 284
//  scoreManager.CalculateWordScore("", "XXXXXX");
//        scoreManager.CalculateWordScore("", "HEARTEN");
//scoreManager.CalculateWordScore("", "BULLDOG");