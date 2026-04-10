using System;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    private static Player DUMMY_PLAYER;
    private readonly ScoreGrid scoreGrid;
    private readonly List<Word> words = new();
    public int currentScore;
    public bool isBot;
    public string longestWord = "";
    public string name;
    public WordGrid wordGrid;


    public Player(string name) {
        this.name = name;
    }

    public Player(string aname, WordGrid awordGrid, ScoreGrid ascoreGrid) {
        name = aname;
        wordGrid = awordGrid;
        scoreGrid = ascoreGrid;
        Initialize();
    }

    public Word currentWord { get; private set; }

    public int wordScore { get; set; }
    public int highestWordScore { get; private set; }
    public string highestWordScoreWord { get; private set; }
    public int numWords { get; private set; }
    public int numChangedWords { get; private set; }
    public int numLettersUsed { get; set; }

    public bool IsStolenWord() {
        return currentWord.IsStolenWord();
    }

    private void Initialize() {
        if (scoreGrid == null)
            return;
        var currentWordScore = currentWord != null ? currentWord.currentWordHistory.score : 0;
        scoreGrid.totalScore.text = currentScore.ToString().PadRight(5) + currentWordScore;
        scoreGrid.name.text = name;
    }

    public void UpdateScoreText() {
        var currentWordScore = currentWord != null ? currentWord.currentWordHistory.score : 0;
        scoreGrid.totalScore.text = currentScore.ToString().PadRight(5) + currentWordScore;
    }

    public void Activate(bool isActive) {
        if (isActive) {
            scoreGrid.image.color = Color.blue;
        }
        else {
            scoreGrid.image.color = Color.black;
        }
    }

    public void AddNewWord(Word word) {
        currentWord = word;
        words.Add(word);
        UpdateHighScores(word);
    }

    public void UpdateExistingWord(Word word, string content, int score) {
        currentWord = word;
        word.CreateWord(content, this, score);
        UpdateHighScores(word);
    }


    private void UpdateHighScores(Word word) {
        wordScore += word.currentWordHistory.score;
        numWords++;
        numLettersUsed += word.GetCurrentContents().Length - word.GetPreviousContents().Length;
        if (word.currentWordHistory.score > highestWordScore) {
            highestWordScore = word.currentWordHistory.score;
            highestWordScoreWord = word.GetCurrentContents();
        }

        if (word.GetCurrentContents().Length > longestWord.Length) longestWord = word.GetCurrentContents();

        if (word.GetNumModified() > 1) numChangedWords++;

        MonoBehaviour.print("Player.UpdateScore " + currentScore + " wordScore " + wordScore + "\n");
    }

    public void UpdateScoreForReplaceRack(int points) {
        var subtractPoints = isBot ? points * -1 : Math.Min(currentScore, points) * -1;
        currentScore += subtractPoints;
        scoreGrid.totalScore.text = currentScore.ToString().PadRight(5) + subtractPoints;
        MonoBehaviour.print($"Player.UpdateScoreForReplaceRack subtractPoints {subtractPoints}  points {points}\n");
    }

    public override string ToString() {
        return $"CurrentScore: {currentScore}, IsBot: {isBot}, Name: {name}, wordScore: {wordScore}";
    }
}