using System.Collections.Generic;
using UnityEngine;

public class BrucesBot0 : BrucesBotAbstract {
    public BrucesBot0() {
        maxLetters = 1;
    }

    public override ResultMatch FindBestWord(List<ResultMatch> newWordCombinations) {
        var bestResultMatch = new ResultMatch();
        // find lowest scoring word
        var lowScore = 10000;

        foreach (var combination in newWordCombinations) {
            var originalWord = combination.SourceObject == null
                ? ""
                : combination.SourceObject.GetCurrentContents();
            var score = scoreCalculator.CalculateWordScore(originalWord, combination.GeneratedWord);
            if (score < lowScore) {
                lowScore = score;
                bestResultMatch = combination;
            }

            MonoBehaviour.print(
                "BrucesBoto.FindBestWord  lowestWord " + bestResultMatch + " lowScore " + lowScore + "\n");

            return bestResultMatch;
        }


        MonoBehaviour.print("BrucesBot.FindBestWord  highest " + bestResultMatch + "\n");

        return bestResultMatch;
    }
}