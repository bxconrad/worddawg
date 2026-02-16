public class GameHelper {
    public static string LANGUAGE = MyPrefs.PREFS_LANG_EN;

    public static string ContractDoubleLetter(string word) {
        return HandleDoubleLetter(false, word);
    }

    public static string ExpandDoubleLetter(string word) {
        return HandleDoubleLetter(true, word);
    }

    private static string HandleDoubleLetter(bool isAdding, string word) {
        if (isAdding) {
            if (!word.Contains("QU")) {
                word = word.Replace("Q", "QU");
            }

            if (MyPrefs.PREFS_LANG_SP.Equals(LANGUAGE))
                word = word.Replace("*", "LL");
        }
        else {
            word = word.Replace("QU", "Q");
            if (MyPrefs.PREFS_LANG_SP.Equals(LANGUAGE))
                word = word.Replace("LL", "*");
        }

        return word;
    }
}