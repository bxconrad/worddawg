using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = System.Random;

public class Dealer : MonoBehaviour {
    [SerializeField] private CountdownTimer countDown;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BetterRack betterRack;
    [SerializeField] private GameParameters gameParameters;
    private readonly int consMax = 5;
    private readonly List<string> letters = new();
    private readonly int vowelMax = 4;
    private Random aRandom;

    private int numLetters;
    private int numLettersDealt;
    private int? randomSeed;


    public void End() {
        SetNumLetters(0);
        SetRandomSeed(null);
    }

    public void Initialize() {
        numLettersDealt = 0;
        letters.Clear();
        if (gameParameters.isGameOfTheDay) SetRandomSeed(DateTime.Today.DayOfYear);
        FillLetterBag();
        betterRack.ClearRack();
        Deal();
        print("Dealer.Initialize complete #letters  " + letters.Count + "\n");
    }

    private void FillLetterBag() {
        print("Dealer.AddLetters *start* #letters  " + letters.Count + " lang " + gameParameters.language + "\n");
        var letterInfos = LetterInfo.letterInfosEN;
        if (MyPrefs.PREFS_LANG_SP.Equals(gameParameters.language))
            letterInfos = LetterInfo.letterInfosSP;
        foreach (var letterInfo in letterInfos) {
            for (var i = 0; i < letterInfo.getDistribution(); i++) {
                letters.Add(letterInfo.getTheLetter());
            }
        }

        // foreach (var group in letters.GroupBy(b => b).OrderBy(g => g.Key)) {
        //     print("Dealer.AddLetters letter  " + $"{group.Key} ({group.Count()})" + "\n");
        // }

        Shuffle(letters);

        print("Dealer.AddLetters #letters  " + letters.Count + "\n");
    }

    private void Shuffle<T>(IList<T> list) {
        var n = list.Count;
        aRandom = GetRandomSeed() == null ? new Random() : new Random((int)GetRandomSeed());
        print("Dealer.Shuffle randomSeed  [" + GetRandomSeed() + "] aRandom " + aRandom + "\n");
        while (n > 1) {
            n--;
            var k = aRandom.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public void DealNewRack() {
        betterRack.ClearRack();
        Deal();
    }

    public void Deal() {
        print("Dealer.Deal word {" + betterRack.GetWord().Trim() + "} timed? " + gameManager.IsTimed() +
              " letters.count " + letters.Count() + " lt? " + (letters.Count() <= MyPrefs.NUM_RACK_LETTERS) + "\n");

        // calculate how many letters are needed from the rack
        var numLettersNeeded = MyPrefs.NUM_RACK_LETTERS - betterRack.GetWord().Trim().Length;
        var rackLetters = betterRack.GetWord().Trim();

        for (var i = 0; i < numLettersNeeded && IsDealable(); i++) {
            //print("Dealer.Deal i" + i + "\n");
            numLettersDealt++;
            rackLetters += letters[0];
            //print("Dealer.Deal addLetter rackLetters " + rackLetters + "\n");
            rackLetters = FixLetterDistribution(rackLetters);
            letters.RemoveAt(0);
        }

        if (rackLetters.Length == 0) {
            print("Dealer.Deal calling endGame " + rackLetters + "\n");
            _ = gameManager.EndGame();
            return;
        }

        betterRack.InitializeTiles(rackLetters);
        countDown.SetText(GetTotalNumLettersLeft().ToString());

        print("Dealer.Deal rackLetters numLettersNeeded " + numLettersNeeded +
              " rack " + betterRack.GetWord() + " numLettersDealt " + numLettersDealt + "\n");
    }


    public string FixLetterDistribution(string dealString) {
        //bcdo small bug here. we should fix distribution when we first hit last rack
        // dont reset letters at end of untimed game
        dealString = FixConsonantOrVowels(dealString);
        dealString = FixDuplicates(dealString);
        return dealString;
    }

    // See if there are more than one duplicate letters, ie AABBCDE or AABBCCE
    public string FixDuplicates(string dealString) {
        if (dealString.Length < 3) return dealString;
        var updatedDealString = new StringBuilder(dealString);
        var lettersIndex = 0;
        var distinctCount = dealString.Distinct().Count();
        while (distinctCount <= dealString.Length - 2) {
            print("Dealer.FixDuplicates *FIX*  dealString " + dealString + " distinctCount " + distinctCount + "\n");

            // get the last letter
            var lastLetterIndex = dealString.Length - 1;
            var lastLetter = dealString.Substring(lastLetterIndex, 1);

            var isVowel = IsAllVowels(lastLetter);
            print("Dealer.FixDuplicates *Found* updatedDealString " + updatedDealString +
                  " lastLetter " + lastLetter + " isVowel " + isVowel + "\n");
            // if letter is vowel, find different vowel. if consonant, find a different consonant
            lettersIndex = BuyMeAVowelOrConsonant(isVowel, lettersIndex + 1);
            if (lettersIndex < 0)
                // didn't find a suitable letter to swap in the letter bag
                return updatedDealString.ToString();

            if (!letters[lettersIndex].Equals(lastLetter)) {
                // found a different letter of same type (vowel or consonant)
                print("Dealer.FixDuplicates SWITCH newLetter {" + letters[lettersIndex] +
                      "} original {" + lastLetter + " lettersIndex " + lettersIndex + " lastLetterIndex " +
                      lastLetterIndex + "}\n");
                // switch letters. put the old one back in the bag
                SwitchLetters(lettersIndex, lastLetter, updatedDealString, lastLetterIndex);
                print("Dealer.FixDuplicates switched updatedDealString " + updatedDealString + "}\n");
            }
            else {
                print("Dealer.FixDuplicates No Switch same letter " + lastLetter + "}\n");
            }

            distinctCount = updatedDealString.ToString().Distinct().Count();
            print("Dealer.FixDuplicates return " + updatedDealString + " dealString " + dealString +
                  " distinctCount " + distinctCount + "\n");
        }

        return updatedDealString.ToString();
    }


    // Ensure a good letter distribution. At least 3-5 consonants, 2-4 vowels 
    private string FixConsonantOrVowels(string dealString) {
        // we cant tell until we have 5 letters (if all 5 are vowels)
        if (!IsCheckingVowelDistribution(dealString)) return dealString;
        // calculate number of consonants in dealString
        var numCons = 0;
        var numVowels = 0;
        foreach (var dealLetter in dealString.Trim()) {
            numCons = IsAllConsonant(dealLetter.ToString()) ? numCons + 1 : numCons;
            numVowels = dealString.Length - numCons;
        }

        // print("Dealer.FixConsonantOrVowels dealString " + dealString + " numCons  " + numCons + "\n");
        var updatedDealString = new StringBuilder(dealString);
        var lastLetterIndex = dealString.Length - 1;
        // switch the last letter if we have too many cons or vowels
        if (numCons > consMax || numVowels > vowelMax) {
            //print("Dealer.FixConsonantOrVowels  ~~~ dealString  " + dealString + " sb " + updatedDealString + "\n");
            var dealLetter = updatedDealString[lastLetterIndex].ToString(); // get the letter from the dealString
            // update if we now have a good distribution
            var isConsMax = numCons > consMax;
            // if we have too many consonants and the letter is a consonant, look for a vowel to replace it
            if ((IsAllConsonant(dealLetter) && isConsMax) ||
                // if we have too few consonants and the letter is a vowel, look for a consonant to replace it
                (!IsAllConsonant(dealLetter) && !isConsMax)) {
                // find a consonant or vowel in our bag of letters starting at the index we left off on our last Buy
                var lettersIndex = BuyMeAVowelOrConsonant(isConsMax, 0);
                if (lettersIndex >= 0) SwitchLetters(lettersIndex, dealLetter, updatedDealString, lastLetterIndex);

                print("Dealer.FixConsonantOrVowels  *** dealString  " + dealString + " updatedDealString " +
                      updatedDealString + "\n");
                //print("Dealer.FixConsonantOrVowels  *** numCons  " + numCons + "\n");
            }
        }

        //print("Dealer.FixConsonantOrVowels dealString  " + dealString + " with " + updatedDealString + "\n");
        return updatedDealString.ToString();
    }

    private bool IsCheckingVowelDistribution(string dealString) {
        return dealString.Length >= consMax;
    }

// iterate over our array of letters and find a consonant or vowel as requested.
// We pass in the start index so we continue where we left off
    private int BuyMeAVowelOrConsonant(bool isVowel, int startIndex) {
        // print("Dealer.BuyMeAVowelOrConsonant  isVowel " + isVowel + " startIndex " + startIndex + "\n");
        for (var lettersIndex = startIndex; lettersIndex < letters.Count && startIndex >= 0; lettersIndex++) {
            var letter = letters[lettersIndex];
            //print("Dealer.BuyMeAVowelOrConsonant  letter " + letter + " index " + lettersIndex + "\n");
            if ((isVowel && IsAllVowels(letter)) || (!isVowel && !IsAllVowels(letter))) return lettersIndex;
        }

        print("Dealer.BuyMeAVowelOrConsonant  *** Unable to find vowel or consonant in letters[]\n");
        return -1;
    }

// Switch a letter in the dealString with a letter from our letters array
    private void SwitchLetters(int lettersIndex, string dealLetter, StringBuilder sb, int sbIndex) {
        var aLetter = letters[lettersIndex];
        letters[lettersIndex] = dealLetter;
        sb[sbIndex] = aLetter.ToCharArray()[0];
        print("Dealer.SwitchLetters replace  " + dealLetter + " with " + aLetter + " lettersIndex " +
              lettersIndex + "\n");
    }

    private bool IsAllConsonant(string compareString) {
        var matchCons = new Regex("^[^aeiouAEIOU]+$").Match(compareString).Success;
        //print("Dealer.isAllConsonant compareString " + compareString + " " + matchCons + "\n");

        return matchCons;
    }

    private bool IsAllVowels(string compareString) {
        return !IsAllConsonant(compareString);
    }

    private bool IsDealable() {
        // for timed game, if we run out of letters add more and will return true
        if (gameManager.IsTimed() && letters.Count() < MyPrefs.NUM_RACK_LETTERS) FillLetterBag();
        return letters.Count > 0 && GetNumLetters() - numLettersDealt > 0;
    }

    // this includes letters in rack
    public int GetTotalNumLettersLeft() {
        var numLeft = GetNumLetters() - numLettersDealt + betterRack.GetWord().Trim().Length;
        print(
            "Dealer.GetTotalNumLettersLeft  " + numLeft + " numLtrs " + GetNumLetters() + " numLettersDealt " +
            numLettersDealt + " numRack " + betterRack.GetWord() + " ltrCount " + letters.Count() + "\n");
        return numLeft;
    }

    public bool IsNearEndGame() {
        return GetTotalNumLettersLeft() <= MyPrefs.NUM_RACK_LETTERS;
    }

    // it's timed game numLetters will be 0. would be better to ask GameManager if it is a timedGame or not.
    private int GetNumLetters() {
        return numLetters == 0 ? 99999 : numLetters;
    }

    public void SetNumLetters(int value) {
        numLetters = value;
    }

    private int? GetRandomSeed() {
        return randomSeed;
    }

    public void SetRandomSeed(int? value) {
        randomSeed = value;
    }
}