using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PB3 {
    private static readonly Dictionary<char, int> LetterValues = new() {
        { 'A', 1 }, { 'E', 1 }, { 'I', 1 }, { 'L', 1 }, { 'N', 1 }, { 'O', 1 }, { 'R', 1 }, { 'S', 1 }, { 'T', 1 },
        { 'U', 1 },
        { 'D', 2 }, { 'G', 2 },
        { 'B', 3 }, { 'C', 3 }, { 'M', 3 }, { 'P', 3 },
        { 'F', 4 }, { 'H', 4 }, { 'V', 4 }, { 'W', 4 }, { 'Y', 4 },
        { 'K', 5 },
        { 'J', 8 }, { 'X', 8 },
        { 'Q', 10 }, { 'Z', 10 }
    };

    private readonly TrieNode _dictionaryTrie = new();

    public string
        dictionaryFilePath =
            "C:/Users/bacon/Downloads/dictionary-EN.txt"; // Make the path configurable in the Unity Inspector

    private bool isDictionaryRead;

    private void xStart() {
        if (string.IsNullOrEmpty(dictionaryFilePath)) {
            Debug.LogError("Dictionary File Path is not set in the Inspector!");
            return;
        }

        LoadDictionary(dictionaryFilePath);

        // Example usage (you can modify this for your testing)
        var wordList = new List<string> { "begrimmed" };
        var rack = new List<string> { "s", "f", "u", "o", "u", "a", "g" };

        var highestScoringWord = FindHighestScoringWord(wordList, rack);

        Debug.Log("Word List: " + string.Join(", ", wordList));
        Debug.Log("Rack: " + string.Join(", ", rack));
        Debug.Log($"Highest Scoring Word: {highestScoringWord ?? "No valid word found"}");

        if (highestScoringWord != null) {
            Debug.Log($"Score: {CalculateScore(highestScoringWord)}");
        }
    }

    private void LoadDictionary(string filePath) {
        if (isDictionaryRead) return;
        try {
            foreach (var word in File.ReadAllLines(filePath)) {
                InsertWord(word.ToUpper());
            }

            isDictionaryRead = true;
        }
        catch (FileNotFoundException) {
            Debug.LogError($"Error: Dictionary file not found at {filePath}");
        }
    }

    private void InsertWord(string word) {
        var current = _dictionaryTrie;
        foreach (var c in word) {
            if (!current.Children.ContainsKey(c)) {
                current.Children[c] = new TrieNode();
            }

            current = current.Children[c];
        }

        current.IsEndOfWord = true;
    }

    private bool IsPrefix(string prefix) {
        var current = _dictionaryTrie;
        foreach (var c in prefix) {
            if (!current.Children.ContainsKey(c)) {
                return false;
            }

            current = current.Children[c];
        }

        return true;
    }

    private bool IsValidWord(string word) {
        var current = _dictionaryTrie;
        foreach (var c in word) {
            if (!current.Children.ContainsKey(c)) {
                return false;
            }

            current = current.Children[c];
        }

        return current.IsEndOfWord;
    }

    private int CalculateScore(string word) {
        var score = 0;
        foreach (var c in word) {
            score += LetterValues.GetValueOrDefault(char.ToUpper(c), 0);
        }

        var multiplier = Mathf.Max(0, word.Length - 3); // Using Mathf.Max for Unity
        return score * multiplier;
    }

    private void FindHighestScoringWordHelper(char[] availableLetters, string currentWord,
        HashSet<(string Word, int Score)> validWordsWithScores) {
        if (IsValidWord(currentWord)) {
            validWordsWithScores.Add((currentWord, CalculateScore(currentWord)));
        }

        if (!IsPrefix(currentWord)) {
            return;
        }

        for (var i = 0; i < availableLetters.Length; i++) {
            var nextLetter = availableLetters[i];
            var remainingLetters = availableLetters.Where((c, index) => index != i).ToArray();
            FindHighestScoringWordHelper(remainingLetters, currentWord + nextLetter, validWordsWithScores);
        }
    }

    public string FindHighestScoringWord(List<string> wordList, List<string> rack) {
        LoadDictionary(dictionaryFilePath);
        Debug.Log("Word List: " + string.Join(", ", wordList));
        Debug.Log("Rack: " + string.Join(", ", rack));
        Debug.Log("Time: " + DateTime.Now + "\n");
        var start = DateTime.Now.Millisecond;

        string overallHighestScoringWord = null;
        var overallHighestScore = -1;
        var allValidWords = new HashSet<(string Word, int Score)>();

        foreach (var originalWord in wordList) {
            var originalWordUpper = originalWord.ToUpper();
            var originalWordChars = originalWordUpper.ToCharArray();
            var rackChars = rack.SelectMany(s => s.ToUpper().ToCharArray()).ToArray();
            var combinedLetters = originalWordChars.Concat(rackChars).ToArray();

            var validWordsForOriginal = new HashSet<(string Word, int Score)>();
            FindHighestScoringWordHelper(combinedLetters, "", validWordsForOriginal);

            string bestWordForCurrentOriginal = null;
            var bestScoreForCurrentOriginal = -1;
            var primaryGoalValidWords = new List<(string Word, int Score)>();

            Debug.Log($"--- Valid words formed using '{originalWord}' and rack letters ---\n");

            foreach (var (word, score) in validWordsForOriginal) {
                if (word.Length >= originalWordUpper.Length + 1) {
                    var usesAllOriginal = originalWordUpper.All(c =>
                        word.Count(wc => wc == c) >= originalWordUpper.Count(oc => oc == c));

                    if (usesAllOriginal) {
                        // Check if at least one letter in 'word' came from 'rack' (considering counts)
                        var usedRackLetter = false;
                        var originalCounts = originalWordUpper.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
                        var rackCounts = rackChars.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
                        var wordCounts = word.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

                        foreach (var kvp in wordCounts) {
                            var letter = kvp.Key;
                            var wordCount = kvp.Value;
                            var originalCount = originalCounts.GetValueOrDefault(letter, 0);
                            var rackAvailable = rackCounts.GetValueOrDefault(letter, 0);

                            if (wordCount > originalCount && rackAvailable > 0) {
                                usedRackLetter = true;
                                break;
                            }
                        }

                        if (usedRackLetter) {
                            var combinedLetterCounts =
                                combinedLetters.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
                            var possible = true;
                            foreach (var kvp in wordCounts) {
                                if (!combinedLetterCounts.ContainsKey(kvp.Key) ||
                                    kvp.Value > combinedLetterCounts[kvp.Key]) {
                                    possible = false;
                                    break;
                                }
                            }

                            if (possible) {
                                primaryGoalValidWords.Add((word, score));
                                Debug.Log($"Word: {word}, Score: {score}");
                                allValidWords.Add((word, score));

                                if (score > bestScoreForCurrentOriginal) {
                                    bestScoreForCurrentOriginal = score;
                                    bestWordForCurrentOriginal = word;
                                }
                            }
                        }
                    }
                }
            }

            if (bestScoreForCurrentOriginal > overallHighestScore) {
                overallHighestScore = bestScoreForCurrentOriginal;
                overallHighestScoringWord = bestWordForCurrentOriginal;
            }
            else if (bestScoreForCurrentOriginal == overallHighestScore && bestWordForCurrentOriginal != null &&
                     overallHighestScoringWord == null) {
                overallHighestScoringWord =
                    bestWordForCurrentOriginal; // In case the first valid word has the highest score so far
            }
        }

        // Secondary Goal remains the same, but will only be executed if no word met the primary goal across the entire word list
        if (overallHighestScoringWord == null) {
            Debug.Log("\n--- Valid words formed using only rack letters ---");
            var distinctRackChars = rack.SelectMany(s => s.ToUpper().ToCharArray()).Distinct().ToArray();
            var validRackWordsWithScores = new HashSet<(string Word, int Score)>();
            FindHighestScoringWordHelper(distinctRackChars, "", validRackWordsWithScores);

            foreach (var (word, score) in validRackWordsWithScores.Where(w => w.Word.Length >= 3)
                         .OrderByDescending(ws => ws.Score)) {
                Debug.Log($"Word: {word.ToUpper()}, Score: {score}");
                allValidWords.Add((word.ToUpper(), score));
                if (score > overallHighestScore) {
                    overallHighestScore = score;
                    overallHighestScoringWord = word.ToUpper();
                }
            }
        }

        var end1 = DateTime.Now.Millisecond;
        Debug.Log("End Time: " + DateTime.Now + "\n");
        var elapsed = DateTime.Now.Millisecond - start;
        Debug.Log("time ---" + elapsed + "\n");

        return overallHighestScoringWord;
    }

    private class TrieNode {
        public Dictionary<char, TrieNode> Children { get; } = new();
        public bool IsEndOfWord { get; set; }
    }
}