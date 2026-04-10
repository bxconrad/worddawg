using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// bcdo have a bot factory that returns a bot with the code for that level
namespace bot {
    public abstract class BrucesBotAbstract {
        protected int maxLetters = 0;
        public ScoreCalculator scoreCalculator;
        public ITrieDictionary trieDictionary;

        public ResultMatch FindBestestWord(List<Word> sourceWords,
            string rackWord,
            int minNumberLetters = 3,
            int? maxAddedLetters = null,
            int? maxTotalWords = null,
            bool stopAtFirst = false) {
            var resultMatches = GetCombinations(sourceWords, rackWord);
            MonoBehaviour.print(
                $"BrucesBotAbstract.FindBestestWord  New words formed: {resultMatches.Count} rack {rackWord}\n");

            var bestResultMatch = FindBestWord(resultMatches);

            return bestResultMatch;
        }

        protected abstract ResultMatch FindBestWord(List<ResultMatch> resultMatches);

        private List<ResultMatch> GetCombinations(
            List<Word> sourceWords,
            string rackWord,
            int minNumberLetters = 3,
            int? maxAddedLetters = null,
            int? maxTotalWords = null,
            bool stopAtFirst = false) {
            var results = new List<ResultMatch>();
            maxAddedLetters = maxLetters;

            foreach (var source in sourceWords) {
                // Create a combined pool of letters (Original + Rack)
                var combinedPool = source.GetCurrentContents() + rackWord;
                var letterCounts = GetLetterCounts(combinedPool);

                // We need to track the "Minimum Requirements"
                // The result must contain AT LEAST these letters
                var requiredCounts = GetLetterCounts(source.contents);

                var foundForThisSource = new HashSet<string>();

                FindWordsRecursive(
                    "",
                    letterCounts,
                    requiredCounts,
                    source.contents.Length + 1, // Min length: original + 1
                    source.contents.Length + (maxAddedLetters ?? rackWord.Length), // Max length
                    foundForThisSource,
                    stopAtFirst && results.Count == 0
                );

                foreach (var word in foundForThisSource) {
                    results.Add(new ResultMatch { GeneratedWord = word, SourceObject = source });
                    if (stopAtFirst || (maxTotalWords.HasValue && results.Count >= maxTotalWords))
                        return results;
                }
            }

            MonoBehaviour.print("BrucesBotAbstract.GetCombinations  No modified words found. Use rack " + rackWord +
                                "\n");

            // FALLBACK: If no words found, use only the rack
            if (results.Count == 0) {
                var rackOnlyFound = new HashSet<string>();
                FindWordsRecursive(
                    "",
                    GetLetterCounts(rackWord),
                    new Dictionary<char, int>(), // No requirements
                    minNumberLetters,
                    rackWord.Length,
                    rackOnlyFound,
                    stopAtFirst
                );

                foreach (var word in rackOnlyFound) {
                    results.Add(new ResultMatch { GeneratedWord = word, SourceObject = null });
                    if (stopAtFirst || (maxTotalWords.HasValue && results.Count >= maxTotalWords))
                        return results;
                }
            }

            return results;
        }

        private void FindWordsRecursive(
            string currentPrefix,
            Dictionary<char, int> availableLetters,
            Dictionary<char, int> requiredLetters,
            int minLength,
            int maxLength,
            HashSet<string> foundWords,
            bool stopAtFirst) {
            // 1. TRIE CHECK: Prune if this prefix doesn't exist
            if (currentPrefix.Length > 0 && !trieDictionary.HasPrefix(currentPrefix))
                return;

            // 2. DICTIONARY CHECK: Is it a valid word?
            if (currentPrefix.Length >= minLength) {
                if (MeetsRequirements(currentPrefix, requiredLetters)) {
                    if (trieDictionary.Contains(currentPrefix)) {
                        foundWords.Add(currentPrefix);
                        if (stopAtFirst) return;
                    }
                }
            }

            // 3. RECURSION LIMIT
            if (currentPrefix.Length >= maxLength) return;

            // 4. GENERATE PERMUTATIONS
            var keys = availableLetters.Keys.ToList();
            foreach (var c in keys) {
                if (availableLetters[c] > 0) {
                    availableLetters[c]--;
                    FindWordsRecursive(currentPrefix + c, availableLetters, requiredLetters, minLength, maxLength,
                        foundWords, stopAtFirst);
                    availableLetters[c]++; // Backtrack

                    if (stopAtFirst && foundWords.Count > 0) return;
                }
            }
        }

        private bool MeetsRequirements(string word, Dictionary<char, int> required) {
            if (required.Count == 0) return true;
            var wordCounts = GetLetterCounts(word);
            foreach (var kvp in required) {
                if (!wordCounts.ContainsKey(kvp.Key) || wordCounts[kvp.Key] < kvp.Value)
                    return false;
            }

            return true;
        }

        private Dictionary<char, int> GetLetterCounts(string word) {
            var counts = new Dictionary<char, int>();
            foreach (var c in word) {
                if (counts.ContainsKey(c)) counts[c]++;
                else counts[c] = 1;
            }

            return counts;
        }


        protected bool IsPluralized(ResultMatch resultMatch) {
            var originalWord = resultMatch.SourceObject == null
                ? ""
                : resultMatch.SourceObject.GetCurrentContents();
            var newWord = resultMatch.GeneratedWord;

            var retVal = newWord.Length - originalWord.Length == 1 && newWord.StartsWith(originalWord) &&
                         newWord.EndsWith("S");
            MonoBehaviour.print(
                $"BrucesBotAbstract.IsPluralized {retVal} newWord {newWord}  originalWord {originalWord}\n");

            return retVal;
        }
    }
}