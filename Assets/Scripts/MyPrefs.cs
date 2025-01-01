using EasyUI.Toast;
using TMPro;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

public class MyPrefs : MonoBehaviour {
    public static int DEFAULT_DURATION = 4;
    public static int DEFAULT_LETTERS = 50;
    public static string DEFAULT_IS_TIMER = "TRUE";
    public static string DEFAULT_IS_SHOW_BUTTON = "TRUE";
    public static string DEFAULT_IS_GOTD = "TRUE";
    public static string PREFS_RT_DURATION = "RT_DURATION";
    public static string PREFS_RT_LANGUAGE = "RT_LANGUAGE";
    public static string PREFS_RT_LETTERS = "RT_LETTERS";
    public static string PREFS_RT_IS_TIMER = "RT_IS_TIMER";
    public static string PREFS_RT_IS_GOTD = "RT_IS_GOTD";
    public static string PREFS_RT_IS_SHOW_BUTTON = "RT_IS_SHOW_BUTTON";
    public static int NUM_RACK_LETTERS = 7;
    public static string PREFS_LANG_SP = "SP";
    public static string PREFS_LANG_EN = "EN";
    public static string DEFAULT_LANG = PREFS_LANG_SP;

    public static readonly string[] PREFS_KEYS = {
        PREFS_RT_LETTERS, PREFS_RT_IS_TIMER, PREFS_RT_DURATION, PREFS_RT_IS_SHOW_BUTTON, PREFS_RT_IS_GOTD
    };

    [SerializeField] private TMP_Dropdown durationDropdown;
    [SerializeField] private TMP_Dropdown letterDropdown;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Toggle showButtonsToggle;
    [SerializeField] private Toggle timerToggle;
    [SerializeField] private Toggle gameOfTheDayToggle;
    [SerializeField] private GameParameters gameParameters;

    private void Start() {
        print("MyPrefs.Start " + PlayerPrefs.GetString(PREFS_RT_IS_TIMER) + " \n");
        timerToggle.onValueChanged.AddListener(delegate { TimerToggleValueChanged(timerToggle); });
        timerToggle.isOn = PlayerPrefs.GetString(PREFS_RT_IS_TIMER, DEFAULT_IS_TIMER).ToUpper().Equals("TRUE");
        print("MyPrefs.Start2 " + PlayerPrefs.GetString(PREFS_RT_IS_TIMER) + " ttog " + timerToggle + " \n");


        gameOfTheDayToggle.onValueChanged.AddListener(delegate { GameOfTheDayToggleValueChanged(gameOfTheDayToggle); });
        gameOfTheDayToggle.isOn = PlayerPrefs.GetString(PREFS_RT_IS_GOTD, DEFAULT_IS_GOTD).ToUpper()
            .Equals("TRUE");

        durationDropdown.value = PlayerPrefs.GetInt(PREFS_RT_DURATION, DEFAULT_DURATION) - 1;
        letterDropdown.value = PlayerPrefs.GetInt(PREFS_RT_LETTERS, DEFAULT_LETTERS) / 50 - 1;
        languageDropdown.value = 2; //PlayerPrefs.GetString(PREFS_RT_LANGUAGE, DEFAULT_LANG);

        ShowTimerOnOff();
        Toast.Dismiss();

        showButtonsToggle.onValueChanged.AddListener(delegate { ShowButtonsToggleValueChanged(showButtonsToggle); });
        showButtonsToggle.isOn = PlayerPrefs.GetString(PREFS_RT_IS_SHOW_BUTTON, DEFAULT_IS_SHOW_BUTTON).ToUpper()
            .Equals("TRUE");
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

    private void ShowButtonsToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(PREFS_RT_IS_SHOW_BUTTON, val.ToString());
        print("MyPrefs.ShowButtonsToggleValueChanged " + val + " \n");
    }

    private void GameOfTheDayToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(PREFS_RT_IS_GOTD, val.ToString());
        print("MyPrefs.GameOfTheDayToggleValueChanged " + val + " \n");
    }

    private void ShowTimerOnOff() {
        var isTimerActive = PlayerPrefs.GetString(PREFS_RT_IS_TIMER, DEFAULT_IS_TIMER).ToUpper().Equals("TRUE");
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


    public static int GetNumLetters() {
        var numLetters = DEFAULT_LETTERS;
        if (!IsTimer()) numLetters = PlayerPrefs.GetInt(PREFS_RT_LETTERS, DEFAULT_LETTERS);

        return numLetters;
    }

    public static bool IsShowButtons() {
        return PlayerPrefs.GetString(PREFS_RT_IS_SHOW_BUTTON, DEFAULT_IS_SHOW_BUTTON).ToUpper().Equals("TRUE");
    }

    public static bool IsTimer() {
        return PlayerPrefs.GetString(PREFS_RT_IS_TIMER, DEFAULT_IS_TIMER).ToUpper().Equals("TRUE");
    }
}