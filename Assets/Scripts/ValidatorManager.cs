using UnityEngine;

public class ValidatorManager {
    private string[] allWords;
    private string language { get; set; }

    public void Initialize(string lang) {
        language = lang;
        var dictionaryName = "dictionary-EN"; // + lang;
        MonoBehaviour.print("ValidatorManager.Initialize " + dictionaryName + "\n");
        var textFile = Resources.Load(dictionaryName) as TextAsset;
        allWords = textFile.text.Split();

        MonoBehaviour.print("ValidatorManager.Initialize allWords " + allWords.Length + " dictionary " +
                            dictionaryName +
                            "\n");
    }

    public string ValidateInputWord(SelectedWord selectedWord, BetterRack betterRack, string inputWordString) {
        MonoBehaviour.print("ValidatorManager.ValidateInputWord {" + inputWordString + "}\n");
        //  return "TRUE";
        // qulogic
        var selectedWordString = GameHelper.ExpandDoubleLetter(selectedWord.GetWord(), language);
        var rackWordString = GameHelper.ExpandDoubleLetter(betterRack.GetWord(), language);
        inputWordString = GameHelper.ExpandDoubleLetter(inputWordString, language);
        // check all letters from selectedWord are used
        if (!selectedWord.isAllLettersUsed()) {
            MonoBehaviour.print("ValidatorManager.ValidateInputWord IsAllLettersUsed  false \n");
            return "You must use all the letters in the selected word";
        }

        MonoBehaviour.print("ValidatorManager.ValidateInputWord IsAllLettersUsed True\n");

        if (!betterRack.isOneLetterUsed()) {
            MonoBehaviour.print("ValidatorManager.ValidateInputWord IsRackLettersUsed false\n");
            return "You must use at least one letter from the rack";
        }

        var letter = IsLettersContained(inputWordString, selectedWordString, rackWordString);
        if (letter != ' ') {
            MonoBehaviour.print("ValidatorManager.ValidateInputWord isLettersAvailable false letter " + letter + " \n");
            return "The letter " + letter + " is not in the selected word or the rack";
        }

        MonoBehaviour.print("ValidatorManager.ValidateInputWord IsRackLettersUsed true\n");

        if (inputWordString.Length > 0 && inputWordString.Length < 3) {
            MonoBehaviour.print("ValidatorManager.ValidateInputWord is Length 3 " + inputWordString.Length + " \n");
            return "Words must be at least 3 letters long";
        }

        MonoBehaviour.print("ValidatorManager.ValidateInputWord IsLength true\n");

        if (!IsInDictionary(inputWordString)) {
            MonoBehaviour.print("ValidatorManager.ValidateInputWord " + inputWordString +
                                " IsInDictionary  isValidNew false\n");
            return inputWordString + " is not in the dictionary";
        }

        MonoBehaviour.print("ValidatorManager.IsInDictionary  isValidNew true \n");

        return "TRUE";
    }

    //verify that all the letters in the selectedWord have been used in the inputWord and at least one from the rack
    // if not, return the first letter that was not used. If so, return blank.
    private char IsLettersContained(string inputWordString, string selectedWordString, string letterRackWord) {
        MonoBehaviour.print("IsLettersContained sel " + selectedWordString + " letterRackWord " + letterRackWord);
        foreach (var letter in inputWordString) {
            // MonoBehaviour.print("IsLettersContained " + letter);
            var index = selectedWordString.IndexOf(letter);
            if (index >= 0) {
                selectedWordString = selectedWordString.Remove(index, 1);
                // MonoBehaviour.print("IsLettersContained " + index + " selectedWordString " + selectedWordString);
                continue;
            }

            index = letterRackWord.IndexOf(letter);
            if (index >= 0) {
                letterRackWord = letterRackWord.Remove(index, 1);
                // MonoBehaviour.print("IsLettersContained " + index + " letterRackWord " + letterRackWord);
                continue;
            }

            return letter;
        }

        return ' ';
    }

    private bool IsInDictionary(string word) {
        MonoBehaviour.print("ValidatorManager.IsInDictionary word " + word + "\n");
        for (var i = 0; i < allWords.Length; i++) {
            if (word.Equals(allWords[i].ToUpper())) {
                //bcdo make allWords upper at initialization
                MonoBehaviour.print("ValidatorManager.IsInDictionary " + i + " allWords[i] " + allWords[i] + "\n");
                return true;
            }
        }

        MonoBehaviour.print("ValidatorManager.IsInDictionary false  word " + word + "\n");
        return false;
    }
}