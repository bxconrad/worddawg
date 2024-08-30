using System.Collections.Generic;

public class LetterInfo {
    public static LetterInfo A = new("A", 1, 9);
    public static LetterInfo B = new("B", 4, 2);
    public static LetterInfo C = new("C", 4, 2);
    public static LetterInfo D = new("D", 2, 4);
    public static LetterInfo E = new("E", 1, 13); //12 -1
    public static LetterInfo F = new("F", 4, 3); //2   +1
    public static LetterInfo G = new("G", 4, 3);
    public static LetterInfo H = new("H", 5, 3); //2   +1
    public static LetterInfo I = new("I", 1, 7); //9   +2
    public static LetterInfo J = new("J", 8, 1);
    public static LetterInfo K = new("K", 6, 1);
    public static LetterInfo L = new("L", 3, 5); //4   +1
    public static LetterInfo M = new("M", 4, 3); //2   -1
    public static LetterInfo N = new("N", 2, 6);
    public static LetterInfo O = new("O", 1, 8);
    public static LetterInfo P = new("P", 4, 3); //2   +1
    public static LetterInfo Q = new("Q", 8, 0); //1   -1
    public static LetterInfo R = new("R", 3, 6); //6    0
    public static LetterInfo S = new("S", 1, 5); //4   +1
    public static LetterInfo T = new("T", 2, 6);
    public static LetterInfo U = new("U", 4, 2);
    public static LetterInfo V = new("V", 6, 2);
    public static LetterInfo W = new("W", 4, 2);
    public static LetterInfo X = new("X", 10, 1);
    public static LetterInfo Y = new("Y", 4, 2);
    public static LetterInfo Z = new("Z", 10, 1);

    //public static LetterInfo BLANK = new LetterInfo("_", 0, 2);
    public static LetterInfo NULL_LETTER_INFO = new(" ", 0, 1);

    // 39 vowels, 64 consonants 38% vowel
    // vowels 36% https://en.wikipedia.org/wiki/Letter_frequency
    public static List<LetterInfo> allLetterInfos = new()
        { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, R, S, T, U, V, W, X, Y, Z };

    public static Dictionary<string, int> letterDictionary = new();

    private readonly int distribution;

    private readonly string theLetter;
    private readonly int value;

    static LetterInfo() {
        {
            foreach (var letterInfo in allLetterInfos) {
                letterDictionary.Add(letterInfo.theLetter, letterInfo.value);
            }
        }
        //print("LetterInfo.static " + letterDictionary.Count);
    }

    public LetterInfo(string theLetter, int value, int distribution) {
        this.theLetter = theLetter;
        this.value = value;
        this.distribution = distribution;
    }

    private static void CreateDictionary() {
        letterDictionary = new Dictionary<string, int>();
        foreach (var letterInfo in allLetterInfos) {
            letterDictionary.Add(letterInfo.theLetter, letterInfo.value);
        }
    }

    public string getTheLetter() {
        return theLetter;
    }

    public int getValue() {
        return value;
    }

    public int getDistribution() {
        return distribution;
    }
}