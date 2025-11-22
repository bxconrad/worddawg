using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrucesBot {
    private readonly BrucesDicionaryTrie brucesDictionaryTrie = new();
    private readonly WordChecker wordChecker = new();


    // called by highest scoringWord with empty currentWord, then recursively here
    private void FindAllWordsForEachWord(char[] availableLetters, string currentWord, List<string> validWords,
        string originalWord, string rackWord) {
        //  Debug.Log("FIndAllWordsForEachWord currentWord " + currentWord + "avl " + string.Join("", availableLetters));

        // checks for original word AFTER making list. possibly better to check that first and short circuit the checking... maybe not
        if (brucesDictionaryTrie.IsValidWord(currentWord)) {
            if (wordChecker.CanFormNewWord(originalWord, currentWord, rackWord)) {
                //  if (currentWord.Length >= originalWordUpper.Length + 1) {
                validWords.Add(currentWord);
            }
        }

        if (brucesDictionaryTrie.IsEndOfTrie(currentWord)) {
            //  Debug.Log("FindHighestScoringWordHelper  IsEndOfTrie return  " + currentWord);
            return;
        }
        //   }

        // create every possible word from the letters. 
        // could check for dup first letter here. could set min word length
        for (var i = 0; i < availableLetters.Length; i++) {
            var nextLetter = availableLetters[i];
            var remainingLetters = availableLetters.Where((c, index) => index != i).ToArray();
            //   Debug.Log("FindHighestScoringWordHelper recurse " + currentWord + nextLetter);

            FindAllWordsForEachWord(remainingLetters, currentWord + nextLetter, validWords, originalWord,
                rackWord);
        }
    }


    public string FindAllWords(List<string> wordList, List<string> rack) {
        var start = DateTime.Now.Millisecond;
        brucesDictionaryTrie.LoadDictionary("C:/Users/bacon/Downloads/dictionary-EN.txt");
        Debug.Log("Word List: " + string.Join(", ", wordList));
        Debug.Log("Rack: " + string.Join(", ", rack));
        Debug.Log("Time: " + DateTime.Now + "\n");
        var rackWord = string.Join(", ", rack);

        string overallHighestScoringWord = null;
        // finds every possible combination
        var rackChars = rack.SelectMany(s => s.ToUpper().ToCharArray()).ToArray();
        var validWordsForOriginal = new List<string>();
        foreach (var originalWord in wordList) {
            Debug.Log("FindAllWords --- " + originalWord + "\n");
            var originalWordChars = originalWord.ToUpper().ToCharArray();
            var combinedLetters = originalWordChars.Concat(rackChars).ToArray();

            FindAllWordsForEachWord(combinedLetters, "", validWordsForOriginal, originalWord.ToUpper(),
                rackWord);


            Debug.Log($"--- Valid words formed using '{originalWord}' and rack letters ---\n");
        }

        foreach (var word in validWordsForOriginal) {
            Debug.Log("FindAllWords --- " + word + "\n");
        }

        Debug.Log("End Time: " + DateTime.Now + "\n");
        var elapsed = DateTime.Now.Millisecond - start;
        Debug.Log("elapsed ms ---" + elapsed + "\n");

        return overallHighestScoringWord;
    }
}