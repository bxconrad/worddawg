using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BrucesBot1 : BrucesBotAbstract {
    public BrucesBot1() {
        maxLetters = 1;
    }


    public override ResultMatch FindBestWord(List<ResultMatch> newWordCombinations) {
        // return random word
        var index = new Random().Next(0, newWordCombinations.Count);
        var bestResultMatch = newWordCombinations[index];
        MonoBehaviour.print("BrucesBot1.FindBestWord  random " + bestResultMatch + " index " + index + "\n");
        return bestResultMatch;
    }
}