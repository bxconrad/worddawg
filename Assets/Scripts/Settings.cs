using TMPro;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

public class Settings : MonoBehaviour {
    [SerializeField] private TMP_Dropdown botLevelDropdown;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle twoPlayerToggle;
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] public TMP_InputField userNameText;

    private void Start() {
        soundToggle.onValueChanged.AddListener(delegate { SoundToggleValueChanged(soundToggle); });
        soundToggle.isOn = GetIsSound();
        userNameText.onValueChanged.AddListener(delegate { UserNameInputFieldValueChanged(userNameText); });
        userNameText.text = GetUserName();
        twoPlayerToggle.onValueChanged.AddListener(delegate { TwoPlayerToggleValueChanged(twoPlayerToggle); });
        twoPlayerToggle.isOn = GetIsTwoPlayer();
        botLevelDropdown.interactable = twoPlayerToggle.isOn;
        MyPrefs.BotSelectList(botLevelDropdown);
        print("~Settings.Start end " + PlayerPrefs.GetInt(MyPrefs.PREFS_RT_BOT_LEVEL) + "\n");
    }

    private void OnEnable() {
        print("~Settings.OnEnable  \n");
        Start();
    }


    private void BotLevelDropdown(int index) {
        //var difficulty = index + 1; // 1–5
        var botName = MyPrefs.BOT_NAMES[index];
        PlayerPrefs.SetInt(MyPrefs.PREFS_RT_BOT_LEVEL, index);
        Debug.Log("Settings.BotLevelDropdown Selected: " + botName + " (" + index + ")");
    }


    private void SoundToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(MyPrefs.PREFS_RT_IS_SOUND, val.ToString());
        print("~Settings.SoundToggleValueChanged " + val + " \n");
    }

    private void TwoPlayerToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(MyPrefs.PREFS_RT_IS_TWOPLAYER, val.ToString());
        botLevelDropdown.interactable = val;
        print("~Settings.TwoPlayerToggleValueChanged " + val + " \n");
    }

    private void UserNameInputFieldValueChanged(TMP_InputField val) {
        PlayerPrefs.SetString(MyPrefs.PREFS_RT_USER_NAME, val.text);
        //print("~Settings.UserNameInputFieldValueChanged " + val + " \n");
    }


    public static string GetUserName() {
        var userName = PlayerPrefs.GetString(MyPrefs.PREFS_RT_USER_NAME, MyPrefs.DEFAULT_USER_NAME);
        print("~Settings.GetUserName " + userName + " \n");
        return userName;
    }

    public static int GetBotLevel() {
        print("~Settings.GetBotLevel" + PlayerPrefs.GetInt(MyPrefs.PREFS_RT_BOT_LEVEL, MyPrefs.DEFAULT_BOT_LEVEL) +
              " \n");
        return PlayerPrefs.GetInt(MyPrefs.PREFS_RT_BOT_LEVEL, MyPrefs.DEFAULT_BOT_LEVEL);
    }


    public static bool GetIsSound() {
        var retVal = PlayerPrefs.GetString(MyPrefs.PREFS_RT_IS_SOUND, MyPrefs.DEFAULT_IS_SOUND).ToUpper()
            .Equals("TRUE");
        print("~Settings.GetIsSound " + retVal + " \n");
        return retVal;
    }


    public static bool GetIsTwoPlayer() {
        var retVal = PlayerPrefs.GetString(MyPrefs.PREFS_RT_IS_TWOPLAYER, MyPrefs.DEFAULT_IS_TWOPLAYER).ToUpper()
            .Equals("TRUE");
        print("~Settings.GetIsTwoPlayer " + retVal + " \n");
        return retVal;
    }
/*
    public static bool GetIsShowButtons() {
        return PlayerPrefs.GetString(MyPrefs.PREFS_RT_IS_SHOW_BUTTON, MyPrefs.DEFAULT_IS_SHOW_BUTTON).ToUpper()
            .Equals("TRUE");
    }
      public void OnResetPrefsButtonClicked() {
        print("~Settings.OnResetPrefsButtonClicked \n");
        foreach (var key in MyPrefs.PREFS_KEYS) {
            PlayerPrefs.DeleteKey(key);
        }

        Start();
    }


    public void OnResetStatsButtonClicked() {
        print("~Settings.OnResetStatsButtonClicked \n");

        foreach (var statKey in Stats.STAT_KEYS) {
            PlayerPrefs.DeleteKey(statKey);
            foreach (var gameMode in Stats.STAT_GAME_MODES) {
                var key = statKey + "_" + gameMode;
                PlayerPrefs.DeleteKey(key);
                print("~Settings.OnResetStatsButtonClicked " + key + " \n");
            }
        }

        //ResetPrefs();
    }
       private void ResetPrefs() {
        print("~Settings.ResetPrefs " + MyPrefs.GetNumRackLetters() + " \n");
        foreach (var key in MyPrefs.PREFS_KEYS) {
            PlayerPrefs.DeleteKey(key);
        }
    }

*/
}