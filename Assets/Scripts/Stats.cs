using System;
using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour {
    private static readonly string PREFS_ST_SCORE = "ST_SCORE";
    private static readonly string PREFS_ST_WORDS = "ST_WORDS";
    private static readonly string PREFS_ST_LETTERS = "ST_LETTERS";
    private static readonly string PREFS_ST_CHANGED_WORDS = "ST_CHANGED_WORDS";
    private static readonly string PREFS_ST_LONG_WORD = "ST_LONG_WORD";
    private static readonly string PREFS_ST_BEST_WORD = "ST_BEST_WORD";
    private static readonly string PREFS_ST_BEST_WORD_SCORE = "ST_BEST_WORD_SCORE";
    public static readonly string PREFS_ST_MODE_GOTD = "Game Of The Day";
    public static readonly string PREFS_ST_MODE_UNTIMED_50 = "Untimed, 50 Letters";
    public static readonly string PREFS_ST_MODE_TIMED_4 = "Timed, 4 Minutes";
    public static readonly string PREFS_ST_MODE_CUSTOM = "Custom";

    private static readonly string[] STAT_KEYS = {
        PREFS_ST_BEST_WORD, PREFS_ST_BEST_WORD_SCORE, PREFS_ST_LETTERS,
        PREFS_ST_LONG_WORD, PREFS_ST_CHANGED_WORDS, PREFS_ST_SCORE, PREFS_ST_WORDS
    };

    private static readonly string[] STAT_GAME_MODES = {
        PREFS_ST_MODE_GOTD, PREFS_ST_MODE_UNTIMED_50, PREFS_ST_MODE_TIMED_4, PREFS_ST_MODE_CUSTOM
    };

    [SerializeField] private TextMeshProUGUI titleText;
    private string currentGameMode;
    private int rowNum;

    public void UpdateStats(string gameMode, Player player) {
        print("Stats.UpdateStats\n");
        currentGameMode = gameMode;
        // Add the gameModeSuffix to the key to set different stats for each gameMode (timed, untimed, etc)
        var currentGameModeSuffix = "_" + gameMode;
        titleText.text = gameMode + '\n' + DateTime.Today.ToString("MMM dd, yyyy");

        rowNum = 1; // skip header
        UpdateStat(PREFS_ST_SCORE + currentGameModeSuffix, "Score", player.currentScore);
        UpdateStat(PREFS_ST_WORDS + currentGameModeSuffix, "Words", player.numWords);
        UpdateStat(PREFS_ST_CHANGED_WORDS + currentGameModeSuffix, "Changed", player.numChangedWords);
        UpdateStat(PREFS_ST_LONG_WORD + currentGameModeSuffix, "Longest", player.longestWord);
        UpdateStat(PREFS_ST_BEST_WORD_SCORE + currentGameModeSuffix, "Word Score", player.highestWordScore,
            player.highestWordScoreWord);
        //       UpdateStat(PREFS_ST_LETTERS + currentGameModeSuffix, "Letters", player.numLettersUsed);
    }

    private void UpdateStat(string key, string label, int current) {
        UpdateStat(key, label, current, "");
    }

    private void UpdateStat(string key, string label, int current, string highestWordScoreWord) {
        var high = PlayerPrefs.GetInt(key);
        var bestMarker = "";
        print("Stats.UpdateStat key " + key + " current " + current + " high  " + high + "\n");
        if (current > high) {
            bestMarker = "*";
            high = current;
            PlayerPrefs.SetInt(key, high);
            print("Stats.UpdateStat HIGH key " + key + "  current " + current + " high  " + high + "\n");
        }

        UpdateBestWordStats(key, bestMarker, highestWordScoreWord);

        UpdateRow(label, current.ToString(), high.ToString(), bestMarker);
    }

    // special case for PREFS_ST_BEST_WORD based on PREFS_ST_BEST_WORD_SCORE
    private void UpdateBestWordStats(string key, string bestMarker, string highestWordScoreWord) {
        //bcdo player refactor fix highestWordScoreWord
        if (key.StartsWith(PREFS_ST_BEST_WORD_SCORE)) {
            var bestWordKey = PREFS_ST_BEST_WORD + "_" + currentGameMode;
            var currentBestWord = highestWordScoreWord;
            var allTimeBestWord = "";
            if ("*".Equals(bestMarker)) {
                allTimeBestWord = currentBestWord;
                PlayerPrefs.SetString(bestWordKey, currentBestWord);
            }
            else {
                allTimeBestWord = PlayerPrefs.GetString(bestWordKey);
            }

            print("Stats.UpdateBestWordStats bestWordKey " + bestWordKey +
                  " allTimeBestWord " + allTimeBestWord + " \n");

            UpdateRow("Best Word", currentBestWord, allTimeBestWord, bestMarker);
        }
    }


    private void UpdateStat(string key, string label, string current) {
        var high = PlayerPrefs.GetString(key);
        print("Stats.UpdateStat" + label + key + high + " \n");
        var bestMarker = "";
        if (current.Length > high.Length) {
            bestMarker = "*";
            high = current;
            PlayerPrefs.SetString(key, high);
        }

        UpdateRow(label, current, high, bestMarker);
    }

    private void UpdateRow(string label, string var1, string var2, string bestMarker) {
        print("Stats.UpdateRow" + label + " " + var1 + " " + var2 + " \n");
        var statRow = transform.GetChild(rowNum);
        var textUpdaters = statRow.GetComponentsInChildren<TextUpdater>();
        textUpdaters[0].SetMyText(label);
        textUpdaters[1].SetMyText(var1);
        textUpdaters[2].SetMyText(var2);
        textUpdaters[3].SetMyText(bestMarker);
        rowNum++;
    }

    public void OnResetStatsButtonClicked() {
        print("Stats.OnResetStatsButtonClicked \n");

        foreach (var statKey in STAT_KEYS) {
            PlayerPrefs.DeleteKey(statKey);
            foreach (var gameMode in STAT_GAME_MODES) {
                var key = statKey + "_" + gameMode;
                PlayerPrefs.DeleteKey(key);
                print("Stats.OnResetStatsButtonClicked " + key + " \n");
            }
        }

        ResetPrefs();
        //UpdateStats(currentGameMode);
    }

    private void ResetPrefs() {
        print("Stats.ResetPrefs " + MyPrefs.GetNumRackLetters() + " \n");
        foreach (var key in MyPrefs.PREFS_KEYS) {
            PlayerPrefs.DeleteKey(key);
        }
    }
}