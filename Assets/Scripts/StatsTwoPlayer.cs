using System;
using EasyUI.Toast;
using TMPro;
using UnityEngine;

public class StatsTwoPlayer : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI titleText;

    private string currentGameMode;
    private int rowNum;

    public void UpdateStats(string gameMode, Player player, Player player2) {
        print("~StatsTwoPlayer.UpdateStats player1 " + player + "\n");
        print("~StatsTwoPlayer.UpdateStats player2 " + player2 + "\n");

        ShowToastMessage(player, player2);
        currentGameMode = gameMode;
        // Add the gameModeSuffix to the key to set different stats for each gameMode (timed, untimed, etc)
        var currentGameModeSuffix = "_" + gameMode;
        titleText.text = gameMode + " vs " + player2.name + '\n' + DateTime.Today.ToString("MMM dd, yyyy");
        rowNum = 0;
        UpdateRow("", player.name, player2.name, "");

        UpdateStat(Stats.PREFS_ST_SCORE + currentGameModeSuffix, "Score", player.currentScore, player2.currentScore);
        UpdateStat(Stats.PREFS_ST_WORDS + currentGameModeSuffix, "Words", player.numWords, player2.numWords);
        UpdateStat(Stats.PREFS_ST_CHANGED_WORDS + currentGameModeSuffix, "Changed", player.numChangedWords,
            player2.numChangedWords);
        UpdateStatLongestWord(Stats.PREFS_ST_LONG_WORD + currentGameModeSuffix, "Longest", player.longestWord,
            player2.longestWord);
    }

    private static void ShowToastMessage(Player player, Player player2) {
        var toastMessage = $"And the winner is  {player2.name}. Better luck next time  {player.name}";
        if (player.currentScore > player2.currentScore) {
            toastMessage = $"And the winner is  {player.name}. You Beat {player2.name}!!! Congratulations!!!";
        }
        else if (player.currentScore == player2.currentScore) {
            toastMessage = "It's a tie!!! It's not easy to tie " + player2.name;
        }

        Toast.Show(
            toastMessage, 15f, Color.magenta, GameHelper.GetToastPosition());
    }


    private void UpdateStat(string key, string label, int current, int currentPlayer2) {
        var high = PlayerPrefs.GetInt(key);
        var bestMarker = "";
        print("~StatsTwoPlayer.UpdateStat key " + key + " current " + current + " high  " + high + "\n");
        if (current > high) {
            bestMarker = "*";
            high = current;
            PlayerPrefs.SetInt(key, high);
            print("~StatsTwoPlayer.UpdateStat HIGH key " + key + "  current " + current + " high  " + high + "\n");
        }

        UpdateRow(label, current.ToString(), currentPlayer2.ToString(), bestMarker);
    }


    // called for longest word
    private void UpdateStatLongestWord(string key, string label, string current, string currentPlayer2) {
        var high = PlayerPrefs.GetString(key);
        print("~StatsTwoPlayer.UpdateStat" + label + key + high + " \n");
        var bestMarker = "";
        if (current.Length > high.Length) {
            bestMarker = "*";
            high = current;
            PlayerPrefs.SetString(key, high);
        }

        UpdateRow(label, current, currentPlayer2, bestMarker);
    }

    private void UpdateRow(string label, string var1, string var2, string bestMarker) {
        print("~StatsTwoPlayer.UpdateRow" + label + " " + var1 + " " + var2 + " \n");
        var statRow = transform.GetChild(rowNum);
        var textUpdaters = statRow.GetComponentsInChildren<TextUpdater>();
        textUpdaters[0].SetMyText(label);
        textUpdaters[1].SetMyText(var1 + " " + bestMarker);
        textUpdaters[2].SetMyText(var2);
        rowNum++;
    }
}