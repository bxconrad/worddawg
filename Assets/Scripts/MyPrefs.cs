using EasyUI.Toast;
using TMPro;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

public class MyPrefs : MonoBehaviour {
    public static string PREFS_LANG_SP = "SP";
    public static string PREFS_LANG_EN = "EN";

    public static int DEFAULT_DURATION = 4;
    public static readonly string DEFAULT_IS_TIMER = "TRUE";
    private static readonly string DEFAULT_IS_SHOW_BUTTON = "TRUE";
    private static readonly string DEFAULT_IS_GOTD = "TRUE";
    private static readonly string DEFAULT_IS_SOUND = "TRUE";
    public static string DEFAULT_LANG = PREFS_LANG_SP;
    public static readonly int DEFAULT_NUM_LETTERS = 50;
    public static readonly int DEFAULT_NUM_RACK_LETTERS = 7;

    public static string PREFS_RT_DURATION = "RT_DURATION";
    private static readonly string PREFS_RT_IS_SHOW_BUTTON = "RT_IS_SHOW_BUTTON";
    private static readonly string PREFS_RT_IS_SOUND = "RT_IS_SOUND";
    public static string PREFS_RT_IS_GOTD = "RT_IS_GOTD";
    public static string PREFS_RT_IS_TIMER = "RT_IS_TIMER";
    public static string PREFS_RT_LANGUAGE = "RT_LANGUAGE";
    public static string PREFS_RT_LETTERS = "RT_LETTERS";
    public static string PREFS_RT_RACK_LETTERS = "RT_RACK_LETTERS";

    public static readonly string[] PREFS_KEYS = {
        PREFS_RT_DURATION, PREFS_RT_IS_SHOW_BUTTON, PREFS_RT_IS_GOTD, PREFS_RT_IS_TIMER, PREFS_RT_LANGUAGE,
        PREFS_RT_LETTERS, PREFS_RT_RACK_LETTERS
    };

    [SerializeField] private TMP_Dropdown durationDropdown;
    [SerializeField] private TMP_Dropdown letterDropdown;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private TMP_Dropdown rackLettersDropdown;
    [SerializeField] private Toggle timerToggle;
    [SerializeField] private Toggle gameOfTheDayToggle;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private GameParameters gameParameters;

    private void Start() {
        print("MyPrefs.Start " + PlayerPrefs.GetString(PREFS_RT_IS_TIMER) + " \n");
        Toast.Dismiss();
        timerToggle.onValueChanged.AddListener(delegate { TimerToggleValueChanged(timerToggle); });
        timerToggle.isOn = GetIsTimer();

        gameOfTheDayToggle.onValueChanged.AddListener(delegate { GameOfTheDayToggleValueChanged(gameOfTheDayToggle); });
        gameOfTheDayToggle.isOn = GetIsGOTD();

        durationDropdown.value = GetDuration() - 1;
        letterDropdown.value = GetNumLetters() / 50 - 1;
        print("MyPrefs.Start LetterDropdown" + letterDropdown.value + " \n");
        rackLettersDropdown.value = GetNumRackLetters() - 7;
        // print("MyPrefs.Start rackLettersDropdown" + rackLettersDropdown.value + " \n");

        languageDropdown.value = GetLanguage() == PREFS_LANG_SP ? 1 : 0;
        ShowTimerOnOff();

        soundToggle.onValueChanged.AddListener(delegate { SoundToggleValueChanged(soundToggle); });
        soundToggle.isOn = GetIsSound();
        print("MyPrefs.Start end\n");
    }


    public void DurationDropdown(int index) {
        print("MyPrefs.DurationDropdown " + index + " \n");
        PlayerPrefs.SetInt(PREFS_RT_DURATION, index + 1);
    }

    public void LetterDropdown(int index) {
        var numLetters = (index + 1) * 50;
        print("MyPrefs.LetterDropdown " + index + "  nl " + numLetters + " \n");
        PlayerPrefs.SetInt(PREFS_RT_LETTERS, numLetters);
    }

    public void RackLettersDropdown(int index) {
        var numLetters = index + 7;
        print("MyPrefs.RackLettersDropdown " + index + "  nl " + numLetters + " \n");
        PlayerPrefs.SetInt(PREFS_RT_RACK_LETTERS, numLetters);
    }

    public void LanguageDropdown(int option) {
        print("MyPrefs.LanguageDropdown " + option + " \n");

        var lang = option == 1 ? PREFS_LANG_SP : PREFS_LANG_EN;
        PlayerPrefs.SetString(PREFS_RT_LANGUAGE, lang);
    }

    private void TimerToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(PREFS_RT_IS_TIMER, val.ToString());
        gameParameters.isTimed = val;
        ShowTimerOnOff();
        print("MyPrefs.TimerToggleValueChanged " + val + " \n");
    }

    private void SoundToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(PREFS_RT_IS_SOUND, val.ToString());
        print("MyPrefs.SoundToggleValueChanged " + val + " \n");
    }

    private void GameOfTheDayToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(PREFS_RT_IS_GOTD, val.ToString());
        print("MyPrefs.GameOfTheDayToggleValueChanged " + val + " \n");
    }

    private void ShowTimerOnOff() {
        var isTimerActive = PlayerPrefs.GetString(PREFS_RT_IS_TIMER, DEFAULT_IS_TIMER).ToUpper().Equals("TRUE");
        print("MyPrefs.ShowTimerOnOff isTimerActive " + isTimerActive + " \n");

        durationDropdown.gameObject.SetActive(isTimerActive);
        letterDropdown.gameObject.SetActive(!isTimerActive);
    }

    public void OnResetPrefsButtonClicked() {
        print("MyPrefs.OnResetPrefsButtonClicked \n");
        foreach (var key in PREFS_KEYS) {
            PlayerPrefs.DeleteKey(key);
        }

        Start();
    }


    public static string GetLanguage() {
        return PlayerPrefs.GetString(PREFS_RT_LANGUAGE, DEFAULT_LANG);
    }

    public static int GetNumRackLetters() {
        return PlayerPrefs.GetInt(PREFS_RT_RACK_LETTERS, DEFAULT_NUM_RACK_LETTERS);
    }

    public static int GetNumLetters() {
        return PlayerPrefs.GetInt(PREFS_RT_LETTERS, DEFAULT_NUM_LETTERS);
    }

    public static int GetDuration() {
        return PlayerPrefs.GetInt(PREFS_RT_DURATION, DEFAULT_DURATION) * 60;
    }

    public static bool GetIsGOTD() {
        return PlayerPrefs.GetString(PREFS_RT_IS_GOTD, DEFAULT_IS_GOTD).ToUpper()
            .Equals("TRUE");
    }

    public static bool GetIsSound() {
        var retVal = PlayerPrefs.GetString(PREFS_RT_IS_SOUND, DEFAULT_IS_SOUND).ToUpper()
            .Equals("TRUE");
        print("MyPrefs.GetIsSound " + retVal + " \n");
        return retVal;
    }


    public static bool GetIsTimer() {
        return PlayerPrefs.GetString(PREFS_RT_IS_TIMER, DEFAULT_IS_TIMER).ToUpper().Equals("TRUE");
    }

    public static bool GetIsShowButtons() {
        return PlayerPrefs.GetString(PREFS_RT_IS_SHOW_BUTTON, DEFAULT_IS_SHOW_BUTTON).ToUpper()
            .Equals("TRUE");
    }
}