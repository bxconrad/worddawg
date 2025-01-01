using UnityEngine;

public class ValidatorManager : MonoBehaviour {
    [SerializeField] private GameParameters gameParameters;
    private string[] allWords;

    public void Initialize() {
        print("ValidatorManager.Initialize\n");
        var dictionaryName = "dictionary-" + gameParameters.language;
        // dictionaryName = "dictionary-spanishProcessed";
        var textFile = Resources.Load(dictionaryName) as TextAsset;
        allWords = textFile.text.Split();

        print("ValidatorManager.Initialize allWords " + allWords.Length + " dictionary " + dictionaryName + "\n");
    }

    public string ValidateInputWord(SelectedWord selectedWord, BetterRack betterRack, string inputWordString) {
        print("ValidatorManager.ValidateInputWord {" + inputWordString + "}\n");
        //return true;
        // qulogic
        var selectedWordString = gameParameters.ExpandDoubleLetter(selectedWord.GetWord());
        var rackWordString = gameParameters.ExpandDoubleLetter(betterRack.GetWord());
        inputWordString = gameParameters.ExpandDoubleLetter(inputWordString);
        // check all letters from currentWord are used
        if (!selectedWord.isAllLettersUsed()) {
            print("ValidatorManager.ValidateInputWord IsAllLettersUsed  false \n");
            return "You must use all the letters in the selected word";
        }

        print("ValidatorManager.ValidateInputWord IsAllLettersUsed True\n");

        if (!betterRack.isOneLetterUsed()) {
            print("ValidatorManager.ValidateInputWord IsRackLettersUsed false\n");
            return "You must use at least one letter from the rack";
        }

        var letter = IsLettersContained(inputWordString, selectedWordString, rackWordString);
        if (letter != ' ') {
            print("ValidatorManager.ValidateInputWord isLettersAvailable false letter " + letter + " \n");
            return "The letter " + letter + " is not in the selected word or the rack";
        }

        print("ValidatorManager.ValidateInputWord IsRackLettersUsed true\n");

        if (inputWordString.Length > 0 && inputWordString.Length < 3) {
            print("ValidatorManager.ValidateInputWord is Length 3 " + inputWordString.Length + " \n");
            return "Words must be at least 3 letters long";
        }

        print("ValidatorManager.ValidateInputWord IsLength true\n");

        if (!IsInDictionary(inputWordString)) {
            print("ValidatorManager.ValidateInputWord " + inputWordString + " IsInDictionary  isValidNew false\n");
            return inputWordString + " is not in the dictionary";
        }

        print("ValidatorManager.IsInDictionary  isValidNew true \n");

        return "TRUE";
    }

    //verify that all the letters in the selectedWord have been used in the inputWord and at least one from the rack
    // if not, return the first letter that was not used. If so, return blank.
    private char IsLettersContained(string inputWordString, string selectedWordString, string letterRackWord) {
        print("IsLettersContained sel " + selectedWordString + " letterRackWord " + letterRackWord);
        foreach (var letter in inputWordString) {
            //print("IsLettersContained " + letter);
            var index = selectedWordString.IndexOf(letter);
            if (index >= 0) {
                selectedWordString = selectedWordString.Remove(index, 1);
                //print("IsLettersContained " + index + " selectedWordString " + selectedWordString);
                continue;
            }

            index = letterRackWord.IndexOf(letter);
            if (index >= 0) {
                letterRackWord = letterRackWord.Remove(index, 1);
                //print("IsLettersContained " + index + " letterRackWord " + letterRackWord);
                continue;
            }

            return letter;
        }

        return ' ';
    }

    private bool IsInDictionary(string word) {
        print("ValidatorManager.IsInDictionary word " + word + "\n");
        for (var i = 0; i < allWords.Length; i++) {
            if (word.Equals(allWords[i].ToUpper())) {
                //bcdo make allWords upper at initialization
                print("ValidatorManager.IsInDictionary " + i + " allWords[i] " + allWords[i] + "\n");
                return true;
            }
        }

        print("ValidatorManager.IsInDictionary false  word " + word + "\n");
        return false;
    }
}