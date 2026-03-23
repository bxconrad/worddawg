using System;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    private static Player DUMMY_PLAYER;
    private readonly List<Word> words = new();
    public int currentScore;
    public bool isBot;
    public string longestWord = "";
    public string name;
    public ScoreGrid scoreGrid;
    public WordGrid wordGrid;


    public Player(string name) {
        this.name = name;
    }

    public Player(string name, WordGrid wordGrid, ScoreGrid scoreGrid) {
        Initialize(name, wordGrid, scoreGrid);
    }

    public Word currentWord { get; private set; }

    public int wordScore { get; set; }
    public int highestWordScore { get; private set; }
    public string highestWordScoreWord { get; private set; }
    public int numWords { get; private set; }
    public int numChangedWords { get; private set; }
    public int numLettersUsed { get; set; }

    public void Initialize(string name, WordGrid wordGrid, ScoreGrid scoreGrid) {
        this.name = name;
        this.wordGrid = wordGrid;
        this.scoreGrid = scoreGrid;
    }

    public void Initialize() {
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
        var subtractPoints = Math.Min(currentScore, points) * -1;
        currentScore += subtractPoints;
        scoreGrid.totalScore.text = currentScore.ToString().PadRight(5) + subtractPoints;
        MonoBehaviour.print("Player.UpdateScoreForReplaceRack " + points + "\n");
    }

    public override string ToString() {
        return $"CurrentScore: {currentScore}, IsBot: {isBot}, Name: {name}, wordScore: {wordScore}";
    }
    // public override string ToString() {
    //     return
    //         $"{nameof(name)}: {name}";
    //    // $"{nameof(name)}: {name}, {nameof(words)}: {words}, {nameof(currentScore)}: {currentScore}, {nameof(isBot)}: {isBot}, {nameof(longestWord)}: {longestWord},  {nameof(currentWord)}: {currentWord}, {nameof(wordScore)}: {wordScore}, {nameof(highestWordScore)}: {highestWordScore}, {nameof(highestWordScoreWord)}: {highestWordScoreWord}, {nameof(numWords)}: {numWords}, {nameof(numChangedWords)}: {numChangedWords}";
    // }
}