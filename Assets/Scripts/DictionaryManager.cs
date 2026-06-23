using System;
using UnityEngine;

public class DictionaryManager : MonoBehaviour {
    private TrieDictionary currentDictionary;
    private string currentDictionaryName;

    public string[] BuildDictionaryDogWords() {
        //bcdo refactor to use trie
        var textFile = Resources.Load("dogwords") as TextAsset;
        var dogBonusWords = textFile.text.Split();
        return dogBonusWords;
    }

    public TrieDictionary BuildDictionary(GameParameters gameParameters) {
        currentDictionaryName = "dictionary-" + gameParameters.language;
        currentDictionary = BuildDictionaryTrie(currentDictionaryName);
        return currentDictionary;
    }

    public TrieDictionary BuildDictionaryForBot(GameParameters gameParameters) {
        var botDictionaryName = GetDictionaryNameForBot(gameParameters);
        if (botDictionaryName.Equals(currentDictionaryName)) {
            print($"~DictionaryManager.BuildDictionaryForBot return currentDictionary {currentDictionary}\n");
            return currentDictionary;
        }

        return BuildDictionaryTrie(botDictionaryName);
    }


    private TrieDictionary BuildDictionaryTrie(string dictionaryName) {
        print($"~DictionaryManager.BuildDictionaryTrie {dictionaryName}\n");
        var dictionaryTrie = new TrieDictionary();
        var textFile1 = Resources.Load(dictionaryName) as TextAsset;
        var words = textFile1.text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        print($"~DictionaryManager.BuildDictionaryTrie {dictionaryName} # {words.Length}\n");
        dictionaryTrie.LoadDictionary(words);
        return dictionaryTrie;
    }

    private string GetDictionaryNameForBot(GameParameters gameParameters) {
        var specialDictionaryName = "";
        if (gameParameters.botLevel <= 1 && MyPrefs.PREFS_LANG_EN.Equals(gameParameters.language)) {
            specialDictionaryName = "dictionaryTiny-" + gameParameters.language;
        }
        else if (gameParameters.botLevel < 4 && MyPrefs.PREFS_LANG_EN.Equals(gameParameters.language)) {
            specialDictionaryName = "dictionarySmall-" + gameParameters.language;
        }

        var resource = Resources.Load(specialDictionaryName);
        var botDictionaryName = resource != null ? specialDictionaryName : "dictionary-" + gameParameters.language;

        print($"~UpdateBoardBot.BuildDictionaries  botDictionaryName {botDictionaryName} \n");
        return botDictionaryName;
    }

    // public void BuildDictionaries() {
    //     //bcdo refactor to dictionaryHandler?
    //     if (gameParameters.language.Equals(saveLanguage)) {
    //         return;
    //     }
    //
    //     var dictionaryName = "dictionary-" + gameParameters.language;
    //               var dictionaryTrie = BuildDictionaryTrie(dictionaryName);
    //         //validatorManager.trieDictionary = dictionaryTrie;
    //     
    //
    //     var textFile = Resources.Load("dogwords") as TextAsset;
    //     var dogBonusWords = textFile.text.Split();
    //     //scoreCalculator.dogBonusWords = dogBonusWords;
    //     MonoBehaviour.print($"~DictionaryManager.BuildDictionaries dogBonusWords {dogBonusWords.Length} \n");
    //
    //     MonoBehaviour.print($"~DictionaryManager.BuildDictionaries dictionaryName {dictionaryName} \n");
    //     saveLanguage = gameParameters.language;
    // }
}