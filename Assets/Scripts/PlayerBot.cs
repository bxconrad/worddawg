using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerBot {
    private readonly Trie _scrabbleDictionaryTrie = new();

    public PlayerBot() {
        var result = LoadDictionary("dictionary-EN.txt");
    }

    private string LoadDictionary(string filePath) {
        var fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads",
            filePath);
        try {
            var words = File.ReadAllLines(fullPath);
            foreach (var word in words) {
                _scrabbleDictionaryTrie.Insert(word.ToUpper());
            }

            MonoBehaviour.print($"Successfully loaded {words.Length} words from the dictionary.");
        }
        catch (FileNotFoundException) {
            MonoBehaviour.print($"Error: Dictionary file not found at {fullPath}");
            Environment.Exit(1);
        }
        catch (Exception ex) {
            MonoBehaviour.print($"An error occurred while loading the dictionary: {ex.Message}");
            return "ERROR";
        }

        return "SUCCESS";
    }

    public string FindValidWord(List<string> wordList, List<char> rack) {
        MonoBehaviour.print("FindValidWord");
        // var result = LoadDictionary("dictionary-EN.txt");
        // if ("ERROR".Equals(result)) return result;
        foreach (var originalWord in wordList) {
            var validWord = FindValidCombinedWord(originalWord, rack);
            if (validWord != null) {
                return validWord;
            }
        }

        return FindValidRackOnlyWord(rack);
    }

    private string FindValidCombinedWord(string originalWord, List<char> rack) {
        var originalLetters = originalWord.ToUpper().ToList();

        for (var i = 1; i <= rack.Count; i++) {
            var rackLetterCombinations = GetCombinations(rack.Select(c => char.ToUpper(c)).ToList(), i);

            foreach (var rackCombination in rackLetterCombinations) {
                var combinedLetters = originalLetters.Concat(rackCombination).ToList();
                var validPermutation = FindValidPermutationWithTrie(combinedLetters);
                if (validPermutation != null) {
                    return validPermutation;
                }
            }
        }

        return null;
    }

    private string FindValidRackOnlyWord(List<char> rack) {
        if (rack.Count < 3) {
            return null;
        }

        return FindValidPermutationWithTrie(rack.Select(c => char.ToUpper(c)).ToList(), 3);
    }

    private string FindValidPermutationWithTrie(List<char> letters, int minLength = 0) {
        var permutations = new HashSet<string>();
        GeneratePermutationsWithTrie(letters, 0, "", permutations);

        foreach (var permutation in permutations) {
            if (permutation.Length >= minLength && _scrabbleDictionaryTrie.Search(permutation)) {
                return permutation;
            }
        }

        return null;
    }

    private void GeneratePermutationsWithTrie(List<char> elements, int k, string currentPrefix,
        HashSet<string> permutations) {
        if (!_scrabbleDictionaryTrie.StartsWith(currentPrefix)) {
            return; // Optimization: Prune if the current prefix is not valid
        }

        if (k == elements.Count) {
            permutations.Add(currentPrefix);
        }
        else {
            for (var i = k; i < elements.Count; i++) {
                Swap(elements, k, i);
                GeneratePermutationsWithTrie(elements, k + 1, currentPrefix + elements[k], permutations);
                Swap(elements, k, i); // Backtrack
            }
        }
    }

    private List<List<char>> GetCombinations<T>(List<T> list, int k) {
        if (k < 0 || k > list.Count) {
            return new List<List<char>>();
        }

        if (k == 0) {
            return new List<List<char>> { new() };
        }

        if (k == list.Count) {
            return new List<List<char>> { list.Cast<char>().ToList() };
        }

        var combinations = new List<List<char>>();
        GetCombinations(list, k, 0, new List<char>(), combinations);
        return combinations;
    }

    private void GetCombinations<T>(List<T> list, int k, int start, List<char> currentCombination,
        List<List<char>> combinations) {
        if (currentCombination.Count == k) {
            combinations.Add(new List<char>(currentCombination));
            return;
        }

        for (var i = start; i < list.Count; i++) {
            currentCombination.Add((char)(object)list[i]);
            GetCombinations(list, k, i + 1, currentCombination, combinations);
            currentCombination.RemoveAt(currentCombination.Count - 1);
        }
    }

    private void Swap<T>(List<T> list, int index1, int index2) {
        (list[index1], list[index2]) = (list[index2], list[index1]);
    }

    public static void Main(string[] args) {
        // Example Usage:
        var game = new PlayerBot();
        var wordList1 = new List<string> { "ape", "ink", "tea" };
        var rack1 = new List<char> { 's', 't', 'n', 'k' };
        var result1 = game.FindValidWord(wordList1, rack1);
        MonoBehaviour.print($"Result 1: {result1 ?? "No valid word found."}");

        var wordList2 = new List<string> { "cat", "pen" };
        var rack2 = new List<char> { 'g', 'o', 'z' };
        var result2 = game.FindValidWord(wordList2, rack2);
        MonoBehaviour.print($"Result 2: {result2 ?? "No valid word found."}");
    }

    // Trie Data Structure for Prefix Optimization
    private class TrieNode {
        public readonly TrieNode[] Children = new TrieNode[26];
        public bool IsEndOfWord;

        public TrieNode() {
            IsEndOfWord = false;
            for (var i = 0; i < 26; i++) {
                Children[i] = null;
            }
        }
    }

    private class Trie {
        private readonly TrieNode _root;

        public Trie() {
            _root = new TrieNode();
        }

        public void Insert(string word) {
            var currentNode = _root;
            for (var i = 0; i < word.Length; i++) {
                var index = word[i] - 'A';
                if (currentNode.Children[index] == null) {
                    currentNode.Children[index] = new TrieNode();
                }

                currentNode = currentNode.Children[index];
            }

            currentNode.IsEndOfWord = true;
        }

        public bool Search(string word) {
            var currentNode = _root;
            for (var i = 0; i < word.Length; i++) {
                var index = word[i] - 'A';
                if (currentNode.Children[index] == null) {
                    return false;
                }

                currentNode = currentNode.Children[index];
            }

            return currentNode != null && currentNode.IsEndOfWord;
        }

        public bool StartsWith(string prefix) {
            var currentNode = _root;
            for (var i = 0; i < prefix.Length; i++) {
                var index = prefix[i] - 'A';
                if (currentNode.Children[index] == null) {
                    return false;
                }

                currentNode = currentNode.Children[index];
            }

            return true;
        }
    }
}