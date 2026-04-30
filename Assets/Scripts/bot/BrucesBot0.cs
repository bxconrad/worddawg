using System.Collections.Generic;
using UnityEngine;

namespace bot {
    public class BrucesBot0 : BrucesBotAbstract {
        public BrucesBot0() {
            maxLetters = 1;
        }

        // find lowest scoring word
        protected override ResultMatch FindBestWord(List<ResultMatch> resultMatches) {
            var bestResultMatch = new ResultMatch();
            var lowScore = 10000;

            foreach (var combination in resultMatches) {
                var originalWord = combination.SourceObject == null
                    ? ""
                    : combination.SourceObject.GetCurrentContents();

                var score = scoreCalculator.CalculateWordScore(originalWord, combination.GeneratedWord);
                if (score < lowScore) {
                    lowScore = score;
                    bestResultMatch = combination;
                }
            }

            MonoBehaviour.print($"~BrucesBot0.FindBestWord  lowestWord {bestResultMatch}  lowScore {lowScore}\n");
            return bestResultMatch;
        }

        // geoffrey rule. dont use the S
        protected override bool IsPluralized(string originalWord, string newWord) {
            var retVal = newWord.Length - originalWord.Length == 1 && newWord.StartsWith(originalWord) &&
                         newWord.EndsWith("S");
            if (retVal)
                MonoBehaviour.print(
                    $"~BrucesBot0.IsPluralized {retVal} newWord {newWord}  originalWord {originalWord}\n");

            return retVal;
        }
    }
}