using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// --- 2. MAIN WORD COMBINATOR LOGIC ---
public class PB4 {
    private readonly DicionaryTrie _dictionary = new();

    /// <summary>
    ///     Generates all valid combinations, pruned by the Trie dictionary.
    /// </summary>
    /// <param name="originalWord">The mandatory set of letters.</param>
    /// <param name="rackWord">The optional pool of letters to choose at least one from.</param>
    /// <returns>A list of all unique, valid combination strings found in the dictionary.</returns>
    public List<string> GenerateCombinations(string originalWord, string rackWord) {
        Debug.Log("PXXFindAllWords ---originalWord " + originalWord + " rackWord " + rackWord + "\n");
        string[] sampleWords = { "CART", "CARGO", "CRAM", "CAT", "CARTO", "ACT", "ARCTIC", "ACTOR" };
        _dictionary.LoadDictionary(sampleWords);
        var b = _dictionary.IsValidWord("CART");
        Debug.Log("PXXGenerateCombinations --- cart?" + b + "\n");
        var rackLetters = rackWord.ToCharArray();
        var rackLength = rackLetters.Length;

        // Use a HashSet to store all unique final word combinations.
        var finalCombinations = new HashSet<string>();

        // Iterate through all possible non-empty subsets of rackWord using a bitmask.
        // This ensures the constraint: "Each combination must contain at least one letter that is in rackword"
        for (var i = 1; i < 1 << rackLength; i++) {
            var currentRackSubset = new StringBuilder();
            for (var j = 0; j < rackLength; j++) {
                if ((i & (1 << j)) != 0) {
                    currentRackSubset.Append(rackLetters[j]);
                }
            }

            // Combine mandatory letters with the current rack subset.
            var combinedLetters = originalWord + currentRackSubset;

            // Generate permutations for this combined set, using the Trie for pruning.
            GenerateTriePrunedPermutations(combinedLetters, finalCombinations);
        }

        Debug.Log("PXXGenerateCombinations --- " + finalCombinations.Count + "\n");
        foreach (var word in finalCombinations.ToList()) {
            Debug.Log("PXXGenerateCombinations --- " + word + "\n");
        }

        return finalCombinations.ToList();
    }

    /// <summary>
    ///     Helper to set up the letter counts and start the recursive backtracking.
    /// </summary>
    private void GenerateTriePrunedPermutations(string letters, HashSet<string> results) {
        // 1. Count the frequency of each unique letter.
        var letterCounts = new Dictionary<char, int>();
        foreach (var c in letters) {
            if (letterCounts.ContainsKey(c)) {
                letterCounts[c]++;
            }
            else {
                letterCounts[c] = 1;
            }
        }

        var desiredLength = letters.Length;

        // Start the backtracking process from the Trie Root.
        GeneratePermutationsBacktrack(
            letterCounts,
            desiredLength,
            new StringBuilder(),
            results
        );
    }

    /// <summary>
    ///     Recursive backtracking function that uses the Trie for pruning.
    /// </summary>
    private void GeneratePermutationsBacktrack(
        Dictionary<char, int> counts,
        int length,
        StringBuilder currentPrefix,
        HashSet<string> results
    ) {
        Debug.Log("PXXGeneratePermutationsBacktrack --- cp {" + currentPrefix + "} le " + length + "\n");
        // Base Case: If the combination reaches the desired length, and it's a valid word endpoint, save it.
        // We only save words that are in the dictionary (IsEndOfWord is true).
        if (currentPrefix.Length == length) {
            if (_dictionary.IsValidWord(currentPrefix.ToString())) {
                results.Add(currentPrefix.ToString());
                Debug.Log("PXXGeneratePermutationsBacktrack validWord " + currentPrefix + " \n");
            }

            // Even if the desired length is reached, we return, as we can't add more letters.
            return;
        }

        // Recursive Step: Iterate through all available unique letters.
        foreach (var letter in counts.Keys.ToList()) {
            Debug.Log("PXXGeneratePermutationsBacktrack ltr " + letter + " counts[letter] " + counts[letter] + "\n");
            if (counts[letter] > 0) {
                // Pruning Check: If the next letter does not form a valid prefix, stop this branch.
                var testPrefix = currentPrefix.ToString();
                testPrefix.Append(letter);
                if (!_dictionary.IsEndOfTrie(testPrefix)) {
                    Debug.Log("PXXGeneratePermutationsBacktrack not eot \n");
                    //            if (currentNode.Children.TryGetValue(letter, out var nextNode)) {
                    // 1. CHOOSE
                    counts[letter]--;
                    currentPrefix.Append(letter);

                    // 2. EXPLORE
                    Debug.Log("PXXGeneratePermutationsBacktrack not eot- recurse " + currentPrefix + " \n");
                    GeneratePermutationsBacktrack(counts, length, currentPrefix, results);

                    // 3. UNCHOOSE (Backtrack)
                    currentPrefix.Length--;
                    counts[letter]++;
                    Debug.Log("PXXGeneratePermutationsBacktrack not eot- after bt " + currentPrefix + " \n");
                }
                // If TryGetValue fails, this means 'letter' is not a valid continuation of the prefix
                // in the dictionary, so the 'if' block is skipped and the branch is pruned.
            }
        }
    }
}