using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace bot {
    public class BrucesBot1 : BrucesBot0 {
        protected override ResultMatch FindBestWord(List<ResultMatch> resultMatches) {
            // return random word
            MonoBehaviour.print($"~BrucesBot1.FindBestWord  Count {resultMatches.Count}\n");
            if (resultMatches.Count == 0) return new ResultMatch();

            var index = new Random().Next(0, resultMatches.Count);
            var randomResultMatch = resultMatches[index];

            MonoBehaviour.print($"~BrucesBot1.FindBestWord  randomResultMatch {randomResultMatch} index {index}\n");
            return randomResultMatch;
        }
    }
}