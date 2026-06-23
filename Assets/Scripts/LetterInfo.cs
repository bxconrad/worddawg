using System.Collections.Generic;
using UnityEngine;

public class LetterInfo {
    public static LetterInfo A = new("A", 1, 9);
    public static LetterInfo B = new("B", 4, 2);
    public static LetterInfo C = new("C", 4, 2);
    public static LetterInfo D = new("D", 2, 4);
    public static LetterInfo E = new("E", 1, 12);
    public static LetterInfo F = new("F", 4, 3); //2   +1
    public static LetterInfo G = new("G", 4, 3);
    public static LetterInfo H = new("H", 5, 3); //2   +1
    public static LetterInfo I = new("I", 1, 8); //9   +1
    public static LetterInfo J = new("J", 8, 1);
    public static LetterInfo K = new("K", 6, 1);
    public static LetterInfo L = new("L", 3, 5); //4   +1
    public static LetterInfo M = new("M", 4, 3); //2   -1
    public static LetterInfo N = new("N", 2, 6);
    public static LetterInfo O = new("O", 1, 8);
    public static LetterInfo P = new("P", 4, 2);
    public static LetterInfo Q = new("Q", 8, 1);
    public static LetterInfo R = new("R", 3, 6);
    public static LetterInfo S = new("S", 1, 5); //4   +1
    public static LetterInfo T = new("T", 2, 6);
    public static LetterInfo U = new("U", 4, 2);
    public static LetterInfo V = new("V", 6, 2);
    public static LetterInfo W = new("W", 4, 2);
    public static LetterInfo X = new("X", 10, 1);
    public static LetterInfo Y = new("Y", 4, 2);
    public static LetterInfo Z = new("Z", 10, 1);

    public static LetterInfo A_SP = new("A", 1, 11);
    public static LetterInfo B_SP = new("B", 3, 3);
    public static LetterInfo C_SP = new("C", 2, 4);
    public static LetterInfo D_SP = new("D", 2, 4);
    public static LetterInfo E_SP = new("E", 1, 11);
    public static LetterInfo F_SP = new("F", 4, 2);
    public static LetterInfo G_SP = new("G", 2, 2);
    public static LetterInfo H_SP = new("H", 4, 2);
    public static LetterInfo I_SP = new("I", 1, 6);
    public static LetterInfo J_SP = new("J", 6, 2);
    public static LetterInfo K_SP = new("K", 8, 1);
    public static LetterInfo L_SP = new("L", 1, 4);
    public static LetterInfo LL_SP = new("*", 6, 3); // 8/1 or 4/3
    public static LetterInfo M_SP = new("M", 3, 3);
    public static LetterInfo N_SP = new("N", 1, 5);
    public static LetterInfo Ñ_SP = new("Ñ", 6, 3); // 8/1 or 1/3
    public static LetterInfo O_SP = new("O", 1, 8);
    public static LetterInfo P_SP = new("P", 2, 3);
    public static LetterInfo Q_SP = new("Q", 8, 1);
    public static LetterInfo R_SP = new("R", 1, 4);
    public static LetterInfo S_SP = new("S", 1, 7);
    public static LetterInfo T_SP = new("T", 1, 4);
    public static LetterInfo U_SP = new("U", 4, 6);
    public static LetterInfo V_SP = new("V", 4, 2);
    public static LetterInfo W_SP = new("W", 4, 2);
    public static LetterInfo X_SP = new("X", 10, 1);
    public static LetterInfo Y_SP = new("Y", 4, 1);
    public static LetterInfo Z_SP = new("Z", 10, 1);


    // 39 vowels, 64 consonants 38% vowel
    // vowels 36% https://en.wikipedia.org/wiki/Letter_frequency
    public static List<LetterInfo> letterInfosEN = new()
        { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z };

    public static List<LetterInfo> letterInfosSP = new() {
        A_SP, B_SP, C_SP, D_SP, E_SP, F_SP, G_SP, H_SP, I_SP, J_SP, K_SP, L_SP, LL_SP, M_SP, N_SP, Ñ_SP, O_SP, P_SP,
        Q_SP, R_SP, S_SP, T_SP, U_SP, V_SP, W_SP, X_SP, Y_SP, Z_SP
    };

    public static Dictionary<string, Dictionary<string, int>> letterDictionaryDictionary = new();

    private readonly int distribution;

    private readonly string theLetter;

    private readonly int value;

    static LetterInfo() {
        var letterDictionary = new Dictionary<string, int>();
        foreach (var letterInfo in letterInfosEN) {
            letterDictionary.Add(letterInfo.theLetter, letterInfo.value);
        }

        letterDictionaryDictionary.Add(MyPrefs.PREFS_LANG_EN, letterDictionary);
        letterDictionaryDictionary.Add(MyPrefs.PREFS_LANG_FR, letterDictionary);

        letterDictionary = new Dictionary<string, int>();
        foreach (var letterInfo in letterInfosSP) {
            letterDictionary.Add(letterInfo.theLetter, letterInfo.value);
        }


        letterDictionaryDictionary.Add(MyPrefs.PREFS_LANG_SP, letterDictionary);

        MonoBehaviour.print("~LetterInfo.LetterInfo letterDictionaryDictionary " +
                            letterDictionaryDictionary["SP"].Count + "\n");
    }

    private LetterInfo(string theLetter, int value, int distribution) {
        this.theLetter = theLetter;
        this.value = value;
        this.distribution = distribution;
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