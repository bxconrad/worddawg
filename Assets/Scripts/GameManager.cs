using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyUI.Toast;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [Header("UI")] [SerializeField] private GameObject endGameContainer;
    [SerializeField] private GameObject prefsContainer;
    [SerializeField] private GameObject settingsContainer;
    [SerializeField] private GameObject settingsWidget;
    [SerializeField] private GameObject timeScorePanel;
    [SerializeField] private GameObject logoImage2;
    [SerializeField] private UpdateBoard updateBoard;
    [SerializeField] private CountdownTimer countdownTimer;

    [SerializeField] private ToastMaster toastMaster;

    [SerializeField] private BetterRack betterRack;
    [SerializeField] private Stats stats;
    [SerializeField] private StatsTwoPlayer statsTwoPlayer;
    [SerializeField] private HelpDisplay helpDisplay;
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private TransformShaker transformShaker;
    [SerializeField] private LogoImage logoImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Toggle twoPlayerToggle;

    private readonly List<Image> panelImages = new();

    private Player player;
    private Player player2;

    private string savedGameMode;

    public void Start() {
        Toast.Dismiss();
        gameObject.SetActive(true);
        InactivateOtherCanvases();
        settingsWidget.SetActive(true);
        // end components
        countdownTimer.EndTimer();
        toastMaster.End();
        gameParameters.Initialize();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        print("~GameManager.Start sound " + Settings.GetIsSound() + " " + gameParameters + "\n");
    }


    private void InactivateOtherCanvases() {
        print("~GameManager.InactivateOtherCanvases eg{" + endGameContainer + "}\n");
        // set all other canvases to inactive
        updateBoard.gameObject.SetActive(false);
        endGameContainer.SetActive(false);
        prefsContainer.SetActive(false);
        timeScorePanel.SetActive(false);
        settingsContainer.SetActive(false);
        logoImage2.SetActive(false);
        helpDisplay.Initialize();
    }

    public void Initialize() {
        print("~GameManager.Initialize\n");
        Start();
    }

    public void NewGameOfTheDay() {
        print("~GameManager.NewGameOfTheDay \n");
        gameParameters.Initialize();
        savedGameMode = Stats.PREFS_ST_MODE_GOTD;
        gameParameters.gameMode = Stats.PREFS_ST_MODE_GOTD;
        gameParameters.isGameOfTheDay = true;
        gameParameters.numLetters = gameParameters.isTwoPlayer
            ? MyPrefs.DEFAULT_NUM_LETTERS_TWO_PLAYER
            : MyPrefs.DEFAULT_NUM_LETTERS;
        gameParameters.dealerSeed = DateTime.Today.DayOfYear;
        NewGame();
    }

    public void NewUntimedGame() {
        print("~GameManager.NewUntimedGame \n");
        gameParameters.Initialize();
        savedGameMode = Stats.PREFS_ST_MODE_UNTIMED_50;
        gameParameters.gameMode = Stats.PREFS_ST_MODE_UNTIMED_50;
        gameParameters.isTimed = false;
        gameParameters.numLetters = gameParameters.isTwoPlayer
            ? MyPrefs.DEFAULT_NUM_LETTERS_TWO_PLAYER
            : MyPrefs.DEFAULT_NUM_LETTERS;
        NewGame();
    }

    public void NewTimedGame() {
        print("~GameManager.NewTimedGame \n");
        gameParameters.Initialize();
        savedGameMode = Stats.PREFS_ST_MODE_TIMED_4;
        gameParameters.gameMode = Stats.PREFS_ST_MODE_TIMED_4;
        gameParameters.isTimed = true;
        gameParameters.numSeconds = MyPrefs.DEFAULT_DURATION * 60;
        gameParameters.numLetters = 999;

        NewGame();
    }

    public void NewCustomGamePrefs() {
        print("~GameManager.NewCustomGamePrefs \n");
        Toast.Dismiss();
        prefsContainer.SetActive(true);
        gameObject.SetActive(false);
    }


    private void InitializeCustomGameParameters() {
        print("~GameManager.InitializeCustomGameParameters  isSound {" + Settings.GetIsSound() + "} \n");
        gameParameters.Initialize();
        gameParameters.gameMode = Stats.PREFS_ST_MODE_CUSTOM;
        gameParameters.isTimed = MyPrefs.GetIsTimer();
        gameParameters.isGameOfTheDay = MyPrefs.GetIsGOTD();
        if (gameParameters.isGameOfTheDay) {
            gameParameters.dealerSeed = DateTime.Today.DayOfYear;
        }
        else {
            gameParameters.dealerSeed = MyPrefs.GetDealerSeed();
        }

        gameParameters.numSeconds = MyPrefs.GetDuration();
        gameParameters.numLetters = MyPrefs.GetNumLetters();
        gameParameters.numRackLetters = MyPrefs.GetNumRackLetters();
        gameParameters.language = MyPrefs.GetLanguage();
        gameParameters.botLevel = MyPrefs.GetBotLevel();
        gameParameters.isTwoPlayer = MyPrefs.GetIsTwoPlayer();
        if (gameParameters.language == null) {
            gameParameters.language = MyPrefs.PREFS_LANG_EN; //? it happens
            print("~GameManager.InitializeCustomGameParameters ?? lanuage null set to EN " + gameParameters.language +
                  " \n");
        }

        print("~GameManager.InitializeCustomGameParameters gameParameters " + gameParameters + " \n");
    }

    public void NewCustomGamePlay() {
        print("~GameManager.NewCustomGamePlay \n");
        prefsContainer.SetActive(false);
        gameObject.SetActive(true);
        savedGameMode = Stats.PREFS_ST_MODE_CUSTOM;
        InitializeCustomGameParameters();
        NewGame();
    }

    public void OpenSettings() {
        print("~GameManager.OpenSettings \n");
        var isGameManagerActive = settingsContainer.activeSelf;
        InactivateOtherCanvases();
        settingsContainer.SetActive(!isGameManagerActive);
        gameObject.SetActive(isGameManagerActive);
    }

    public void RepeatGame() {
        print("~GameManager.RepeatGame \n");
        if (Stats.PREFS_ST_MODE_CUSTOM.Equals(gameParameters.gameMode))
            NewCustomGamePlay();
        else if (Stats.PREFS_ST_MODE_GOTD.Equals(gameParameters.gameMode))
            NewGameOfTheDay();
        else if (Stats.PREFS_ST_MODE_TIMED_4.Equals(gameParameters.gameMode))
            NewTimedGame();
        else if (Stats.PREFS_ST_MODE_UNTIMED_50.Equals(gameParameters.gameMode))
            NewUntimedGame();
        else
            print("~GameManager.RepeatGame Unknown gameMode " + gameParameters.gameMode);
    }

    private void NewGame() {
        print("~GameManager.NewGame " + gameParameters + "\n");
        InactivateOtherCanvases();
        betterRack.Initialize();
        countdownTimer.enabled = false;
        //bcdo move to updateBoard?
        timeScorePanel.SetActive(!gameParameters.isTwoPlayer);
        if (gameParameters.isTimed) {
            print("~GameManager.NewGame isTimed " + gameParameters.isTimed + "\n");
            countdownTimer.Initialize();
            countdownTimer.Startx();
            countdownTimer.gameObject.SetActive(true);
            countdownTimer.enabled = true;
            countdownText.text = "Countdown";
        }
        else {
            countdownText.text = "# Letters";
        }

        audioSource.mute = !Settings.GetIsSound();
        GameHelper.LANGUAGE = gameParameters.language;
        updateBoard.gameObject.SetActive(true);
        gameObject.SetActive(false);
        // player = new Player(gameParameters.userName);
        // player2 = new Player("dummy");

        updateBoard.NewGame();
        settingsWidget.SetActive(false);
        print("~GameManager.NewGame done \n");
    }

    private async Task Spinit() {
        var tasks = new Task[2];
        var transforms = new[] { logoImage.transform, logoImage2.transform };
        tasks[0] = transformShaker.ABeginRandomSpins(transforms, .3f, 16);
//        tasks[0] = transformShaker.ASpin(transforms, .24f, 18, 3, false);
        tasks[1] = transformShaker.ASpin(updateBoard.transform, .48f, 9, 1, true);

        await Task.WhenAll(tasks);
    }


    public async Task EndGame() {
        print("~GameManager.EndGame\n");
        Toast.Dismiss();
        updateBoard.EndGame();
        await Spinit();
        InactivateOtherCanvases();
        countdownTimer.EndTimer();
        toastMaster.End();

        endGameContainer.SetActive(true);
        var players = updateBoard.GetPlayers();
        if (gameParameters.isTwoPlayer) {
            stats.gameObject.SetActive(false);
            statsTwoPlayer.gameObject.SetActive(true);
            logoImage2.SetActive(true);
            statsTwoPlayer.UpdateStats(gameParameters.gameMode, players[1], players[0]);
        }
        else {
            statsTwoPlayer.gameObject.SetActive(false);
            stats.gameObject.SetActive(true);
            stats.UpdateStats(gameParameters.gameMode, players[0]);
        }

        print("~GameManager.EndGame complete\n");
    }

    public void ExitApplication() {
        Debug.Log("GameManager.ExitApplication");
        // UpdateColors();
        Application.Quit();
    }

    public void OpenHowToPlayVideo() {
        Application.OpenURL("https://sites.google.com/view/worddawg/howtoplayvideo");
    }

    private void UpdateColors() {
        var canvas = GetComponentInParent<Canvas>();
        panelImages.Add(GameObject.FindGameObjectWithTag("header").GetComponent<Image>());
        panelImages.Add(GameObject.FindGameObjectWithTag("endGamePanel").GetComponent<Image>());
        panelImages.Add(canvas.GetComponentInChildren<GameManager>().GetComponent<Image>());
        var ub = canvas.GetComponentInChildren<UpdateBoard>();
        ub.enabled = true;
        var ubi = ub.GetComponent<Image>();
        panelImages.Add(ubi);

        var wg = canvas.GetComponentInChildren<WordGrid>();
        var wgi = wg.gameObject.transform.parent.GetComponent<Image>();
        panelImages.Add(wgi);
        foreach (var image in panelImages) {
            image.color = Color.red;
        }

        var buttons = canvas.GetComponentInChildren<GameManager>().GetComponentsInChildren<Button>();
        var buttons2 = ub.GetComponentsInChildren<Button>();
        buttons.Concat(buttons2);
        foreach (var button in buttons) {
            var image = button.GetComponent<Image>();
            image.color = Color.blue;
        }

        foreach (var button in buttons2) {
            var image = button.GetComponent<Image>();
            image.color = Color.blue;
        }
    }
}