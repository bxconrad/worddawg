using UnityEngine;

public class ValidatorManager {
    public TrieDictionary trieDictionary;


    public string ValidateInputWord(SelectedWord selectedWord, BetterRack betterRack, string inputWord) {
        MonoBehaviour.print($"~ValidatorManager.ValidateInputWord {inputWord}\n");
        //return "TRUE";
        var displayWord = inputWord.Replace("#", "QU").Replace("*", "LL");
        // add 1  to length for Q or LL (spanish)
        var wordLength = displayWord.Length;

        if (wordLength < 3) {
            MonoBehaviour.print($"~ValidatorManager.ValidateInputWord is Length <3 {inputWord.Length} \n");
            return "Words must be at least 3 letters long";
        }

        // check all letters from selectedWord are used
        MonoBehaviour.print("~ValidatorManager.ValidateInputWord IsLength true\n");
        if (!selectedWord.isAllLettersUsed()) {
            MonoBehaviour.print("~ValidatorManager.ValidateInputWord IsAllLettersUsed  false \n");
            return "You must use all the letters in the selected word";
        }

        MonoBehaviour.print("~ValidatorManager.ValidateInputWord IsAllLettersUsed True\n");

        if (!betterRack.isOneLetterUsed()) {
            MonoBehaviour.print("~ValidatorManager.ValidateInputWord IsRackLettersUsed false\n");
            return "You must use at least one letter from the rack";
        }

        var letter = IsLettersContained(inputWord, selectedWord.GetWord(), betterRack.GetWord());
        if (letter != ' ') {
            MonoBehaviour.print($"~ValidatorManager.ValidateInputWord isLettersAvailable false letter {letter}" +
                                $"inputWord {inputWord} selectedWord {selectedWord.GetWord()} rack {betterRack.GetWord()} \n");
            return $"The letter {letter} is not in the selected word or the rack";
        }

        MonoBehaviour.print("~ValidatorManager.ValidateInputWord IsRackLettersUsed true\n");

        if (!IsInDictionary(inputWord)) {
            MonoBehaviour.print(
                $"~ValidatorManager.ValidateInputWord {inputWord} IsInDictionary  isValidNew false\n");
            return $"{displayWord} is not in the dictionary";
        }

        MonoBehaviour.print("~ValidatorManager.ValidateInputWord  isValidNew true \n");
        return "TRUE";
    }

//verify that all the letters in the selectedWord have been used in the inputWord and at least one from the rack
// if not, return the first letter that was not used. If so, return blank.
    private char IsLettersContained(string inputWordString, string selectedWordString, string letterRackWord) {
        MonoBehaviour.print(
            "~ValidatorManager.IsLettersContained sel {selectedWordString} letterRackWord {letterRackWord}");
        foreach (var letter in inputWordString) {
            // MonoBehaviour.print("~IsLettersContained {letter);
            var index = selectedWordString.IndexOf(letter);
            if (index >= 0) {
                selectedWordString = selectedWordString.Remove(index, 1);
                // MonoBehaviour.print("~ValidatorManager.IsLettersContained {index} selectedWordString {selectedWordString);
                continue;
            }

            index = letterRackWord.IndexOf(letter);
            if (index >= 0) {
                letterRackWord = letterRackWord.Remove(index, 1);
                // MonoBehaviour.print("~ValidatorManager.IsLettersContained {index} letterRackWord {letterRackWord);
                continue;
            }

            return letter;
        }

        return ' ';
    }

    private bool IsInDictionary(string word) {
        //MonoBehaviour.print($"~ValidatorManager.IsInDictionary word {word{\n");
        var retVal = trieDictionary.IsValidWord(word);

        MonoBehaviour.print($"~ValidatorManager.IsInDictionary {retVal}   word {word}\n");
        return retVal;
    }
}