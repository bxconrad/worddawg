public class GameHelper {
    public static string ContractDoubleLetter(string word, string lang) {
        return HandleDoubleLetter(false, word, lang);
    }

    public static string ExpandDoubleLetter(string word, string lang) {
        return HandleDoubleLetter(true, word, lang);
    }

    private static string HandleDoubleLetter(bool isAdding, string word, string lang) {
        if (isAdding) {
            word = word.Replace("Q", "QU");
            if (MyPrefs.PREFS_LANG_SP.Equals(lang))
                word = word.Replace("*", "LL");
        }
        else {
            word = word.Replace("QU", "Q");
            if (MyPrefs.PREFS_LANG_SP.Equals(lang))
                word = word.Replace("LL", "*");
        }

        return word;
    }
}