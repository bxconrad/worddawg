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
    [SerializeField] private AudioSource audioSource;
    private readonly List<string> letters = new();
    private Random aRandom;
    private int consMax;
    private AudioClip howl;
    private int lengthForDuplicates;
    private int numLettersDealt;
    private int vowelMax;

    public void Awake() {
        howl = Resources.Load("dogHowlingAtMoon") as AudioClip;
    }

    public void Initialize() {
        print("Dealer.Initialize \n");
        numLettersDealt = 0;
        numLettersDealt = 0;
        letters.Clear();
        consMax = (int)(gameParameters.numRackLetters * .73);
        vowelMax = (int)(gameParameters.numRackLetters * .6);
        lengthForDuplicates = (int)(gameParameters.numRackLetters * .45);
        FillLetterBag();
        betterRack.ClearRack();
        Deal();
        print("Dealer.Initialize complete #letters  " + letters.Count + " consMax " + consMax + " vowelMax " +
              vowelMax + " lengthForDuplicates " + lengthForDuplicates + "\n");
    }

    private void FillLetterBag() {
        print("Dealer.FillLetterBag *start* #letters  " + letters.Count + " lang " + gameParameters.language + "\n");
        var letterInfos = LetterInfo.letterInfosEN;
        if (MyPrefs.PREFS_LANG_SP.Equals(gameParameters.language))
            letterInfos = LetterInfo.letterInfosSP;
        foreach (var letterInfo in letterInfos) {
            for (var i = 0; i < letterInfo.getDistribution(); i++) {
                letters.Add(letterInfo.getTheLetter());
            }
        }

        Shuffle(letters);
        //    letters.Insert(1, "I");
        print("Dealer.FillLetterBag #letters " + letters.Count + "\n");
    }

    private void Shuffle<T>(IList<T> list) {
        var n = list.Count;
        aRandom = GetRandom();
        print("Dealer.Shuffle randomSeed  [" + gameParameters.dealerSeed + "] aRandom " + aRandom + "\n");
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
        print("Dealer.Deal word *{" + betterRack.GetWord().Trim() + "} timed? " + gameParameters.isTimed +
              " letters.count " + letters.Count() + " lt? " + (letters.Count() <= gameParameters.numRackLetters) +
              "\n");

        // calculate how many letters are needed from the rack
        var numLettersNeeded = gameParameters.numRackLetters - betterRack.GetWord().Trim().Length;
        var rackLetters = betterRack.GetWord().Trim();

        for (var i = 0; i < numLettersNeeded && IsDealable(); i++) {
            //print("Dealer.Deal i" + i + "\n");
            numLettersDealt++;
            rackLetters += letters[0];
            //print("Dealer.Deal addLetter rackLetters " + rackLetters + "\n");
            letters.RemoveAt(0);
            rackLetters = FixLetterDistribution(rackLetters);
        }

        if (rackLetters.Length == 0) {
            print("Dealer.Deal calling endGame  rackLetters " + rackLetters + " muteSound? " + audioSource.mute +
                  " numLettersNeeded " + numLettersNeeded + "\n");
            // bcHack. audio would not play in EndGame so we do it here.
            audioSource.PlayOneShot(howl);
            _ = gameManager.EndGame();
            return;
        }

        betterRack.InitializeTiles(rackLetters);
        countDown.SetText(GetTotalNumLettersLeft().ToString());

        print("~~Dealer.Deal rackLetters  rack " + betterRack.GetWord() + " numLettersNeeded " + numLettersNeeded +
              " numLettersDealt " + numLettersDealt + "\n");
    }

    private string FixLetterDistribution(string dealString) {
        //bcdo small bug here. we should fix distribution when we first hit last rack
        // dont reset letters at end of untimed game
        var savedDealString = dealString;
        dealString = FixConsonantOrVowels(dealString);
        dealString = FixDuplicates(dealString);
        // If we fixed the dealString, put the orignal last letter back in the bag and remove the new last letter from the bag
        if (!savedDealString.Equals(dealString)) {
            print("Dealer.FixLetterDistribution *FIXa*  savedDealString " + savedDealString + "\n");
            // put the orignal last letter back in the bag 
            var savedLastLetter = savedDealString.Substring(savedDealString.Length - 1, 1);
            letters.Insert(0, savedLastLetter);
            //remove the new last letter from the bag
            var newLastLetter = dealString.Substring(dealString.Length - 1, 1);
            letters.Remove(newLastLetter);
            print("Dealer.FixLetterDistribution *FIXb*  dealString " + dealString + "\n");
        }

        return dealString;
    }

    // See if there are more than one duplicate letters, ie AABBCDE or AABBCCE
    private string FixDuplicates(string dealString) {
        if (dealString.Length < lengthForDuplicates) return dealString;
        var updatedDealString = new StringBuilder(dealString);
        var lettersIndex = -1;
        var distinctCount = dealString.Distinct().Count();
        while (distinctCount <= dealString.Length - 2) {
            print("Dealer.FixDuplicates *FIX*  dealString " + dealString + " distinctCount " + distinctCount + "\n");
            var lastLetter = updatedDealString.ToString().Substring(dealString.Length - 1, 1);

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
                // replace the old letter with the new letter 
                ReplaceLetter(updatedDealString, lettersIndex);
                print("Dealer.FixDuplicates updated updatedDealString " + updatedDealString + "}\n");
            }

            distinctCount = updatedDealString.ToString().Distinct().Count();
            print("Dealer.FixDuplicates *Fix* updatedDealString " + updatedDealString + " dealString " + dealString +
                  " distinctCount " + distinctCount + "\n");
        }

        // print("Dealer.FixDuplicates return " + updatedDealString + " dealString " + dealString +
        //       " distinctCount " + distinctCount + "\n");
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
            print("Dealer.FixConsonantOrVowels  ~~~ dealString  " + dealString + " sb " + updatedDealString + "\n");
            var dealLetter = updatedDealString[lastLetterIndex].ToString(); // get the letter from the dealString
            // update if we now have a good distribution
            var isConsMax = numCons > consMax;
            // if we have too many consonants and the letter is a consonant, look for a vowel to replace it
            if ((IsAllConsonant(dealLetter) && isConsMax) ||
                // if we have too few consonants and the letter is a vowel, look for a consonant to replace it
                (!IsAllConsonant(dealLetter) && !isConsMax)) {
                // find a consonant or vowel in our bag of letters starting at the index we left off on our last Buy
                var lettersIndex = BuyMeAVowelOrConsonant(isConsMax, 0);
                if (lettersIndex >= 0) {
                    ReplaceLetter(updatedDealString, lettersIndex);
                }
                else {
                    print("Dealer.FixConsonantOrVowels Could not fix  dealString " + dealString +
                          " updatedDealString " + updatedDealString + "\n");
                }

                print("Dealer.FixConsonantOrVowels  *** dealString  " + dealString + " updatedDealString " +
                      updatedDealString + "\n");
                //print("Dealer.FixConsonantOrVowels  *** numCons  " + numCons + "\n");
            }
        }

        //print("Dealer.FixConsonantOrVowels dealString  " + dealString + " with " + updatedDealString + "\n");
        return updatedDealString.ToString();
    }

    private void ReplaceLetter(StringBuilder updatedDealString, int lettersIndex) {
        print("Dealer.ReplaceLetter *B4Replace*  updatedDealString " + updatedDealString + "\n");
        updatedDealString.Replace(updatedDealString[updatedDealString.Length - 1].ToString(), letters[lettersIndex],
            updatedDealString.Length - 1, 1);
        print("Dealer.ReplaceLetter  *AFReplace updatedDealString " + updatedDealString + "\n");
    }

    private bool IsCheckingVowelDistribution(string dealString) {
        return dealString.Length >= consMax;
    }

    // iterate over our array of letters and find a consonant or vowel as requested.
    // We pass in the start index so we continue where we left off
    private int BuyMeAVowelOrConsonant(bool isVowel, int startIndex) {
        print("Dealer.BuyMeAVowelOrConsonant  isVowel " + isVowel + " startIndex " + startIndex + "\n");
        for (var lettersIndex = startIndex; lettersIndex < letters.Count && startIndex >= 0; lettersIndex++) {
            var letter = letters[lettersIndex];
            //print("Dealer.BuyMeAVowelOrConsonant  letter " + letter + " index " + lettersIndex + "\n");
            if ((isVowel && IsAllVowels(letter)) || (!isVowel && !IsAllVowels(letter))) {
                print("Dealer.BuyMeAVowelOrConsonant returning  letter " + letter + " index " + lettersIndex + "\n");
                return lettersIndex;
            }
        }

        var which = isVowel ? "vowel" : "consonant";
        print("Dealer.BuyMeAVowelOrConsonant  *** Unable to find " + which + " in letters[]. numLetters " +
              letters.Count + "\n");
        return -1;
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
        if (letters.Count() < gameParameters.numRackLetters) {
            if (gameParameters.isTimed || gameParameters.numLetters > numLettersDealt) {
                FillLetterBag();
            }
        }

        return letters.Count > 0 && GetNumLetters() - numLettersDealt > 0;
    }

    // this includes letters in rack
    public int GetTotalNumLettersLeft() {
        var numLeft = GetNumLetters() - numLettersDealt + betterRack.GetWord().Trim().Length;
        // print("Dealer.GetTotalNumLettersLeft  " + numLeft + " numLtrs " + GetNumLetters() + " numLettersDealt " +
        //     numLettersDealt + " numRack " + betterRack.GetWordText() + " ltrCount " + letters.Count() + "\n");
        return numLeft;
    }

    public bool IsNearEndGame() {
        return !gameParameters.isTimed && GetTotalNumLettersLeft() <= gameParameters.numRackLetters;
    }

    // it's timed game numLetters will be 99999.
    private int GetNumLetters() {
        return gameParameters.isTimed ? 99999 : gameParameters.numLetters;
    }

    private Random GetRandom() {
        return gameParameters.dealerSeed == 0 ? new Random() : new Random(gameParameters.dealerSeed);
    }


    private string LetterBagToString() {
        var word = "";
        foreach (var letter in letters) {
            word += letter;
        }

        return word;
    }
}