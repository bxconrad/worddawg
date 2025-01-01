using System;
using EasyUI.Toast;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    [SerializeField] private TransformShaker transformShaker;
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private LogoImage logoImage;
    public TMP_Text scoreText;

    private readonly Color toastColor = new(0, .5f, 0, 1);
    private string[] dogBonusWords;
    private int letterScore;
    public int currentScore { get; private set; }
    public int wordScore { get; private set; }
    public string longestWord { get; private set; }
    public int highestWordScore { get; private set; }
    public string highestWordScoreWord { get; private set; }
    public int numWords { get; private set; }
    public int numChangedWords { get; private set; }
    public int numLettersUsed { get; set; }

    public void Awake() {
        Initialize();
    }

    public void Start() {
        print("ScoreManager.Start\n");
        var textFile = Resources.Load("dogwords") as TextAsset;
        dogBonusWords = textFile.text.Split();

        print("ScoreManager.LoadData dogBonusWords " + dogBonusWords.Length + "\n");
    }

    public void Initialize() {
        currentScore = 0;
        scoreText.text = "0";
        longestWord = "";
        highestWordScoreWord = "";
        highestWordScore = 0;
        numChangedWords = 0;
        numWords = 0;
        numLettersUsed = 0;
    }

    public void End() {
        scoreText.text = "";
    }

    public void UpdateScore(string originalWord, string newWord) {
        currentScore += CalculateWordScore(originalWord, newWord);
        numLettersUsed += newWord.Length - originalWord.Length;
        UpdateHighScores(originalWord, newWord);
        updateScoreText();
        ShowToastMessage(originalWord, newWord);
        print("ScoreManager.UpdateScore " + currentScore + " wordScore " + wordScore + "\n");
    }

    public void UpdateScoreForReplaceRack() {
        currentScore = Math.Max(0, currentScore -= 50);
        if (!gameParameters.isEndGame) updateScoreText();
        print("ScoreManager.UpdateScoreForReplaceRack " + currentScore + "\n");
    }

    public int CalculateWordScore(string originalWord, string newWord) {
        originalWord = gameParameters.ExpandDoubleLetter(originalWord);
        newWord = gameParameters.ExpandDoubleLetter(newWord);
        letterScore = CalculateLetterScore(newWord);
        var multiplier = CalculateMultiplier(originalWord, newWord);

        wordScore = Convert.ToInt32(multiplier * letterScore);
        if (IsDogBonusWord(newWord)) {
            // bcdo calling IsDogBonusWord 2x, fix
            wordScore += 100;
            print("ScoreManager.CalculateWordScore IsDogBonusWord ");
        }

        // If entire rack is used
        if (newWord.Length - originalWord.Length >= MyPrefs.NUM_RACK_LETTERS) {
            wordScore += 100;
            print("ScoreManager.CalculateWordScore 100 bonus ");
        }

        print("ScoreManager.CalculateWordScore wordScore " + wordScore + "  letterScore " + letterScore +
              "  multiplier " + multiplier + " originalWord " + originalWord + " newWord " + newWord + "\n");

        return wordScore;
    }

    private void ShowToastMessage(string originalWord, string newWord) {
        var msg = "";
        var toastTime = 15f;
        Toast.Dismiss();
        originalWord = gameParameters.ExpandDoubleLetter(originalWord);
        newWord = gameParameters.ExpandDoubleLetter(newWord);
        if (IsDogBonusWord(newWord)) {
            msg = "Arooo! Special Word Dawg Bonus for " + newWord + "!!!\n";
            _ = transformShaker.ABeginRandomSpin(logoImage.transform, .3f, 4);
            print("ScoreManager.SendToastMessage IsDogBonusWord ");
        }

        // If entire rack is used
        if (newWord.Length - originalWord.Length >= MyPrefs.NUM_RACK_LETTERS) {
            print("ScoreManager.SendToastMessage 100 bonus ");
            msg += "100 Point Bonus for using all letters!!! Great Job!";
            _ = transformShaker.ABeginRandomSpin(logoImage.transform, .3f, 4);
        }

        if (wordScore > 100 && msg.Equals("")) {
            toastTime = 2f;
            msg = wordScore + " points! " + ComplimentHandler.instance.GetRandomCompliment();
            _ = transformShaker.ABeginRandomSpin(logoImage.transform, .3f, 2);
        }

        if (msg.Equals("")) {
            if (numWords < 3 && numChangedWords == 0)
                msg = "See if you can modify " + newWord + ". Select " + newWord +
                      " from the list of words. You must use ALL the letters in " + newWord +
                      " plus at least ONE letter from the rack.";
            else if (numChangedWords == 1 && originalWord.Length > 0)
                msg = "Congratulations! You turned " + originalWord + " into " + newWord + ". And you scored " +
                      wordScore + " points.\n\n Well done!";
        }

        if (wordScore > 10 && msg.Equals("")) {
            toastTime = 2f;
            msg = ComplimentHandler.instance.GetRandomCompliment();
        }

        print("ScoreManager.ShowToastMessage msg " + msg + "\n");

        if (!msg.Equals("")) Toast.Show(msg, toastTime, toastColor, UpdateBoard.toastPosition);
    }

    private bool IsDogBonusWord(string word) {
        //print("ScoreManager.IsDogBonusWord word " + word + "\n");
        for (var i = 0; i < dogBonusWords.Length; i++) {
            if (word.Equals(dogBonusWords[i].ToUpper())) {
                print("ScoreManager.IsDogBonusWord " + i + " dogBonusWords[i] " + dogBonusWords[i] + "\n");
                return true;
            }
        }

        //print("ScoreManager.IsDogBonusWord false  word " + word + "\n");
        return false;
    }

    private int CalculateLetterScore(string newWord) {
        var newWordLetterScore = 0;
        var letterDictionary = LetterInfo.letterDictionaryDictionary[gameParameters.language];

        foreach (var letter in newWord) {
            newWordLetterScore += letterDictionary[letter.ToString()];
        }

        return newWordLetterScore;
    }

    private double CalculateMultiplier(string originalWord, string newWord) {
        var factor = IsAddedSorD(originalWord, newWord) ? 1 : 1.5;
        var multiplier = newWord.Length > 4 ? (newWord.Length - 1) * factor : 1;
        return multiplier;
    }

    private bool IsAddedSorD(string originalWord, string newWord) {
        return newWord.Length - originalWord.Length == 1 && newWord.StartsWith(originalWord) &&
               (newWord.EndsWith("D") || newWord.EndsWith("S"));
    }


    private void UpdateHighScores(string originalWord, string newWord) {
        if (wordScore > highestWordScore) {
            highestWordScore = wordScore;
            highestWordScoreWord = newWord;
        }

        if (newWord.Length > longestWord.Length) longestWord = newWord;

        numWords++;
        if (originalWord.Length > 0) numChangedWords++;
    }

    private void updateScoreText() {
        scoreText.text = currentScore.ToString().PadRight(5) + wordScore;
    }
}