using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DicionaryTrie {
    private readonly TrieNode dictionaryTrie = new();
    private string[] allWords;

    public string
        dictionaryFilePath =
            "C:/Users/bacon/Downloads/dictionary-EN.txt"; // Make the path configurable in the Unity Inspector

    private bool isDictionaryRead;

    //  private string language { get; set; }

    public void Initialize() {
        Initialize("EN");
    }

    public void Initialize(string lang) {
        var dictionaryName = "dictionary-" + lang;
        MonoBehaviour.print("DicionaryTrie.Initialize " + dictionaryName + "\n");
        var textFile = Resources.Load(dictionaryName) as TextAsset;
        allWords = textFile.text.Split();
        LoadDictionary(allWords);
        MonoBehaviour.print("DicionaryTrie.Initialize allWords " + allWords.Length + " dictionary " +
                            dictionaryName +
                            "\n");
    }

    public void LoadDictionary() {
        LoadDictionary("C:/Users/bacon/Downloads/dictionary-EN.txt");
    }

    public void LoadDictionary(string[] allWords) {
        foreach (var word in allWords) {
            Debug.Log("PXXXinsert " + word);
            InsertWord(word);
        }
    }

    public void LoadDictionary(string filePath) {
        if (isDictionaryRead) return;
        var count = 0;
        try {
            foreach (var word in File.ReadAllLines(filePath)) {
                InsertWord(word.ToUpper());
                count++;
            }

            Debug.Log($"Dictionary count {count}");

            isDictionaryRead = true;
        }
        catch (FileNotFoundException) {
            Debug.LogError($"Error: Dictionary file not found at {filePath}");
        }
    }

    private void InsertWord(string word) {
        var current = dictionaryTrie;
        foreach (var c in word) {
            if (!current.Children.ContainsKey(c)) {
                current.Children[c] = new TrieNode();
            }

            current = current.Children[c];
        }

        current.IsEndOfWord = true;
    }

    private bool IsPrefix(string prefix) {
        var current = dictionaryTrie;
        foreach (var c in prefix) {
            if (!current.Children.ContainsKey(c)) {
                return false;
            }

            current = current.Children[c];
        }

        return true;
    }

    public bool IsEndOfTrie(string prefix) {
        var current = dictionaryTrie;
        foreach (var c in prefix) {
            if (!current.Children.ContainsKey(c)) {
                return true;
            }

            current = current.Children[c];
        }

        return false;
    }

    // called by FindHighestScoringWordHelper.
    // return true if word is found in dictionaryTrie
    public bool IsValidWord(string word) {
        var current = dictionaryTrie;
        foreach (var c in word) {
            if (!current.Children.ContainsKey(c)) {
                return false;
            }

            current = current.Children[c];
        }

        return current.IsEndOfWord;
    }


    private class TrieNode {
        public Dictionary<char, TrieNode> Children { get; } = new();
        public bool IsEndOfWord { get; set; }
    }
}