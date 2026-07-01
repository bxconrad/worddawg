using System.Collections;
using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBoardTwo : UpdateBoardAbstract {
    protected override void NewGameInitializePlayers() {
        UpdateButtonsInteractable(true);
        ActivatePanels(true);
        print($"~UpdateBoardTwo.Start NewGameInitializePlayers scoreGrid1 -{scoreGrid1}- scoreGrid2 -{scoreGrid2}-\n");
        GetPlayerOrder();
        wordGrid2.Initialize();
        var msg = players[0].name + " goes first. Then it's your turn " + players[1].name;
        Toast.Show(msg, 15f, Color.magenta, GameHelper.GetToastPosition());
    }

    protected virtual void GetPlayerOrder() {
        var player1 = new Player(GetOpponentName(), wordGrid1, scoreGrid1);
        var player2 = new Player(gameParameters.userName, wordGrid2, scoreGrid2);
        players = new[] { player1, player2 };
    }

    protected virtual string GetOpponentName() {
        return MyPrefs.BOT_NAMES[gameParameters.botLevel];
    }


    protected override void NextTurn() {
        if (isEndGame) return;
        turnNumber++;
        playerNumber = gameParameters.isTwoPlayer ? turnNumber % 2 : turnNumber % 1;
        currentPlayer = players[playerNumber];
        print($"~UpdateBoardTwo.NextTurn playerNumber {playerNumber}  name {currentPlayer.name} \n");
        OtherPlayer().Activate(false);
        currentPlayer.Activate(true);
        print($"~UpdateBoardTwo.NextTurn betterRack {betterRack} \n");
    }


    // Call this method to start a pause for a specific duration
    protected void AutomatedReplaceRack(float time) {
        print("~UpdateBoardTwo.AutomatedReplaceRack\n");
        StartCoroutine(AutomatedReplaceRackCoroutine(time));
    }

    private IEnumerator AutomatedReplaceRackCoroutine(float time) {
        print("~UpdateBoardTwo.AutomatedReplaceRackCoroutine\n");
        yield return new WaitForSeconds(time);
        ReplaceRackButton();
        NextTurn();
    }

    protected void AutomateWordEntry(Word word, string contents) {
        print("~UpdateBoardTwo.AutomateWordEntry\n");
        StartCoroutine(AutomateWordEntryCoroutine(word, contents));
    }

    private IEnumerator AutomateWordEntryCoroutine(Word word, string contents) {
        print($"~UpdateBoardTwo.AutomateWordEntryCoroutine yield turnNumber {turnNumber}\n");
        SetComponentsInteractable(false);

        yield return new WaitForSeconds(2.0f);
        if (word != null) {
            // bcdo should never be null
            print($"~UpdateBoardTwo.AutomateWordEntryCoroutine betterRack {betterRack} \n");

            LoadSelectedWord(word);
            var wordOwner = word.currentWordHistory.player;
            // Update the selected button/word in the wordGrid
            var button = wordOwner.wordGrid.FindMatchingButton(word);
            if (button == null) {
                // bcdo should never be null
                print($"~UpdateBoardTwo.AutomateWordEntryCoroutine *** Problem with owning player **** {wordOwner} \n");
            }
            else {
                button.gameObject.SetActive(true);
                button.SelectButton(); // updates color
                // this is needed so we know which button to deselect
                wordOwner.wordGrid.selectedButton = button;
            }
        }

        yield return new WaitForSeconds(1.0f);

        print("~UpdateBoardTwo.AutomateWordEntryCoroutine selectLetters \n");
        for (var i = 0; i < contents.Length; i++) {
            var letter = contents.Substring(i, 1);
            if (!selectedWord.SelectLetter(letter)) {
                betterRack.SelectLetter(letter);
            }

            inputWord.AddLetter(letter, i);
            // print($"~UpdateBoardTwo.AutomateWordEntryCoroutine selectLetter {letter} \n");
            yield return new WaitForSeconds(.75f);
        }

        yield return new WaitForSeconds(1.0f);

        UpdateBoardForValidSubmit(contents);
        SetComponentsInteractable(true);
        print($"~UpdateBoardTwo.AutomateWordEntryCoroutine betterRack {betterRack} \n");
    }

    private void SetComponentsInteractable(bool isInteractable) {
        UpdateButtonsInteractable(isInteractable);
        betterRack.SetInteractable(isInteractable);
        //   selectedWord.SetInteractable(isInteractable);
        inputWord.SetInteractable(isInteractable);
        wordGrid1.SetInteractable(isInteractable);
        wordGrid2.SetInteractable(isInteractable);
    }

    protected virtual void UpdateButtonsInteractable(bool isInteractable) {
        print($"~UpdateBoardTwo.UpdateButtonsInteractable  isInteractable {isInteractable}\n");
        var buttons = updateButtons.GetComponentsInChildren<Button>();
        foreach (var button in buttons) {
            print($"~UpdateBoardTwo.UpdateButtonsInteractable  button {button}\n");
            button.interactable = isInteractable;
        }
    }
}