using System;
using EasyUI.Toast;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public TMP_Text scoreText;

    private int letterScore;
    public int currentScore { get; private set; }
    public int wordScore { get; private set; }
    public string longestWord { get; private set; }
    public int highestWordScore { get; private set; }
    public string highestWordScoreWord { get; private set; }
    public int numWords { get; private set; }
    public int numChangedWords { get; private set; }
    public int numLettersUsed { get; set; }

    public void Awake() {
        Initialize();
    }

    public void Initialize() {
        currentScore = 0;
        scoreText.text = "0";
        longestWord = "";
        highestWordScoreWord = "";
        highestWordScore = 0;
        numChangedWords = 0;
        numWords = 0;
        numLettersUsed = 0;
    }

    public void End() {
        scoreText.text = "";
    }

    public void UpdateScore(string originalWord, string newWord) {
        currentScore += CalculateWordScore(originalWord, newWord);
        numLettersUsed += newWord.Length - originalWord.Length;
        UpdateHighScores(originalWord, newWord);
        updateScoreText();
        print("ScoreManager.UpdateScore " + currentScore + " wordScore " + wordScore + "\n");
    }

    public void UpdateScoreForReplaceRack() {
        currentScore = Math.Max(0, currentScore -= 50);
        updateScoreText();
        print("ScoreManager.UpdateScoreForRack " + currentScore + "\n");
    }

    public int CalculateWordScore(string originalWord, string newWord) {
        letterScore = CalculateLetterScore(newWord);
        var multiplier = CalculateMultiplier(originalWord, newWord);

        wordScore = Convert.ToInt32(multiplier * letterScore);
        // If entire rack is used
        if (newWord.Length - originalWord.Length >= MyPrefs.NUM_RACK_LETTERS) {
            wordScore += 100;
            print("ScoreManager.CalculateWordScore 100 bonus ");
            Toast.Show("100 Point Bonus!!! Great Job!", Color.blue, ToastPosition.BottomCenter);
        }

        print("ScoreManager.CalculateWordScore wordScore " + wordScore + "  letterScore " + letterScore +
              "  multiplier " + multiplier + " originalWord " + originalWord + " newWord " + newWord + "\n");

        return wordScore;
    }

    private int CalculateLetterScore(string newWord) {
        var letterScore = 0;
        foreach (var letter in newWord) {
            letterScore += LetterInfo.letterDictionary[letter.ToString()];
        }
        return letterScore;
    }

    private double CalculateMultiplier(string originalWord, string newWord) {
        var factor = IsAddedSorD(originalWord, newWord) ? 1 : 1.5;
        var multiplier = newWord.Length > 4 ? (newWord.Length - 1) * factor : 1;
        return multiplier;
    }

    private bool IsAddedSorD(string originalWord, string newWord) {
        return newWord.Length - originalWord.Length == 1 && newWord.StartsWith(originalWord) &&
               (newWord.EndsWith("D") || newWord.EndsWith("S"));
    }


    private void UpdateHighScores(string originalWord, string newWord) {
        if (wordScore > highestWordScore) {
            highestWordScore = wordScore;
            highestWordScoreWord = newWord;
        }

        if (newWord.Length > longestWord.Length) {
            longestWord = newWord;
        }

        numWords++;
        if (originalWord.Length > 0) {
            numChangedWords++;
        }
    }

    private void updateScoreText() {
        scoreText.text = currentScore.ToString().PadRight(5) + wordScore;
    }
}