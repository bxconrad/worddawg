using System;
using System.Collections.Generic;
using EasyUI.Toast;
using TMPro;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

public class MyPrefs : MonoBehaviour {
    public static readonly string PREFS_LANG_SP = "SP";
    public static readonly string PREFS_LANG_EN = "EN";

    public static readonly int DEFAULT_DURATION = 4;
    private static readonly string DEFAULT_IS_TIMER = "TRUE";
    public static readonly string DEFAULT_IS_SHOW_BUTTON = "TRUE";
    private static readonly string DEFAULT_IS_GOTD = "TRUE";
    public static readonly string DEFAULT_IS_SOUND = "TRUE";
    public static readonly string DEFAULT_IS_TWOPLAYER = "FALSE";
    private static readonly string DEFAULT_LANG = PREFS_LANG_EN;
    public static readonly int DEFAULT_NUM_LETTERS = 50;
    public static readonly int DEFAULT_NUM_RACK_LETTERS = 7;
    public static readonly int DEFAULT_BOT_LEVEL = 0;
    public static readonly string DEFAULT_USER_NAME = "User1";
    public static readonly int DEFAULT_DEALER_SEED = DateTime.Today.DayOfYear;

    private static readonly string PREFS_RT_DURATION = "RT_DURATION";
    public static readonly string PREFS_RT_IS_SHOW_BUTTON = "RT_IS_SHOW_BUTTON";
    public static readonly string PREFS_RT_IS_SOUND = "RT_IS_SOUND";
    private static readonly string PREFS_RT_IS_GOTD = "RT_IS_GOTD";
    private static readonly string PREFS_RT_IS_TIMER = "RT_IS_TIMER";
    public static readonly string PREFS_RT_IS_TWOPLAYER = "RT_IS_TWOPLAYER";
    private static readonly string PREFS_RT_LANGUAGE = "RT_LANGUAGE";
    private static readonly string PREFS_RT_LETTERS = "RT_LETTERS";
    private static readonly string PREFS_RT_RACK_LETTERS = "RT_RACK_LETTERS";
    public static readonly string PREFS_RT_BOT_LEVEL = "RT_BOT_LEVEL";
    public static readonly string PREFS_RT_DEALER_SEED = "RT_DEALER_SEED";
    public static readonly string PREFS_RT_USER_NAME = "RT_USER_NAME";

    public static readonly string[] PREFS_KEYS = {
        PREFS_RT_BOT_LEVEL, PREFS_RT_DURATION, PREFS_RT_IS_GOTD, PREFS_RT_IS_SHOW_BUTTON,
        PREFS_RT_IS_SOUND, PREFS_RT_IS_TIMER, PREFS_RT_IS_TWOPLAYER,
        PREFS_RT_LANGUAGE, PREFS_RT_LETTERS, PREFS_RT_RACK_LETTERS, PREFS_RT_USER_NAME
    };

    public static Dictionary<int, string> BOT_NAMES = new() {
        { 0, "LittleBot" },
        { 1, "DinoBot" },
        { 2, "RaptorBot" },
        { 3, "TRexBot" },
        { 4, "BotZilla" }
    };

    [SerializeField] private TMP_Dropdown durationDropdown;
    [SerializeField] private TMP_Dropdown letterDropdown;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private TMP_Dropdown rackLettersDropdown;
    [SerializeField] private TMP_Dropdown botLevelDropdown;
    [SerializeField] private Toggle timerToggle;
    [SerializeField] private Toggle gameOfTheDayToggle;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle twoPlayerToggle;
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] public TMP_InputField dealerSeed;

    private void Start() {
        print("MyPrefs.Start " + PlayerPrefs.GetString(PREFS_RT_IS_TIMER) + " \n");
        Toast.Dismiss();
        timerToggle.onValueChanged.AddListener(delegate { TimerToggleValueChanged(timerToggle); });
        timerToggle.isOn = GetIsTimer();

        gameOfTheDayToggle.onValueChanged.AddListener(delegate { GameOfTheDayToggleValueChanged(gameOfTheDayToggle); });
        gameOfTheDayToggle.isOn = GetIsGOTD();
        dealerSeed.text = GetDealerSeed().ToString();
        dealerSeed.interactable = !gameOfTheDayToggle.isOn;

        rackLettersDropdown.value = GetNumRackLetters() - 7;
        // print("MyPrefs.Start rackLettersDropdown" + rackLettersDropdown.value + " \n");

        languageDropdown.value = GetLanguage() == PREFS_LANG_SP ? 1 : 0;
        ShowTimerOnOff();

        soundToggle.onValueChanged.AddListener(delegate { SoundToggleValueChanged(soundToggle); });
        soundToggle.isOn = GetIsSound();
        twoPlayerToggle.onValueChanged.AddListener(delegate { TwoPlayerToggleValueChanged(twoPlayerToggle); });
        twoPlayerToggle.isOn = GetIsTwoPlayer();
        BotSelectList(botLevelDropdown);
        LetterSelectList();
        print("MyPrefs.Start end\n");
    }


    private void OnEnable() {
        print("MyPrefs.OnEnable  \n");
        Start();
    }

    public void DealerSeedInputFieldValueChanged(TMP_InputField val) {
        print($"Settings.DealerSeedInputFieldValueChanged start {val.text}  \n");
        if (int.TryParse(val.text, out var intValue)) {
            PlayerPrefs.SetInt(PREFS_RT_DEALER_SEED, intValue);
        }
        else {
            val.text = "0";
        }

        print($"Settings.DealerSeedInputFieldValueChanged {val.text}  \n");
    }


    public static int GetDealerSeed() {
        var adealerSeed = PlayerPrefs.GetInt(PREFS_RT_DEALER_SEED, DEFAULT_DEALER_SEED);
        print("Settings.GetDealerSeed " + adealerSeed + " \n");
        return adealerSeed;
    }

    public static void BotSelectList(TMP_Dropdown myBotlevelDropdown) {
        myBotlevelDropdown.ClearOptions();
        var options = new List<string>();
        for (var i = 0; i < BOT_NAMES.Count; i++) {
            options.Add(BOT_NAMES[i]);
        }

        myBotlevelDropdown.AddOptions(options);
        myBotlevelDropdown.value = PlayerPrefs.GetInt(PREFS_RT_BOT_LEVEL);
        myBotlevelDropdown.RefreshShownValue();
    }


    private void BotLevelDropdown(int index) {
        //var difficulty = index + 1; // 1–5
        var botName = BOT_NAMES[index];
        PlayerPrefs.SetInt(PREFS_RT_BOT_LEVEL, index);
        Debug.Log("Settings.BotLevelDropdown Selected: " + botName + " (" + index + ")");
        //GetBotLevel();
    }


    public void DurationDropdown(int index) {
        print("MyPrefs.DurationDropdown " + index + " \n");
        PlayerPrefs.SetInt(PREFS_RT_DURATION, index + 1);
    }

    public void LetterSelectList() {
        letterDropdown.ClearOptions();
        List<string> options = new() { "25", "50", "75", "100", "125", "150", "175", "200" };
        letterDropdown.AddOptions(options);
        letterDropdown.value = GetNumLetters() / 25 - 1;
        print("MyPrefs.Start LetterDropdown " + letterDropdown.value + " \n");
        letterDropdown.RefreshShownValue();
    }

    public void LetterDropdown(int index) {
        var numLetters = (index + 1) * 25;
        print("MyPrefs.LetterDropdown " + index + "  nl " + numLetters + " \n");
        PlayerPrefs.SetInt(PREFS_RT_LETTERS, numLetters);
    }

    public void RackLettersDropdown(int index) {
        var numLetters = index + 7;
        print("MyPrefs.RackLettersDropdown " + index + "  nl " + numLetters + " \n");
        PlayerPrefs.SetInt(PREFS_RT_RACK_LETTERS, numLetters);
    }

    public void xBotLevelDropdown(int index) {
        print("MyPrefs.BotLevelDropdown " + index + " \n");
        PlayerPrefs.SetInt(PREFS_RT_BOT_LEVEL, index);
        GetBotLevel();
    }

    public void LanguageDropdown(int option) {
        var lang = option == 1 ? PREFS_LANG_SP : PREFS_LANG_EN;
        print("MyPrefs.LanguageDropdown " + option + " lang " + lang + " \n");
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

    private void TwoPlayerToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(PREFS_RT_IS_TWOPLAYER, val.ToString());
        botLevelDropdown.interactable = val;
        print("MyPrefs.TwoPlayerToggleValueChanged " + val + " \n");
    }

    private void GameOfTheDayToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(PREFS_RT_IS_GOTD, val.ToString());
        dealerSeed.interactable = !val;
        print("MyPrefs.GameOfTheDayToggleValueChanged " + val + " \n");
    }

    private void ShowTimerOnOff() {
        var isTimerActive = PlayerPrefs.GetString(PREFS_RT_IS_TIMER, DEFAULT_IS_TIMER).ToUpper().Equals("TRUE");
        print("MyPrefs.ShowTimerOnOff isTimerActive " + isTimerActive + " \n");

        durationDropdown.interactable = isTimerActive;
        letterDropdown.interactable = !isTimerActive;
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

    public static int GetBotLevel() {
        print("MyPrefs.GetBotLevel" + PlayerPrefs.GetInt(PREFS_RT_BOT_LEVEL, DEFAULT_BOT_LEVEL) + " \n");
        return PlayerPrefs.GetInt(PREFS_RT_BOT_LEVEL, DEFAULT_BOT_LEVEL);
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

    private static bool GetIsSound() {
        var retVal = PlayerPrefs.GetString(PREFS_RT_IS_SOUND, DEFAULT_IS_SOUND).ToUpper()
            .Equals("TRUE");
        print("MyPrefs.GetIsSound " + retVal + " \n");
        return retVal;
    }


    public static bool GetIsTimer() {
        return PlayerPrefs.GetString(PREFS_RT_IS_TIMER, DEFAULT_IS_TIMER).ToUpper().Equals("TRUE");
    }

    public static bool GetIsTwoPlayer() {
        return PlayerPrefs.GetString(PREFS_RT_IS_TWOPLAYER, DEFAULT_IS_TWOPLAYER).ToUpper().Equals("TRUE");
    }

    public static bool GetIsShowButtons() {
        return PlayerPrefs.GetString(PREFS_RT_IS_SHOW_BUTTON, DEFAULT_IS_SHOW_BUTTON).ToUpper()
            .Equals("TRUE");
    }
}