using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace bot {
    public class BrucesBot1 : BrucesBotAbstract {
        public BrucesBot1() {
            maxLetters = 1;
        }

        protected override ResultMatch FindBestWord(List<ResultMatch> resultMatches) {
            // return random word
            MonoBehaviour.print($"BrucesBot1.FindBestWord  Count {resultMatches.Count}\n");
            if (resultMatches.Count == 0) return new ResultMatch();
            for (var i = 0; i < 10; i++) {
                var index = new Random().Next(0, resultMatches.Count);
                var randomResultMatch = resultMatches[index];
                // geoffrey rule. dont use the S
                if (IsPluralized(randomResultMatch)) continue;

                MonoBehaviour.print($"BrucesBot1.FindBestWord  randomResultMatch {randomResultMatch} index {index}\n");
                return randomResultMatch;
            }

            MonoBehaviour.print("BrucesBot1.FindBestWord  could not find after 10 tries\n");
            return new ResultMatch();
        }
    }
}