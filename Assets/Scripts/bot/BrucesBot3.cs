using System.Collections.Generic;
using UnityEngine;

namespace bot {
    public class BrucesBot3 : BrucesBotAbstract {
        public BrucesBot3() {
            maxLetters = 3;
        }

        protected override ResultMatch FindBestWord(List<ResultMatch> resultMatches) {
            var bestResultMatch = new ResultMatch();
            var highScore = 0;

            foreach (var combination in resultMatches) {
                //if (IsPluralized(combination)) continue;
                var originalWord = combination.SourceObject == null
                    ? ""
                    : combination.SourceObject.GetCurrentContents();
                var score = scoreCalculator.CalculateWordScore(originalWord, combination.GeneratedWord);
                if (score > highScore) {
                    highScore = score;
                    bestResultMatch = combination;
                }
            }

            MonoBehaviour.print($"BrucesBot3.FindBestWord  highest {bestResultMatch}  highScore {highScore}\n");
            return bestResultMatch;
        }
    }
}