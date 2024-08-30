using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = System.Random;

public class LetterBag : MonoBehaviour {
    [SerializeField] private CountdownTimer countDown;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BetterRack betterRack;
    private readonly int consMax = 5;
    private readonly int consMin = 3;
    private Random aRandom;
    private List<string> letters;

    private int numLetters;
    private int numLettersInRack;
    private int? randomSeed;
    private int startingNumberOfLetters;

    public void End() {
        SetNumLetters(0);
        SetRandomSeed(null);
    }

    public void Initialize() {
        letters = new List<string>();
        foreach (var letterInfo in LetterInfo.allLetterInfos) {
            for (var i = 0; i < letterInfo.getDistribution() * 5; i++) {
                letters.Add(letterInfo.getTheLetter());
            }
        }
        startingNumberOfLetters = letters.Count;
        Shuffle(letters);

        //letters.RemoveRange(GetNumLetters(), letters.Count - GetNumLetters());
        print("LetterBag.Initialize #letters  " + letters.Count + " prefs" + MyPrefs.GetNumLetters() + " firstCount " +
              startingNumberOfLetters + "\n");
    }

    private void Shuffle<T>(IList<T> list) {
        var n = list.Count;
        var aRandom = GetRandomSeed() == null ? new Random() : new Random((int)GetRandomSeed());
        while (n > 1) {
            n--;
            var k = aRandom.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }


    private string Deal() {
        if (IsDealable()) {
            var letter = letters[0];
            letters.RemoveAt(0);
            // print("LetterBag.Deal return" + letter + " next: " + letters[0] + "\n");
            return letter;
        }
        print("Illegal deal request. No letters in bag. keep playing\n");
        return " ";
    }

    public string Deal(int numDeal) {
        print("LetterBag.Deal " + numDeal + " \n");
        var s = "";
        for (var i = 0; i < numDeal; i++) {
            s += Deal();
        }
        print("LetterBag.Deal #lettersUsed " + GetNumLettersUsed() + "\n");
        return s;
    }

    // bcdo could get rid of this and call deal 7,currentRackLetters
    public string DealNewRack() {
        print("LetterBag.DealNewRack\n");
        var s = Deal(MyPrefs.NUM_RACK_LETTERS);
        s = FixLetterDistribution(s);
        return s;
    }

    public string FixLetterDistribution(string dealString) {
        //dealString = dealString.Trim(); // this causes problems
        dealString = FixConsonantOrVowels(dealString);
        dealString = FixTriplicates(dealString);
        numLettersInRack =
            dealString.Trim().Length; // dealString can have blanks and dangerous to trim it and reset dealString
        countDown.SetText(GetNumLettersLeft().ToString());
        return dealString;
    }

    // See if there are three of any one letter in the string
    private string FindTriplicateLetter(string dealString) {
        // print("LetterBag.FindTriplicateLetter dealString " + dealString + "\n");
        var charLookup = dealString.ToLookup(c => c);

        foreach (var c in charLookup) {
            //print("LetterBag.FindTriplicateLetter " + c.Key + " " + charLookup[c.Key].Count());
            if (charLookup[c.Key].Count() > 2) {
                print("LetterBag.FindTriplicateLetter ret {" + c.Key + "}\n");
                // trim in case dups are blanks
                return c.Key.ToString().Trim();
            }
        }

        print("LetterBag.FindTriplicateLetter no triplicates found \n");
        return "";
    }

    // Swap out letters if there are three of any one letter
    private string FixTriplicates(string dealString) {
        // print("LetterBag.FixTriplicates dealString " + dealString + "\n");
        var updatedDealString = new StringBuilder(dealString);
        var dealLetter = FindTriplicateLetter(dealString);
        var lettersIndex = 0;
        while (!dealLetter.Equals("")) {
            // we found a triplicate
            print("LetterBag.FixTriplicates triplicate Found *** " + dealLetter);
            var dsIndex = updatedDealString.ToString().LastIndexOf(dealLetter);
            var isVowel = IsAllVowels(dealLetter);
            // if letter is vowel, find different vowel. if consonant, find a different consonant
            lettersIndex = BuyMeAVowelOrConsonant(isVowel, lettersIndex + 1);
            if (lettersIndex < 0) {
                // didn't find a suitable letter to swap in the letter bag
                return updatedDealString.ToString();
            }

            if (!letters[lettersIndex].Equals(dealLetter)) {
                // found a different letter of same type (vowel or consonant)
                print("LetterBag.FixTriplicates SWITCH newLetter {" +
                      letters[lettersIndex] + "} original {" + dealLetter + " lettersIndex " + lettersIndex +
                      " dsIndex " + dsIndex + "}\n");
                // switch letters. put the old one back in the bag
                SwitchLetters(lettersIndex, dealLetter, updatedDealString, dsIndex);
                // see if there is another triplicate. We may have even created a different one with the swap.
                // print("LetterBag.FixTriplicates afterSwitch updatedDealString " + updatedDealString + "\n");
                dealLetter = FindTriplicateLetter(updatedDealString.ToString());
            }
            // if letters match look for another starting at the updated lettersIndex

            print("LetterBag.FixTriplicates *** updatedDealString " + updatedDealString + " original " + dealString +
                  "\n");
        } // end while
        return updatedDealString.ToString();
    }

    // Ensure a good letter distribution. At least 2 vowels and 2 consonants
    private string FixConsonantOrVowels(string dealString) {
        // calculate number of consonants in dealString
        var numCons = 0;
        foreach (var dealLetter in dealString.Trim()) {
            numCons = IsAllConsonant(dealLetter.ToString()) ? numCons + 1 : numCons;
        }

        print("LetterBag.FixLetterDistribution dealString " + dealString + " numCons  " + numCons + "\n");
        var updatedDealString = new StringBuilder(dealString);
        var lettersIndex = 0;
        // Iterate over each letter in dealString in reverse until we get desired distribution
        for (var dsIndex = updatedDealString.Length - 1;
             dsIndex >= 0 && (numCons > consMax || numCons < consMin);
             dsIndex--) {
            //print("LetterBag.FixLetterDistribution  ~~~ dealString  " + dealString + " sb " + updatedDealString + "\n");
            var dealLetter = updatedDealString[dsIndex].ToString(); // get the letter from the dealString
            // update if we now have a good distribution
            var isConsMax = numCons > consMax;
            // if we have too many consonants and the letter is a consonant, look for a vowel to replace it
            if ((IsAllConsonant(dealLetter) && isConsMax) ||
                // if we have too few consonants and the letter is a vowel, look for a consonant to replace it
                (!IsAllConsonant(dealLetter) && !isConsMax)) {
                // find a consonant or vowel in our bag of letters starting at the index we left off on our last Buy
                lettersIndex = BuyMeAVowelOrConsonant(isConsMax, lettersIndex);
                if (lettersIndex >= 0) {
                    SwitchLetters(lettersIndex, dealLetter, updatedDealString, dsIndex);
                }

                print("LetterBag.FixLetterDistribution  *** dealString  " + dealString + " updatedDealString " +
                      updatedDealString + "\n");
                // update how many consonants we now have
                numCons = isConsMax ? numCons - 1 : numCons + 1;
                //print("LetterBag.FixLetterDistribution  *** numCons  " + numCons + "\n");
            }
        }

        //print("LetterBag.FixLetterDistribution dealString  " + dealString + " with " + updatedDealString + "\n");
        return updatedDealString.ToString();
    }

    // iterate over our array of letters and find a consonant or vowel as requested.
    // We pass in the start index so we continue where we left off
    private int BuyMeAVowelOrConsonant(bool isVowel, int startIndex) {
        //print("LetterBag.FindMeAVowelOrConsonant  isVowel " + isVowel + " startIndex " + startIndex + "\n");
        for (var lettersIndex = startIndex; lettersIndex < letters.Count && startIndex >= 0; lettersIndex++) {
            var letter = letters[lettersIndex];
            // print("LetterBag.FindMeAVowelOrConsonant  letter " + letter + " index " + lettersIndex + "\n");
            if ((isVowel && IsAllVowels(letter)) || (!isVowel && !IsAllVowels(letter))) {
                return lettersIndex;
            }
        }

        print("LetterBag.FindMeAVowelOrConsonant  *** Unable to find vowel or consonant in letters[]\n");
        return -1;
    }

    // Switch a letter in the dealString with a letter from our letters array
    private void SwitchLetters(int lettersIndex, string dealLetter, StringBuilder sb, int sbIndex) {
        var aLetter = letters[lettersIndex];
        letters[lettersIndex] = dealLetter;
        sb[sbIndex] = aLetter.ToCharArray()[0];
        print("LetterBag.SwitchLetters replace  " + dealLetter + " with " + aLetter + " lettersIndex " +
              lettersIndex +
              "\n");
    }

    private bool IsAllConsonant(string compareString) {
        var matchCons = new Regex("^[^aeiouAEIOU]+$").Match(compareString).Success;
        //print("LetterBag.isAllConsonant compareString " + compareString + " " + matchCons + "\n");

        return matchCons;
    }

    private bool IsAllVowels(string compareString) {
        return !IsAllConsonant(compareString);
    }

    // lettersLeft includes #letters in rack. 
    // We can only deal to rack when our #left exceeds number needed for rack
    private bool IsDealable() {
        return GetNumLettersLeft() > MyPrefs.NUM_RACK_LETTERS;
    }

    // after first deal, snl =500, lc = 493, nlr = 7. 500-493+7 = 0, good
    // 30 letters dealt, 7 in rack. 500-470-7 = 23 ... good
    // 50 letters dealt, 7 in rack. 500-450-7 = 43 good

    // This factors in letters in rack
    public int GetNumLettersUsed() {
        //print("LetterBag.GetNumLettersUsed numLettersInRack " + numLettersInRack + "\n");
        return startingNumberOfLetters - letters.Count - numLettersInRack;
    }

    // this includes letters in rack
    public int GetNumLettersLeft() {
        return GetNumLetters() - GetNumLettersUsed();
    }

    private int GetNumLetters() {
        return numLetters == 0 ? 300 : numLetters;
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