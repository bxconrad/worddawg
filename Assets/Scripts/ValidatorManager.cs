using UnityEngine;

public class ValidatorManager : MonoBehaviour {
    private string[] allWords;

    public void Start() {
        print("ValidatorManager.Start\n");
        var textFile = Resources.Load("dictionary") as TextAsset;
        allWords = textFile.text.Split();

        print("ValidatorManager.LoadData allWords " + allWords.Length + "\n");
    }

    public string ValidateInputWord(SelectedWord selectedWord, BetterRack betterRack, string inputWordString) {
        print("ValidatorManager.ValidateInputWord {" + inputWordString + "}\n");
        // return true;
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

        var letter = IsLettersContained(inputWordString, selectedWord.GetWord(), betterRack.GetWord());
        if (letter != ' ') {
            print("ValidatorManager.ValidateInputWord isLettersAvailable false letter " + letter + " \n");
            return "The letter " + letter + " is not in the selected word or the rack";
        }
        print("ValidatorManager.ValidateInputWord IsRackLettersUsed true\n");

        if (inputWordString.Length > 0 && inputWordString.Length < 3) {
            print("ValidatorManager.ValidateInputWord is Length 3 " + inputWordString.Length + " \n");
            return "Words must be at least 3 letters long";
        }
        print("ValidatorManager.ValidateInputWord IsLenght true\n");

        if (!IsInDictionary(inputWordString)) {
            print("ValidatorManager.ValidateInputWord " + inputWordString + " IsInDictionary  isValidNew false\n");
            return inputWordString + " is not in the dictionary";
        }
        print("ValidatorManager.IsInDictionary  isValidNew true \n");

        return "TRUE";
    }

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

            //print("IsLettersContained after continue");

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
                print("ValidatorManager.IsInDictionary " + i + " allWords[i] " + allWords[i] + "\n");
                return true;
            }
        }

        print("ValidatorManager.IsInDictionary false  word " + word + "\n");
        return false;
    }
}