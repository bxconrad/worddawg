using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// --- 1. TRIE DATA STRUCTURE DEFINITION ---

/// <summary>
///     Represents a single node in the Trie (Prefix Tree).
/// </summary>
public class TrieNode {
    public Dictionary<char, TrieNode> Children { get; } = new();
    public bool IsEndOfWord { get; set; }
}

/// <summary>
///     Manages the Trie structure for dictionary lookups.
/// </summary>
public class Trie {
    public TrieNode Root { get; } = new();

    /// <summary>
    ///     Adds a word to the Trie.
    /// </summary>
    public void Insert(string word) {
        var current = Root;
        foreach (var c in word) {
            if (!current.Children.ContainsKey(c)) {
                current.Children[c] = new TrieNode();
            }

            current = current.Children[c];
        }

        current.IsEndOfWord = true;
    }
}

// --- 2. MAIN WORD COMBINATOR LOGIC ---

public class EnhancedWordCombinator {
    private readonly Trie _dictionary;

    public EnhancedWordCombinator() {
        _dictionary = new Trie();
        string[] sampleWords = { "CART", "CARGO", "CRAM", "CAT", "CARTO", "ACT", "ARCTIC", "ACTOR" };
        foreach (var word in sampleWords) {
            Debug.Log("XXXinsert " + word);
            _dictionary.Insert(word);
        }
    }

    /// <summary>
    ///     Generates all valid combinations, pruned by the Trie dictionary.
    /// </summary>
    /// <param name="originalWord">The mandatory set of letters.</param>
    /// <param name="rackWord">The optional pool of letters to choose at least one from.</param>
    /// <returns>A list of all unique, valid combination strings found in the dictionary.</returns>
    public List<string> GenerateCombinations(string originalWord, string rackWord) {
        Debug.Log("XXXFindAllWords ---originalWord " + originalWord + " rackWord " + rackWord + "\n");
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

        Debug.Log("XXXFindAllWords --- " + finalCombinations.ToList().Count + "\n");

        foreach (var word in finalCombinations.ToList()) {
            Debug.Log("XXXFindAllWords --- " + word + "\n");
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
        Debug.Log("XXXdic" + _dictionary);
        Debug.Log("XXXdicr" + _dictionary.Root);
        // Start the backtracking process from the Trie Root.
        GeneratePermutationsBacktrack(
            letterCounts,
            desiredLength,
            new StringBuilder(),
            results,
            _dictionary.Root
        );
    }

    /// <summary>
    ///     Recursive backtracking function that uses the Trie for pruning.
    /// </summary>
    private void GeneratePermutationsBacktrack(
        Dictionary<char, int> counts,
        int length,
        StringBuilder currentPrefix,
        HashSet<string> results,
        TrieNode currentNode) {
        Debug.Log("XXXGeneratePermutationsBacktrack --- cp {" + currentPrefix + "} le " + length + "\n");
        // Base Case: If the combination reaches the desired length, and it's a valid word endpoint, save it.
        // We only save words that are in the dictionary (IsEndOfWord is true).
        if (currentPrefix.Length == length) {
            if (currentNode.IsEndOfWord) {
                results.Add(currentPrefix.ToString());
            }

            Debug.Log("XXXGeneratePermutationsBacktrack return \n");
            // Even if the desired length is reached, we return, as we can't add more letters.
            return;
        }

        // Recursive Step: Iterate through all available unique letters.
        foreach (var letter in counts.Keys.ToList()) {
            Debug.Log("XXXGeneratePermutationsBacktrack ltr " + letter + " counts[letter] " + counts[letter] + "\n");
            if (counts[letter] > 0) {
                // Pruning Check: If the next letter does not form a valid prefix, stop this branch.
                if (currentNode.Children.TryGetValue(letter, out var nextNode)) {
                    Debug.Log("XXXGeneratePermutationsBacktrack not eot  cp {" + currentPrefix + "} ltr {" + letter +
                              "}\n");
                    // 1. CHOOSE
                    counts[letter]--;
                    currentPrefix.Append(letter);

                    // 2. EXPLORE
                    Debug.Log("XXXGeneratePermutationsBacktrack not eot- recurse " + currentPrefix + " \n");
                    GeneratePermutationsBacktrack(counts, length, currentPrefix, results, nextNode);

                    // 3. UNCHOOSE (Backtrack)
                    currentPrefix.Length--;
                    counts[letter]++;
                    Debug.Log("XXXGeneratePermutationsBacktrack not eot- after bt " + currentPrefix + " \n");
                }
                // If TryGetValue fails, this means 'letter' is not a valid continuation of the prefix
                // in the dictionary, so the 'if' block is skipped and the branch is pruned.
            }
        }
    }
}

// --- 3. EXECUTION AND DEMONSTRATION ---
/*
public class Program {
    public static void Main(string[] args) {
        // Create a sample dictionary (Trie)
        var dictionary = new Trie();
        string[] sampleWords = { "CART", "CARGO", "CRAM", "CAT", "CARTO", "ACT", "ARCTIC", "ACTOR" };
        foreach (var word in sampleWords) {
            dictionary.Insert(word);
        }

        // Example Input
        var originalWord = "CAT";
        var rackWord = "OR";

        // Potential combinations:
        // Subset 'O': Letters C, A, T, O -> CAT (Not allowed, must use all letters) - Wait, the length must be 4.
        //   - Words of length 4 from C, A, T, O: CARO (If R was used), CATO (Not in dictionary), COAT (Not in dictionary)
        // Subset 'R': Letters C, A, T, R -> CART
        //   - Words of length 4 from C, A, T, R: CART, TARC, RACT, etc. (Only CART is in the dictionary)
        // Subset 'OR': Letters C, A, T, O, R -> CARTO
        //   - Words of length 5 from C, A, T, O, R: CARTO, ACTOR, TROCA, etc. (CARTO, ACTOR are in dictionary)

        Console.WriteLine("--- Trie-Pruned Word Combinator ---");
        Console.WriteLine("");
        Console.WriteLine($"Original Word (Mandatory): {originalWord}");
        Console.WriteLine($"Rack Word (Optional): {rackWord}");
        Console.WriteLine("---------------------------------------------");

        var generator = new EnhancedWordCombinator(dictionary);
        var combinations = generator.GenerateCombinations(originalWord, rackWord);

        Console.WriteLine($"Found {combinations.Count} valid words in the dictionary:");

        // Display results
        foreach (var combination in combinations.OrderBy(c => c)) {
            Console.WriteLine(combination);
        }

        Console.WriteLine("---------------------------------------------");

        // Example 2: No valid words
        originalWord = "XYZ";
        rackWord = "QW";

        Console.WriteLine($"Original Word (Mandatory): {originalWord}");
        Console.WriteLine($"Rack Word (Optional): {rackWord}");
        Console.WriteLine("---------------------------------------------");

        combinations = generator.GenerateCombinations(originalWord, rackWord);
        Console.WriteLine($"Found {combinations.Count} valid words in the dictionary:");

        // Display results
        foreach (var combination in combinations.OrderBy(c => c)) {
            Console.WriteLine(combination);
        }

        Console.WriteLine("---------------------------------------------");
  }  }
*/