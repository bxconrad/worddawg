using EasyUI.Toast;
using TMPro;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

public class MyPrefs : MonoBehaviour {
    public static int DEFAULT_DURATION = 4;
    public static int DEFAULT_LETTERS = 50;
    public static string DEFAULT_IS_TIMER = "TRUE";
    public static string DEFAULT_IS_SHOW_BUTTON = "TRUE";
    public static string PREFS_RT_DURATION = "RT_DURATION";
    public static string PREFS_RT_LETTERS = "RT_LETTERS";
    public static string PREFS_RT_IS_TIMER = "RT_IS_TIMER";
    public static string PREFS_RT_IS_SHOW_BUTTON = "RT_IS_SHOW_BUTTON";
    public static int NUM_RACK_LETTERS = 7;

    public static readonly string[] PREFS_KEYS = {
        PREFS_RT_LETTERS, PREFS_RT_IS_TIMER, PREFS_RT_DURATION, PREFS_RT_IS_SHOW_BUTTON
    };

    [SerializeField] private TMP_Dropdown durationDropdown;
    [SerializeField] private TMP_Dropdown letterDropdown;
    [SerializeField] private Toggle showButtonsToggle;
    [SerializeField] private Toggle timerToggle;

    private void Start() {
        timerToggle.onValueChanged.AddListener(delegate { TimerToggleValueChanged(timerToggle); });
        timerToggle.isOn = PlayerPrefs.GetString(PREFS_RT_IS_TIMER, DEFAULT_IS_TIMER).ToUpper().Equals("TRUE");
        print("MyPrefs.Start " + PlayerPrefs.GetString(PREFS_RT_IS_TIMER) + " \n");

        showButtonsToggle.onValueChanged.AddListener(delegate { ShowButtonsToggleValueChanged(showButtonsToggle); });
        showButtonsToggle.isOn = PlayerPrefs.GetString(PREFS_RT_IS_SHOW_BUTTON, DEFAULT_IS_SHOW_BUTTON).ToUpper()
            .Equals("TRUE");

        durationDropdown.value = PlayerPrefs.GetInt(PREFS_RT_DURATION, DEFAULT_DURATION) - 1;
        letterDropdown.value = PlayerPrefs.GetInt(PREFS_RT_LETTERS, DEFAULT_LETTERS) / 50 - 1;

        ShowTimerOnOff();
        Toast.Dismiss();
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

    private void TimerToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(PREFS_RT_IS_TIMER, val.ToString());
        ShowTimerOnOff();
        print("MyPrefs.TimerToggleValueChanged " + val + " \n");
    }

    private void ShowButtonsToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(PREFS_RT_IS_SHOW_BUTTON, val.ToString());
        print("MyPrefs.ShowButtonsToggleValueChanged " + val + " \n");
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

    public static int GetTimerDuration() {
        return PlayerPrefs.GetInt(PREFS_RT_DURATION, DEFAULT_DURATION);
    }

    public static int GetNumLetters() {
        var numLetters = DEFAULT_LETTERS;
        if (!IsTimer()) {
            numLetters = PlayerPrefs.GetInt(PREFS_RT_LETTERS, DEFAULT_LETTERS);
        }

        return numLetters;
    }

    public static bool IsShowButtons() {
        return PlayerPrefs.GetString(PREFS_RT_IS_SHOW_BUTTON, DEFAULT_IS_SHOW_BUTTON).ToUpper().Equals("TRUE");
    }

    public static bool IsTimer() {
        return PlayerPrefs.GetString(PREFS_RT_IS_TIMER, DEFAULT_IS_TIMER).ToUpper().Equals("TRUE");
    }
}