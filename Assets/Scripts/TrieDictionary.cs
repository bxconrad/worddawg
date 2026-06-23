using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TrieDictionary : ITrieDictionary {
    private readonly TrieNode dictionaryTrie = new();
    private string[] allWords;

    public string
        dictionaryFilePath =
            "C:/Users/bacon/Downloads/dictionary-EN.txt"; // Make the path configurable in the Unity Inspector

    private bool isDictionaryRead;

    public bool HasPrefix(string prefix) {
        return !IsEndOfTrie(prefix);
    }

    public bool Contains(string contents) {
        return IsValidWord(contents);
    }

    //  private string language { get; set; }

    public void Initialize() {
        Initialize("EN");
    }

    public void Initialize(string lang) {
        var dictionaryName = "dictionary-" + lang;
        MonoBehaviour.print("~TrieDictionary.InitializeWord " + dictionaryName + "\n");
        var textFile = Resources.Load(dictionaryName) as TextAsset;
        allWords = textFile.text.Split();
        LoadDictionary(allWords);
        MonoBehaviour.print("~TrieDictionary.InitializeWord allWords " + allWords.Length + " dictionary " +
                            dictionaryName +
                            "\n");
    }

    public void LoadDictionary() {
        LoadDictionary("C:/Users/bacon/Downloads/dictionary-EN.txt");
    }

    public void LoadDictionary(string[] allWords) {
        Debug.Log("~TrieDictionary.LoadDictionary ");
        foreach (var word in allWords) {
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

            Debug.Log($"~DictionaryTrie count {count}");

            isDictionaryRead = true;
        }
        catch (FileNotFoundException) {
            Debug.LogError($"~TrieDictionary.LoadDictionary Error:  file not found at {filePath}");
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
    // return true if word is found in trieDictionary
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