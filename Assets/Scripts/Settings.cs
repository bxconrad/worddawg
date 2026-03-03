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
        botLevelDropdown.value = PlayerPrefs.GetInt(MyPrefs.PREFS_RT_BOT_LEVEL);
        soundToggle.onValueChanged.AddListener(delegate { SoundToggleValueChanged(soundToggle); });
        soundToggle.isOn = GetIsSound();
        twoPlayerToggle.onValueChanged.AddListener(delegate { TwoPlayerToggleValueChanged(twoPlayerToggle); });
        userNameText.onValueChanged.AddListener(delegate { UserNameInputFieldValueChanged(userNameText); });
        userNameText.text = GetUserName();
        twoPlayerToggle.isOn = GetIsTwoPlayer();
        print("Settings.Start end " + PlayerPrefs.GetInt(MyPrefs.PREFS_RT_BOT_LEVEL) + "\n");
    }


    public void BotLevelDropdown(int index) {
        print("Settings.BotLevelDropdown " + index + " \n");
        PlayerPrefs.SetInt(MyPrefs.PREFS_RT_BOT_LEVEL, index);
    }


    private void SoundToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(MyPrefs.PREFS_RT_IS_SOUND, val.ToString());
        print("Settings.SoundToggleValueChanged " + val + " \n");
    }

    private void TwoPlayerToggleValueChanged(Toggle toggle) {
        var val = toggle.isOn;
        PlayerPrefs.SetString(MyPrefs.PREFS_RT_IS_TWOPLAYER, val.ToString());
        print("Settings.TwoPlayerToggleValueChanged " + val + " \n");
    }

    private void UserNameInputFieldValueChanged(TMP_InputField val) {
        PlayerPrefs.SetString(MyPrefs.PREFS_RT_USER_NAME, val.text);
        print("Settings.UserNameInputFieldValueChanged " + val + " \n");
    }


    public void OnResetPrefsButtonClicked() {
        print("Settings.OnResetPrefsButtonClicked \n");
        foreach (var key in MyPrefs.PREFS_KEYS) {
            PlayerPrefs.DeleteKey(key);
        }

        Start();
    }


    public static string GetUserName() {
        var userName = PlayerPrefs.GetString(MyPrefs.PREFS_RT_USER_NAME, MyPrefs.DEFAULT_USER_NAME);
        print("Settings.GetUserName " + userName + " \n");
        return userName;
    }

    public static int GetBotLevel() {
        print("Settings.GetBotLevel" + PlayerPrefs.GetInt(MyPrefs.PREFS_RT_BOT_LEVEL, MyPrefs.DEFAULT_BOT_LEVEL) +
              " \n");
        return PlayerPrefs.GetInt(MyPrefs.PREFS_RT_BOT_LEVEL, MyPrefs.DEFAULT_BOT_LEVEL);
    }


    public static bool GetIsSound() {
        var retVal = PlayerPrefs.GetString(MyPrefs.PREFS_RT_IS_SOUND, MyPrefs.DEFAULT_IS_SOUND).ToUpper()
            .Equals("TRUE");
        print("Settings.GetIsSound " + retVal + " \n");
        return retVal;
    }


    public static bool GetIsTwoPlayer() {
        return PlayerPrefs.GetString(MyPrefs.PREFS_RT_IS_TWOPLAYER, MyPrefs.DEFAULT_IS_TWOPLAYER).ToUpper()
            .Equals("TRUE");
    }

    public static bool GetIsShowButtons() {
        return PlayerPrefs.GetString(MyPrefs.PREFS_RT_IS_SHOW_BUTTON, MyPrefs.DEFAULT_IS_SHOW_BUTTON).ToUpper()
            .Equals("TRUE");
    }
}