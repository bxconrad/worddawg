using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator {
    private readonly GameParameters gameParameters;
    public string[] dogBonusWords;
    public bool isDogBonusWord;

    public ScoreCalculator(GameParameters gameParameters) {
        this.gameParameters = gameParameters;
    }

    public ScoreCalculator(string[] dogBonusWords) {
        MonoBehaviour.print("ScoreCalculator.Start\n");
        this.dogBonusWords = dogBonusWords;
        MonoBehaviour.print("ScoreCalculator.LoadData dogBonusWords " + dogBonusWords.Length + "\n");
    }


    public int CalculateWordScore(string originalWord, string newWord) {
        var letterScore = CalculateLetterScore(newWord);
        var multiplier = CalculateMultiplier(originalWord, newWord);

        var wordScore = Convert.ToInt32(multiplier * letterScore);
        if (CalculateDogBonusWord(newWord)) {
            // bcdo calling CalculateDogBonusWord 2x, fix
            wordScore += 100;
            MonoBehaviour.print("ScoreCalculator.CalculateWordScore CalculateDogBonusWord ");
        }

        // If entire rack is used
        //bcdo fix 7
        if (newWord.Length - originalWord.Length >= 7) {
            // gameParameters.numRackLetters) {
            wordScore += 100;
            MonoBehaviour.print("ScoreCalculator.CalculateWordScore 100 bonus ");
        }

        MonoBehaviour.print("ScoreCalculator.CalculateWordScore wordScore " + wordScore + "  letterScore " +
                            letterScore + "  multiplier " + multiplier + " originalWord " + originalWord + " newWord " +
                            newWord + "\n");

        return wordScore;
    }

    public bool CalculateDogBonusWord(string word) {
        isDogBonusWord = false;
        // MonoBehaviour.print("ScoreCalculator.CalculateDogBonusWord word " + word + "\n");
        for (var i = 0; i < dogBonusWords.Length; i++) {
            if (word.Equals(dogBonusWords[i].ToUpper())) {
                MonoBehaviour.print("ScoreCalculator.CalculateDogBonusWord " + i + " dogBonusWords[i] " +
                                    dogBonusWords[i] + "\n");

                isDogBonusWord = true;
                return true;
            }
        }

        //MonoBehaviour.print("ScoreCalculator.CalculateDogBonusWord false  word " + word + "\n");
        return false;
    }

    private int CalculateLetterScore(string newWord) {
        var newWordLetterScore = 0;
        MonoBehaviour.print("ScoreCalculator.CalculateLetterScore " + gameParameters.language + "  " +
                            LetterInfo.letterDictionaryDictionary["SP"] + "\n");
        var letterDictionary = LetterInfo.letterDictionaryDictionary[gameParameters.language];
        newWord = newWord.Trim();
        foreach (var letter in newWord) {
            try {
                newWordLetterScore += letterDictionary[letter.ToString()];
            }
            catch (KeyNotFoundException e) {
                MonoBehaviour.print("ScoreCalculator.CalculateLetterScore KeyNotFoundException letter {" + letter +
                                    "}  newWord {" + newWord + "} \n");
            }
        }

        return newWordLetterScore;
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
}