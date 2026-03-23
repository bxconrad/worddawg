using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BrucesBot2 : BrucesBot1 {
    public BrucesBot2() {
        maxLetters = 2;
    }

    public override ResultMatch FindBestWord(List<ResultMatch> newWordCombinations) {
        // return random word
        var index = new Random().Next(0, newWordCombinations.Count);
        var bestResultMatch = newWordCombinations[index];
        MonoBehaviour.print("BrucesBot.FindBestWord  random " + bestResultMatch + " index " + index + "\n");


        MonoBehaviour.print("BrucesBot.FindBestWord  highest " + bestResultMatch + "\n");

        return bestResultMatch;
    }
}