using System;
using System.Collections;
using System.Collections.Generic;
using bot;
using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBoard : MonoBehaviour {
    [Header("UI")] [SerializeField] private GameObject updateButtons;

    [SerializeField] private ToastMaster toastMaster;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private InputWord inputWord;
    [SerializeField] private SelectedWord selectedWord;
    [SerializeField] private GameObject dummyPanel;
    [SerializeField] private GameObject logoImage2;
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
    private BrucesBotAbstract brucesBot;

    private Player currentPlayer;
    private bool isEndGame;

    private int playerNumber;
    private Player[] players;
    private int saveBotLevel = -1;
    private string saveLanguage;
    private ScoreCalculator scoreCalculator;
    private List<string> stringList;
    private int turnNumber;
    private ValidatorManager validatorManager;

    public void Start() {
        print($"~UpdateBoard.Start {botButton} {gameParameters}\n");
        validatorManager = new ValidatorManager();
        scoreCalculator = new ScoreCalculator(gameParameters);
    }


    public void NewGame() {
        //bcdo only need to reload dictionary at startup and custom goame. 
        print($"~UpdateBoard.NewGame botlevel {gameParameters.botLevel} \n");
        isEndGame = false;
        BuildDictionaries();

        ActivatePanels();
        UpdateButtonsInteractable(true);
        if (gameParameters.isTwoPlayer) {
            var player1 = new Player(MyPrefs.BOT_NAMES[gameParameters.botLevel], wordGrid, scoreGrid1);
            var player2 = new Player(gameParameters.userName, wordGrid2, scoreGrid2);
            players = new[] { player1, player2 };
            player1.isBot = true;
            wordGrid2.Initialize();
            var msg = player1.name + " goes first. Then it's your turn" + player2.name;
            Toast.Show(msg, 15f, Color.magenta, GameHelper.GetToastPosition());
        }
        else {
            var player1 = new Player(gameParameters.userName, wordGrid, scoreGrid1);
            players = new[] { player1 };
            Toast.Show(
                "To start the game, create a word by clicking on the letters in the rack. After that, you can create or modify new words.",
                15f, Color.magenta, GameHelper.GetToastPosition());
        }

        currentPlayer = players[0];
        currentPlayer.Activate(true);

        RemoveSelectedWord();
        wordGrid.Initialize();
        inputWord.Initialize();
        dealer.Initialize();
        replaceRackButton.Initialize();
        updateButtons.SetActive(MyPrefs.GetIsShowButtons());
        print($"~UpdateBoard.NewGame done  {scorePanelCanvasGroup.alpha}\n");

        turnNumber = -1;
        NextTurn();
    }

    private void ActivatePanels() {
        var isActive = gameParameters.isTwoPlayer;
        scorePanel.SetActive(isActive);
        logoImage2.SetActive(isActive);
        wordGridScrollPanel2.SetActive(isActive);
        wordGrid.UpdateGridLayoutConstraint(isActive ? 1 : 2);
    }

    private void BuildDictionaries() {
        //bcdo refactor to dictionaryHandler?
        if (gameParameters.language.Equals(saveLanguage) && gameParameters.botLevel == saveBotLevel) {
            return;
        }

        var dictionaryName = "dictionary-" + gameParameters.language;
        if (!gameParameters.language.Equals(saveLanguage)) {
            var dictionaryTrie = BuildDictionaryTrie(dictionaryName);
            validatorManager.trieDictionary = dictionaryTrie;
        }

        var botDictionaryName = "dictionary-" + gameParameters.language;
        if (!gameParameters.language.Equals(saveLanguage) || gameParameters.botLevel != saveBotLevel) {
            if (gameParameters.botLevel <= 1 && MyPrefs.PREFS_LANG_EN.Equals(gameParameters.language)) {
                botDictionaryName = "dictionaryTiny-" + gameParameters.language;
            }
            else if (gameParameters.botLevel < 4 && MyPrefs.PREFS_LANG_EN.Equals(gameParameters.language)) {
                botDictionaryName = "dictionarySmall-" + gameParameters.language;
            }

            var botDictionaryTrie = BuildDictionaryTrie(botDictionaryName);
            brucesBot = BotFactory.Create(gameParameters.botLevel, botDictionaryTrie, scoreCalculator);
        }

        if (saveLanguage == null) {
            var textFile = Resources.Load("dogwords") as TextAsset;
            var dogBonusWords = textFile.text.Split();
            scoreCalculator.dogBonusWords = dogBonusWords;
        }

        print(
            $"~UpdateBoard.BuildDictionaries dictionaryName {dictionaryName} botDictionaryName {botDictionaryName} \n");
        saveLanguage = gameParameters.language;
        saveBotLevel = gameParameters.botLevel;
    }

    private static TrieDictionary BuildDictionaryTrie(string name) {
        print($"~UpdateBoard.BuildDictionaryTrie {name}\n");
        var dictionaryTrie = new TrieDictionary();
        var textFile1 = Resources.Load(name) as TextAsset;
        var words = textFile1.text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        print($"~UpdateBoard.BuildDictionaryTrie {name} # {words.Length}\n");
        dictionaryTrie.LoadDictionary(words);
        return dictionaryTrie;
    }


    // Called onButtonClick from wordGrid
    public void LoadSelectedWord(Word wordObject) {
        if (wordObject != null) {
            print($"~UpdateBoard.LoadSelectedWord {wordObject.GetCurrentContents()}\n");
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

    // Called from UpdateBoard button
    public void ReplaceRackButton() {
        print($"~UpdateBoard.ReplaceRackButton {dealer.IsNearEndGame()}\n");

        var points = 50;
        if (dealer.IsNearEndGame()) {
            points = scoreCalculator.CalculateWordScore(betterRack.GetWord().Trim(), betterRack.GetWord().Trim());
        }
        else {
            var msg =
                $"{currentPlayer.name} could not make a word and has replaced the rack. There is a penalty of 50 points and loss of turn.";
            Toast.Show(msg, 8f, Color.red, GameHelper.GetToastPosition());
        }

        currentPlayer.UpdateScoreForReplaceRack(points);
        toastMaster.UpdateScoreText(currentPlayer);
        print($"~UpdateBoard.ReplaceRackButton   currentPlayer {currentPlayer}\n");

        ClearInputWord();
        dealer.DealNewRack();
        HandleNearEndGame();
        NextTurn();
    }

    public void EndGame() {
        isEndGame = true;
        print($"~UpdateBoard.EndGame {betterRack.GetWord()} \n");
    }

    public void SubmitInputWordButton() {
        print($"~UpdateBoard.SubmitInputWordButton {currentPlayer.name} inputWord {inputWord.GetWord()} \n");
        Toast.Dismiss();

        var expandedInputString = GameHelper.ExpandDoubleLetter(inputWord.GetWord());
        var validationResult = validatorManager.ValidateInputWord(selectedWord, betterRack, expandedInputString);
        if ("TRUE".Equals(validationResult)) {
            UpdateBoardForValidSubmit(expandedInputString);
        }
        else {
            Toast.Show(validationResult, 2f, Color.red, GameHelper.GetToastPosition());
            transformShaker.BeginShake(inputWord.transform);
        }
    }


    public void ClearInputWord() {
        //print("~UpdateBoard.ClearInputWordButton\n");
        inputWord.Initialize();
        selectedWord.ResetStateUnselected();
        betterRack.ResetStateUnselected();
    }

    public void CancelUpdateButton() {
        ClearInputWord();
        print("~UpdateBoard.CancelInputWordButton calling deselect \n");
        currentPlayer.wordGrid.DeselectButton();
        OtherPlayer().wordGrid.DeselectButton();
        RemoveSelectedWord();
    }

    private void HandleNearEndGame() {
        if (dealer.IsNearEndGame()) {
            print($"~UpdateBoard.HandleNearEndGame true  {dealer.GetTotalNumLettersLeft()}\n");
            replaceRackButton.ChangeForEndGame();
        }
    }

    private void UpdateBoardForValidSubmit(string expandedInputString) {
        var word = selectedWord.GetWordObject();
        var isNewWord = !selectedWord.gameObject.activeInHierarchy;
        var isSamePlayer = currentPlayer.wordGrid.FindDisplayButtonForWordObject(word) != null;
        var isSteal = !isNewWord && !isSamePlayer && !currentPlayer.isBot;
        var originalContents = isNewWord ? "" : word.contents;
        print(
            $"~~UpdateBoard.UpdateBoardForValidSubmit isSteal {isSteal} isNewWord {isNewWord} isSamePlayer {isSamePlayer}\n");
        var score = scoreCalculator.CalculateWordScore(originalContents, expandedInputString, isSteal);
        // if there is an active selectedWord we are modifying

        if (isNewWord) {
            print("~UpdateBoard.UpdateBoardForValidSubmit new word\n");
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
            print("~UpdateBoard.UpdateBoardForValidSubmit  STEAL!!! \n");
            currentPlayer.UpdateExistingWord(word, expandedInputString, score);
            currentPlayer.wordGrid.InstantiateDisplayButton(word);

            var button = OtherPlayer().wordGrid.FindDisplayButtonForWordObject(word);
            if (button != null) {
                Destroy(button.gameObject);
            }
            else {
                print("~UpdateBoard.UpdateBoardForValidSubmit couldnt find button for other player\n");
            }
        }

        selectedWord.gameObject.SetActive(false);
        dummyPanel.SetActive(true);
        toastMaster.UpdateScoreText(currentPlayer);
        currentPlayer.UpdateScoreText();
        currentPlayer.currentWord.currentWordHistory.isDogBonusWord =
            scoreCalculator.CalculateDogBonusWord(word.contents); // repeat for bot for toast message

        toastMaster.ShowToastMessage(currentPlayer);
        betterRack.RemoveSelectedLetters();

        scrollRect.verticalNormalizedPosition = 1.0f;
        ClearInputWord();
        RemoveSelectedWord();

        dealer.Deal();
        HandleNearEndGame();
        NextTurn();
    }

    private void NextTurn() {
        if (isEndGame) return;
        turnNumber++;
        playerNumber = gameParameters.isTwoPlayer ? turnNumber % 2 : turnNumber % 1;
        currentPlayer = players[playerNumber];
        print($"~UpdateBoard.NextTurn playerNumber {playerNumber}  name {currentPlayer.name} \n");
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

    private void CallPlayerBot() {
        print("~UpdateBoard.CallPlayerBot begin \n");
        var words = new List<Word>();
        foreach (var player in players) {
            words.AddRange(player.wordGrid.FindWordObjects());
        }

        var bestResultMatch = brucesBot.FindBestestWord(words, betterRack.GetWord());
        print($"~~~UpdateBoard.CallPlayerBot bestResultMatch {bestResultMatch}\n");

        if (!string.IsNullOrEmpty(bestResultMatch.GeneratedWord)) {
            //bcdo fix qu
            AutomateWordEntry(bestResultMatch.SourceObject, bestResultMatch.GeneratedWord);
        }
        else {
            print($"~UpdateBoard.CallPlayerBot  No word Found. Rack {betterRack.GetWord()} call ReplaceRack \n");
            AutomatedReplaceRack(3.0f);
        }
    }

    // Call this method to start a pause for a specific duration
    private void AutomatedReplaceRack(float time) {
        print("~UpdateBoard.AutomatedReplaceRack\n");
        StartCoroutine(AutomatedReplaceRackCoroutine(time));
    }

    private IEnumerator AutomatedReplaceRackCoroutine(float time) {
        print("~UpdateBoard.AutomatedReplaceRackCoroutine\n");
        yield return new WaitForSeconds(time);
        ReplaceRackButton();
        NextTurn();
    }

    private void AutomateWordEntry(Word word, string contents) {
        print("~UpdateBoard.AutomateWordEntry\n");
        StartCoroutine(AutomateWordEntryCoroutine(word, contents));
    }

    private IEnumerator AutomateWordEntryCoroutine(Word word, string contents) {
        print($"~UpdateBoard.AutomateWordEntryCoroutine yield turnNumber {turnNumber}\n");
        betterRack.SetInteractable(false);
        wordGrid.SetInteractable(false);
        wordGrid2.SetInteractable(false);
        UpdateButtonsInteractable(false);
        yield return new WaitForSeconds(2.0f);
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

        yield return new WaitForSeconds(1.0f);

        print("~UpdateBoard.AutomateWordEntryCoroutine selectLetters \n");
        for (var i = 0; i < contents.Length; i++) {
            var letter = contents.Substring(i, 1);
            if (!selectedWord.SelectLetter(letter)) {
                betterRack.SelectLetter(letter);
            }

            inputWord.AddLetter(letter, i);
            // print($"~UpdateBoard.AutomateWordEntryCoroutine selectLetter {letter} \n");
            yield return new WaitForSeconds(.75f);
        }

        yield return new WaitForSeconds(1.0f);

        UpdateBoardForValidSubmit(contents);
        betterRack.SetInteractable(true);
        wordGrid.SetInteractable(true);
        wordGrid2.SetInteractable(true);
        UpdateButtonsInteractable(true);
    }

    private void UpdateButtonsInteractable(bool isInteractable) {
        var buttons = updateButtons.GetComponentsInChildren<Button>();
        foreach (var button in buttons) {
            button.interactable = isInteractable;
        }
    }

    public Player[] GetPlayers() {
        return players;
    }
}
// print("~UpdateBoard.CancelInputWordButton\n");
//scoreManager.CalculateWordScore("BLOOM", "BLOOMING");
//scoreManager.CalculateWordScore("", "HEARTEN");
//scoreManager.CalculateWordScore("HOOD", "HOODED");
//  Stragglers 310disjointed 284
//  scoreManager.CalculateWordScore("", "XXXXXX");
//        scoreManager.CalculateWordScore("", "HEARTEN");
//scoreManager.CalculateWordScore("", "BULLDOG");