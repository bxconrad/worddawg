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
    [SerializeField] private GameObject timeScorePanel;
    [SerializeField] private UpdateBoard updateBoard;
    [SerializeField] private CountdownTimer countdownTimer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private BetterRack betterRack;
    [SerializeField] private Stats stats;
    [SerializeField] private HelpDisplay helpDisplay;
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private TransformShaker transformShaker;
    [SerializeField] private LogoImage logoImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI countdownText;

    private readonly List<Image> panelImages = new();

//        -diag-temp-memory-leak-validation
    private string gameMode;

    public void Start() {
        Toast.Dismiss();

        gameObject.SetActive(true);
        InactivateOtherCanvases();
        // end components
        countdownTimer.EndTimer();
        scoreManager.End();
        gameParameters.Initialize();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        print("GameManager.Start sound " + MyPrefs.GetIsSound() + " " + gameParameters + "\n");
    }


    private void InactivateOtherCanvases() {
        print("GameManager.InactivateOtherCanvases eg{" + endGameContainer + "}\n");
        // set all other canvases to inactive
        updateBoard.gameObject.SetActive(false);
        endGameContainer.SetActive(false);
        prefsContainer.SetActive(false);
        timeScorePanel.SetActive(false);
        helpDisplay.Initialize();
    }

    public void Initialize() {
        print("GameManager.Initialize\n");
        Start();
    }

    public void NewGameOfTheDay() {
        print("GameManager.NewGameOfTheDay \n");
        gameMode = Stats.PREFS_ST_MODE_GOTD;

        gameParameters.gameMode = Stats.PREFS_ST_MODE_GOTD;
        gameParameters.isGameOfTheDay = true;
        gameParameters.numLetters = MyPrefs.DEFAULT_NUM_LETTERS;
        NewGame();
    }

    public void NewUntimedGame() {
        print("GameManager.NewUntimedGame \n");
        gameMode = Stats.PREFS_ST_MODE_UNTIMED_50;

        gameParameters.isTimed = false;
        gameParameters.numLetters = MyPrefs.DEFAULT_NUM_LETTERS;
        gameParameters.gameMode = Stats.PREFS_ST_MODE_UNTIMED_50;
        NewGame();
    }

    public void NewTimedGame() {
        print("GameManager.NewTimedGame \n");
        gameMode = Stats.PREFS_ST_MODE_TIMED_4;

        gameParameters.isTimed = true;
        gameParameters.numSeconds = MyPrefs.DEFAULT_DURATION * 60;
        gameParameters.numLetters = 999;
        gameParameters.gameMode = Stats.PREFS_ST_MODE_TIMED_4;

        NewGame();
    }

    public void NewCustomGamePrefs() {
        print("GameManager.NewCustomGamePrefs \n");
        Toast.Dismiss();
        prefsContainer.SetActive(true);
        gameObject.SetActive(false);
    }

    private void InitializeCustomGameParameters() {
        print("GameManager.InitializeCustomGameParameters  isSound {" + MyPrefs.GetIsSound() + "} \n");

        gameParameters.gameMode = Stats.PREFS_ST_MODE_CUSTOM;
        gameParameters.isTimed = MyPrefs.GetIsTimer();
        gameParameters.isGameOfTheDay = MyPrefs.GetIsGOTD();
        gameParameters.numSeconds = MyPrefs.GetDuration();
        gameParameters.numLetters = MyPrefs.GetNumLetters();
        gameParameters.language = MyPrefs.GetLanguage();
        gameParameters.numRackLetters = MyPrefs.GetNumRackLetters();
        audioSource.mute = !MyPrefs.GetIsSound();

        print("GameManager.InitializeCustomGameParameters gameParameters " + gameParameters + " \n");
    }

    public void NewCustomGamePlay() {
        print("GameManager.NewCustomGamePlay \n");
        prefsContainer.SetActive(false);
        gameObject.SetActive(true);
        gameMode = Stats.PREFS_ST_MODE_CUSTOM;
        InitializeCustomGameParameters();
        NewGame();
    }

    public void RepeatGame() {
        print("GameManager.RepeatGame \n");
        if (Stats.PREFS_ST_MODE_CUSTOM.Equals(gameMode))
            NewCustomGamePlay();
        else if (Stats.PREFS_ST_MODE_GOTD.Equals(gameMode))
            NewGameOfTheDay();
        else if (Stats.PREFS_ST_MODE_TIMED_4.Equals(gameMode))
            NewTimedGame();
        else if (Stats.PREFS_ST_MODE_UNTIMED_50.Equals(gameMode))
            NewUntimedGame();
        else
            print("GameManager.RepeatGame Unknown gameMode " + gameMode);
    }

    private void NewGame() {
        print("GameManager.NewGame " + gameParameters + "\n");
        InactivateOtherCanvases();

        scoreManager.Initialize();
        //validatorManager.Initialize();
        betterRack.Initialize();
        countdownTimer.enabled = false;
        timeScorePanel.SetActive(true);
        if (gameParameters.isTimed) {
            countdownTimer.Initialize();
            countdownTimer.gameObject.SetActive(true);
            countdownTimer.enabled = true;
            countdownText.text = "Countdown";
        }
        else {
            countdownText.text = "# Letters";
        }

        audioSource.mute = !MyPrefs.GetIsSound();
        GameHelper.LANGUAGE = MyPrefs.GetLanguage();
        updateBoard.gameObject.SetActive(true);
        updateBoard.NewGame();
        gameObject.SetActive(false);
    }

    private async Task Spinit() {
        var tasks = new Task[2];
        tasks[0] = transformShaker.ASpin(logoImage.transform, .24f, 18, 3, false);
        tasks[1] = transformShaker.ASpin(updateBoard.transform, .48f, 9, 1, true);

        await Task.WhenAll(tasks);
    }


    public async Task EndGame() {
        print("GameManager.EndGame\n");
        Toast.Dismiss();
        await Spinit();
        gameParameters.isEndGame = true;
        InactivateOtherCanvases();
        countdownTimer.EndTimer();
        scoreManager.End();

        endGameContainer.SetActive(true);
        stats.UpdateStats(gameMode);
        print("GameManager.EndGame complete\n");
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