using System;
using System.Collections.Generic;
using System.Linq;

public class WordGeneratorOriginal {
    private readonly DicionaryTrie _dictionary;
    private Dictionary<char, int> _originalRequiredCounts;
    private Dictionary<char, int> _rackOnlyCounts;

    public WordGeneratorOriginal(DicionaryTrie dictionary) {
        _dictionary = dictionary;
    }

    /// <summary>
    ///     Generates words based on the combined letters of the originalWord and rackWord.
    /// </summary>
    /// <param name="originalWord">The mandatory letter set for combinations.</param>
    /// <param name="rackWord">The pool of letters that must contribute at least one letter.</param>
    /// <param name="minNumberLetters">Minimum length for words generated in the fallback phase.</param>
    /// <param name="options">Optional control parameters.</param>
    /// <returns>A list of valid words found.</returns>
    public List<string> GenerateWords(string originalWord, string rackWord, int minNumberLetters,
        GenerationOptions options) {
        var resultWords = new List<string>();

        // 1. Calculate the total available letter pool and required original letters.
        var totalPool = GetLetterCounts(originalWord + rackWord);
        _originalRequiredCounts = GetLetterCounts(originalWord);
        _rackOnlyCounts = GetLetterCounts(rackWord);

        // --- PHASE 1: originalWord + rackWord Combinations ---
        Console.WriteLine($"--- Starting Phase 1: {originalWord} + {rackWord} ---");

        // The maximum allowed length is the original length + MaxLettersToAdd
        var maxLengthPhase1 = originalWord.Length + options.MaxLettersToAdd;

        // Start the recursive search with an empty word.
        FindWordsRecursive(
            "",
            totalPool,
            options,
            resultWords,
            false,
            Math.Min(options.MaxLength, maxLengthPhase1)
        );

        if (resultWords.Count > 0) {
            return resultWords;
        }

        // --- PHASE 2: Fallback (rackWord only) ---
        Console.WriteLine($"\n--- Starting Phase 2 (Fallback): {rackWord} only ---");

        // Only use letters from the rack for the pool, and reset required counts.
        totalPool = GetLetterCounts(rackWord);
        _originalRequiredCounts = new Dictionary<char, int>(); // Not needed for fallback

        FindWordsRecursive(
            "",
            totalPool,
            options,
            resultWords,
            true,
            options.MaxLength,
            minNumberLetters
        );

        return resultWords;
    }

    /// <summary>
    ///     Recursive function to find words using backtracking.
    /// </summary>
    private void FindWordsRecursive(
        string currentWord,
        Dictionary<char, int> remainingCounts,
        GenerationOptions options,
        List<string> result,
        bool isFallback,
        int maxLength,
        int minLength = 1) {
        // 0. STOP CONDITIONS
        if (result.Count >= options.MaxWordsToCreate || (options.StopAfterFirstWord && result.Count > 0)) {
            return;
        }

        // 1. VALIDATION CHECK (Word found)
        if (currentWord.Length >= minLength && _dictionary.IsValidWord(currentWord)) {
            var meetsPhase1Constraints = true;

            if (!isFallback) {
                // Constraint A: Must contain ALL letters from originalWord (multiset check).
                if (!WordContainsRequiredLetters(currentWord, _originalRequiredCounts)) {
                    meetsPhase1Constraints = false;
                }

                // Constraint B: Must contain at least one letter that was ONLY from the rack.
                if (meetsPhase1Constraints &&
                    !WordUsedRackLetter(currentWord, _originalRequiredCounts, _rackOnlyCounts)) {
                    meetsPhase1Constraints = false;
                }
            }

            if (meetsPhase1Constraints) {
                result.Add(currentWord);
                Console.WriteLine($"Found: {currentWord}");
                if (options.StopAfterFirstWord) return;
            }
        }

        // Max length pruning
        if (currentWord.Length >= maxLength) {
            return;
        }

        // 2. PRUNING CHECK (Trie prefix lookup)
//        if (currentWord.Length > 0 && !_dictionary.IsHasWordsStartingWith(currentWord)) {
        if (currentWord.Length > 0 && _dictionary.IsEndOfTrie(currentWord)) {
            // Prune this entire branch if no word starts with the current prefix.
            return;
        }

        // 3. RECURSION (Try adding every available letter)
        foreach (var key in remainingCounts.Keys.ToList()) {
            if (remainingCounts[key] > 0) {
                // Use the letter
                remainingCounts[key]--;
                var nextWord = currentWord + key;

                // Recurse
                FindWordsRecursive(nextWord, remainingCounts, options, result, isFallback, maxLength, minLength);

                // Backtrack: put the letter back
                remainingCounts[key]++;
            }
        }
    }

    /// <summary>
    ///     Helper: Checks if a generated word contains the full multiset of letters from the original word.
    /// </summary>
    private bool WordContainsRequiredLetters(string word, Dictionary<char, int> requiredCounts) {
        var wordCounts = GetLetterCounts(word);
        foreach (var kvp in requiredCounts) {
            if (!wordCounts.ContainsKey(kvp.Key) || wordCounts[kvp.Key] < kvp.Value) {
                return false; // Missing a required letter or not enough of it.
            }
        }

        return true;
    }

    /// <summary>
    ///     Helper: Checks if at least one letter was used from the rack pool that was NOT required by the original word.
    ///     This satisfies the "must contain at least one letter that is in rackword" constraint by checking if we used any
    ///     letters beyond what was needed for the original word, taken from the combined pool.
    /// </summary>
    private bool WordUsedRackLetter(string word, Dictionary<char, int> originalRequiredCounts,
        Dictionary<char, int> rackOnlyCounts) {
        var wordCounts = GetLetterCounts(word);

        // Check if any letter was used more times than required by originalWord.
        foreach (var kvp in wordCounts) {
            var required = originalRequiredCounts.GetValueOrDefault(kvp.Key, 0);
            var rackContribution = rackOnlyCounts.GetValueOrDefault(kvp.Key, 0);

            // If the word used more of a letter than was required by the original word, 
            // and the rack had that letter available, then a rack letter was used.
            if (kvp.Value > required && rackContribution > 0) {
                return true;
            }
        }

        // Another way to check: if the word contains a letter that was only in the rack and not the original word.
        // Since the total pool is original + rack, any character in the word must have come from one of them.
        // We only need to check if the length of the word > length of original word OR if
        // the excess letters used (word counts - original counts) are present in the rack.

        if (word.Length > _originalRequiredCounts.Values.Sum()) {
            // If the word is longer than the required letters, at least one rack letter MUST have been used.
            return true;
        }

        // If the word is the same length as the original, check if the composition is different (meaning rack letters substituted/extended).
        // Since the DFS uses the combined pool, if the word is longer, a rack letter was definitely used. 
        // If the word is the same length, we rely on the length check or the letter count comparison above.
        // The first check (kvp.Value > required) is the most robust way to prove a rack letter was used beyond the original requirement.
        return false;
    }


    /// <summary>
    ///     Helper: Creates a dictionary of character counts from a string.
    /// </summary>
    private Dictionary<char, int> GetLetterCounts(string s) {
        var counts = new Dictionary<char, int>();
        foreach (var c in s) {
            counts[c] = counts.GetValueOrDefault(c, 0) + 1;
        }

        return counts;
    }

    /// <summary>
    ///     Mock implementation of a Trie-based dictionary for demonstration.
    ///     In a real application, this class would be highly optimized for prefix checking.
    /// </summary>
    public class TrieDictionary {
        // A simplified in-memory dictionary for testing.
        private readonly HashSet<string> _words = new(StringComparer.OrdinalIgnoreCase) {
            "CASTLE", "CASTLEROCK", "CASTING", "ACTOR", "CARTS", "CLEAT", "CATS", "RACK", "ROCK", "CARE", "RACE", "ATE",
            "ARE"
        };

        /// <summary>
        ///     Checks if a string is a complete, valid word.
        /// </summary>
        public bool IsWord(string word) {
            return _words.Contains(word);
        }

        /// <summary>
        ///     Checks if any word in the dictionary starts with the given prefix.
        ///     This is crucial for pruning the search space.
        /// </summary>
        public bool HasWordsStartingWith(string prefix) {
            // Simulate Trie lookup: check if any stored word starts with the prefix.
            return _words.Any(w => w.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    ///     Options structure to control the word generation process.
    /// </summary>
    public class GenerationOptions {
        public int MaxWordsToCreate { get; set; } = int.MaxValue;
        public int MaxLength { get; set; } = int.MaxValue;
        public bool StopAfterFirstWord { get; set; } = false;
        public int MaxLettersToAdd { get; set; } = int.MaxValue;
    }
}