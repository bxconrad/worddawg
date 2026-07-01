using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public abstract class UpdateBoardAbstract : MonoBehaviour {
    [SerializeField] protected ServiceLocator serviceLocator;

    protected BetterRack betterRack;
    protected Player currentPlayer;
    private Dealer dealer;
    protected DictionaryManager dictionaryManager;
    private GameObject dummyPanel;
    protected GameParameters gameParameters;
    protected InputWord inputWord;
    protected bool isEndGame;
    private GameObject logoImage2;
    protected int playerNumber;
    protected Player[] players;
    private UpdateBoardButtonHandler replaceRackButton;
    protected string saveLanguage = "";
    protected ScoreCalculator scoreCalculator;
    protected ScoreGrid scoreGrid1;
    protected ScoreGrid scoreGrid2;
    private GameObject scorePanel;
    protected SelectedWord selectedWord;
    private GameObject selectedWordGo;
    private GameObject timeScorePanel;
    private ToastMaster toastMaster;
    private TransformShaker transformShaker;
    protected int turnNumber;
    protected GameObject updateButtons;
    private ValidatorManager validatorManager;
    protected WordGrid wordGrid1;
    protected WordGrid wordGrid2;
    private GameObject wordGridScrollPanel2;

    protected abstract void NewGameInitializePlayers();
    protected abstract void NextTurn();

    // --- new game initialization ---
    public void InitializeVariables() {
        print($"~UpdateBoardAbstract.InitializeVariables  {serviceLocator}\n");
        serviceLocator = ServiceLocator.instance;
        inputWord = serviceLocator.inputWord;

        selectedWord = serviceLocator.selectedWord;
        selectedWordGo = selectedWord.gameObject;
        betterRack = GetComponentInChildren<BetterRack>();

        dealer = serviceLocator.dealer;
        gameParameters = serviceLocator.gameParameters;
        transformShaker = serviceLocator.transformShaker;
        toastMaster = serviceLocator.toastMaster;

        dummyPanel = serviceLocator.dummyPanel;
        logoImage2 = serviceLocator.logoImage2;
        replaceRackButton = serviceLocator.replaceRackButton;
        scorePanel = serviceLocator.scorePanel;
        updateButtons = serviceLocator.updateButtons;
        scoreGrid1 = serviceLocator.scoreGrid1;
        timeScorePanel = serviceLocator.timeScorePanel;
        wordGrid1 = serviceLocator.wordGrid1;
        wordGrid2 = serviceLocator.wordGrid2;
        wordGridScrollPanel2 = serviceLocator.wordGridScrollPanel2;
        scoreGrid2 = serviceLocator.scoreGrid2;
        dictionaryManager = serviceLocator.dictionaryManager;
        validatorManager = new ValidatorManager();
        scoreCalculator = new ScoreCalculator(gameParameters);
        print($"~UpdateBoardAbstract.InitializeVariables gameParameters {gameParameters} \n");
    }


    public void NewGame() {
        print($"~UpdateBoardAbstract.NewGame gameParameters -{gameParameters}- \n");
        isEndGame = false;
        NewGameBuildDictionaries();
        NewGameInitializePlayers();
        InitializeComponents();
        currentPlayer = players[0];
        currentPlayer.Activate(true);
        toastMaster.UpdateScoreText(currentPlayer);

        turnNumber = -1;
        NextTurn();
    }

    private void InitializeComponents() {
        selectedWord.Initialize();
        wordGrid1.Initialize();
        inputWord.Initialize();
        dealer.Initialize();
        replaceRackButton.Initialize();
    }

    protected void ActivatePanels(bool isTwoPlayer) {
        print($"~UpdateBoardAbstract.ActivatePanels {isTwoPlayer}\n");
        scoreGrid2.gameObject.SetActive(isTwoPlayer);
        wordGridScrollPanel2.SetActive(isTwoPlayer);
        scorePanel.SetActive(isTwoPlayer);
        logoImage2.SetActive(isTwoPlayer);
        timeScorePanel.SetActive(!isTwoPlayer);
        // for one player wordGrid is two/both columns. For two player, each has own/1 column
        wordGrid1.UpdateGridLayoutConstraint(isTwoPlayer ? 1 : 2);
        selectedWordGo.SetActive(false);
        dummyPanel.SetActive(true);
    }

    //  need to reload dictionary at startup or when botLevel or language has changed 
    protected virtual void NewGameBuildDictionaries() {
        print($"~UpdateBoardAbstract.NewGameBuildDictionaries gameParameters -{gameParameters}- \n");
        if (gameParameters.language.Equals(saveLanguage)) {
            return;
        }

        saveLanguage = gameParameters.language;
        validatorManager.trieDictionary = dictionaryManager.BuildDictionary(gameParameters);
        scoreCalculator.dogBonusWords = dictionaryManager.BuildDictionaryDogWords();
    }


    // --- Submit update logic ---

    // Begin submit processing and validation
    public void SubmitInputWordButton() {
        print($"~UpdateBoardAbstract.SubmitInputWordButton {currentPlayer} inputWord {inputWord} \n");
        Toast.Dismiss();

        var validationResult = validatorManager.ValidateInputWord(selectedWord, betterRack, inputWord.GetWord());
        if ("TRUE".Equals(validationResult)) {
            UpdateBoardForValidSubmit(inputWord.GetWord());
        }
        else {
            Toast.Show(validationResult, 2f, Color.red, GameHelper.GetToastPosition());
            transformShaker.BeginShake(inputWord.transform);
        }
    }


    // Major submit processing for valid submit
    protected void UpdateBoardForValidSubmit(string expandedInputString) {
        var word = selectedWord.GetWordObject();
        // if selectedWord is not active it is a new word, else we are modifying
        var isNewWord = !selectedWord.gameObject.activeInHierarchy;
        var originalContents = isNewWord ? "" : word.contents;
        // if we find this word already in players wordGrid, isSamePlayer will be true
        var isSamePlayer = currentPlayer.wordGrid.FindDisplayButtonForWordObject(word) != null;
        var isSteal = !isNewWord && !isSamePlayer && !currentPlayer.isBot;
        print(
            $"~~UpdateBoardAbstract.UpdateBoardForValidSubmit isSteal {isSteal} isNewWord {isNewWord} isSamePlayer {isSamePlayer}\n");
        var score = scoreCalculator.CalculateWordScore(originalContents, expandedInputString, isSteal);

        // For new word, add to player and its wordGrid
        if (isNewWord) {
            print("~UpdateBoardAbstract.UpdateBoardForValidSubmit new word\n");
            word = new Word(expandedInputString, currentPlayer, score);
            currentPlayer.AddNewWord(word);
            currentPlayer.wordGrid.InstantiateDisplayButton(word);
        }
        // For Solo play, isSamePlayer will always be true so it will never be a steal
        else if (isSamePlayer) {
            // Player modified own word, Update the word for the current player
            currentPlayer.UpdateExistingWord(word, expandedInputString, score);
            currentPlayer.wordGrid.UpdateDisplayButton(word);
        }
        else {
            // It's a steal! Add word to current player and remove from other player
            print("~UpdateBoardAbstract.UpdateBoardForValidSubmit  STEAL!!! \n");
            currentPlayer.UpdateExistingWord(word, expandedInputString, score);
            currentPlayer.wordGrid.InstantiateDisplayButton(word);
            OtherPlayer().wordGrid.DestroyDisplayButtonForWordObject(word);
        }

        // Update the display after the submit
        UpdateAfterSubmit(word);
        print("~UpdateBoardAbstract.UpdateBoardForValidSubmit \n");
        NextTurn();
    }

    // complete submit processing
    private void UpdateAfterSubmit(Word word) {
        selectedWord.gameObject.SetActive(false);
        dummyPanel.SetActive(true);
        toastMaster.UpdateScoreText(currentPlayer);
        currentPlayer.UpdateScoreText();
        currentPlayer.currentWord.currentWordHistory.isDogBonusWord =
            scoreCalculator.CalculateDogBonusWord(word.contents); // repeat for bot for toast message

        toastMaster.ShowToastMessage(currentPlayer);
        betterRack.RemoveSelectedLetters();

        // this makes list scroll to top and appear correctly. bcdo move to wordGrid
        var scrollRect = currentPlayer.wordGrid.GetComponentInParent<ScrollRect>();
        print($"~UpdateBoardAbstract.UpdateAfterSubmit scrollRect {scrollRect}\n");
        scrollRect.verticalNormalizedPosition = 1.0f;

        ClearInputWord();
        RemoveSelectedWord();

        dealer.Deal();
        HandleNearEndGame();
        print($"~UpdateBoardAbstract.UpdateAfterSubmit betterRack {betterRack}\n");
    }

    // --- other non-submit logic ----

    public void ReplaceRackButton() {
        print($"~UpdateBoardAbstract.UpdateBoardButtonHandler {dealer.IsNearEndGame()}\n");

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
        print($"~UpdateBoardAbstract.UpdateBoardButtonHandler   currentPlayer {currentPlayer}\n");

        ClearInputWord();
        dealer.DealNewRack();
        HandleNearEndGame();
        NextTurn();
    }

    // onButtonClick callback from wordGrid1
    // We have selected an existing word. Put it in SelectedWord display.
    public void LoadSelectedWord(Word wordObject) {
        print("~UpdateBoardAbstract.LoadSelectedWord begin\n");
        if (wordObject != null) {
            print($"~UpdateBoardAbstract.LoadSelectedWord  contents {wordObject.GetCurrentContents()}\n");
            dummyPanel.SetActive(false);
            selectedWord.gameObject.SetActive(true);
            selectedWord.InitializeWord(wordObject);
            // make sure all other words in wordGrids are deselected except for this one
            wordGrid1.DeselectMismatchButton(wordObject);
            wordGrid2.DeselectMismatchButton(wordObject);
            // Reset the input word to blank
            ClearInputWord();
            Toast.Dismiss();
        }
    }


    // --- Misc Resetting and reinitialization ---
    private void RemoveSelectedWord() {
        selectedWord.InitializeTiles(string.Empty);
        selectedWordGo.SetActive(false);
        dummyPanel.SetActive(true);
    }

    public void EndGame() {
        isEndGame = true;
        print($"~UpdateBoardAbstract.EndGame {betterRack.GetWord()} \n");
    }

    public void ClearInputWord() {
        print($"~UpdateBoardAbstract.ClearInputWordButton betterRack {betterRack}\n");
        inputWord.Initialize();
        selectedWord.ResetStateUnselected();
        betterRack.ResetStateUnselected();
    }


    private void HandleNearEndGame() {
        if (dealer.IsNearEndGame()) {
            print($"~UpdateBoardAbstract.HandleNearEndGame true  {dealer.GetTotalNumLettersLeft()}\n");
            replaceRackButton.ChangeForEndGame();
        }
    }


    public void CancelUpdateButton() {
        ClearInputWord();
        print("~UpdateBoardTwo.CancelInputWordButton calling deselect \n");
        currentPlayer.wordGrid.DeselectButton();
        OtherPlayer().wordGrid.DeselectButton();
        RemoveSelectedWord();
    }

    public Player[] GetPlayers() {
        return players;
    }

    protected Player OtherPlayer() {
        var otherPlayer = players[0];
        if (playerNumber == 0) {
            if (gameParameters.isTwoPlayer) otherPlayer = players[1];
        }

        return otherPlayer;
    }
}