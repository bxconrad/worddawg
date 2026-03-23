using System.Collections.Generic;
using UnityEngine;

public class BrucesBot3 : BrucesBotAbstract {
    public BrucesBot3() {
        maxLetters = 3;
    }

    public override ResultMatch FindBestWord(List<ResultMatch> newWordCombinations) {
        var bestResultMatch = new ResultMatch();

        var highScore = 0;

        foreach (var combination in newWordCombinations) {
            var originalWord = combination.SourceObject == null
                ? ""
                : combination.SourceObject.GetCurrentContents();
            var score = scoreCalculator.CalculateWordScore(originalWord, combination.GeneratedWord);
            if (score > highScore) {
                highScore = score;
                bestResultMatch = combination;
            }
        }

        MonoBehaviour.print("BrucesBot3.FindBestWord  highest " + bestResultMatch + " highScore " + highScore + "\n");
        return bestResultMatch;
    }
}