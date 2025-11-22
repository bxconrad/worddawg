using System.Collections.Generic;

/// <summary>
///     Contains the core logic for checking if a new word can be formed
///     from an original word, with the remaining letters contained in a rack word.
/// </summary>
public class WordChecker {
    /// <summary>
    ///     Helper function to create a frequency map of characters in a word, ignoring case.
    /// </summary>
    /// <param name="word">The input word.</param>
    /// <returns>A dictionary mapping lowercase characters to their counts.</returns>
    private Dictionary<char, int> GetCharCounts(string word) {
        var counts = new Dictionary<char, int>();
        // Normalize the word to lowercase and iterate through characters
        foreach (var c in word.ToLowerInvariant()) {
            if (char.IsLetter(c)) {
                if (counts.ContainsKey(c)) {
                    counts[c]++;
                }
                else {
                    counts.Add(c, 1);
                }
            }
        }

        return counts;
    }

    /// <summary>
    ///     Determines if a 'new word' can be created from an 'original word'
    ///     using only the letters available in the 'original word' itself plus the 'rack word'.
    /// </summary>
    /// <param name="originalWord">The base word whose letters must be present in the new word.</param>
    /// <param name="newWord">The target word to check.</param>
    /// <param name="rackWord">The word providing the remaining letters.</param>
    /// <returns>True if the rule is satisfied, otherwise False.</returns>
    public bool CanFormNewWord(string originalWord, string newWord, string rackWord) {
        // 1. Get frequency maps (all words normalized to lowercase)
        var originalCounts = GetCharCounts(originalWord);
        var newCounts = GetCharCounts(newWord);
        var rackCounts = GetCharCounts(rackWord);

        // --- Step 1: Check if all letters from ORIGINAL WORD are present in NEW WORD ---

        // This is necessary because newWord must contain *at least* the frequency of every letter in originalWord.
        foreach (var kvp in originalCounts) {
            var c = kvp.Key;
            var requiredCount = kvp.Value;

            // Check if the new word even contains the letter, and if the count is sufficient.
            if (!newCounts.TryGetValue(c, out var actualCount) || actualCount < requiredCount) {
                // Failed: New word is missing a required letter or doesn't have enough instances.
                return false;
            }
        }

        // --- Step 2: Calculate the REMAINING letters in NEW WORD ---

        // These are the letters in newWord that were not required by originalWord.
        var remainingCounts = new Dictionary<char, int>();

        foreach (var kvp in newCounts) {
            var c = kvp.Key;
            var newCount = kvp.Value;

            // Get the count required by the original word (0 if not present in originalWord)
            var requiredByOriginal = originalCounts.GetValueOrDefault(c, 0);

            // The remaining count is the excess amount
            var remaining = newCount - requiredByOriginal;

            if (remaining > 0) {
                remainingCounts.Add(c, remaining);
            }
        }

        // --- Step 3: Check if all REMAINING letters are present in RACK WORD ---

        foreach (var kvp in remainingCounts) {
            var c = kvp.Key;
            var requiredRemainingCount = kvp.Value;

            // Check if the rack word contains the letter, and if the count is sufficient.
            if (!rackCounts.TryGetValue(c, out var actualRackCount) || actualRackCount < requiredRemainingCount) {
                // Failed: Rack word is missing a required remaining letter or doesn't have enough instances.
                return false;
            }
        }

        // All checks passed
        return true;
    }
}