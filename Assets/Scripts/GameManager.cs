using System.Threading.Tasks;
using EasyUI.Toast;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("UI")] [SerializeField] private GameObject endGameContainer;
    [SerializeField] private GameObject prefsContainer;
    [SerializeField] private UpdateBoard updateBoard;
    [SerializeField] private CountdownTimer countdownTimer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Dealer dealer;
    [SerializeField] private Stats stats;
    [SerializeField] private HelpDisplay helpDisplay;
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private ValidatorManager validatorManager;
    [SerializeField] private TransformShaker transformShaker;
    [SerializeField] private LogoImage logoImage;

    private string gameMode;

    public void Start() {
        print("GameManager.Start " + endGameContainer + "\n");

        gameObject.SetActive(true);
        InactivateOtherCanvases();
        Toast.Dismiss();
        // end components
        countdownTimer.EndTimer();
        scoreManager.End();
        dealer.End();
        gameParameters.Initialize();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    private void InactivateOtherCanvases() {
        print("GameManager.InactivateOtherCanvases eg{" + endGameContainer + "}\n");
        // set all other canvases to inactive
        updateBoard.gameObject.SetActive(false);
        endGameContainer.SetActive(false);
        prefsContainer.SetActive(false);
        helpDisplay.Initialize();
    }

    public void Initialize() {
        print("GameManager.Initialize\n");
        Start();
    }

    public void NewGameOfTheDay() {
        print("GameManager.NewGameOfTheDay \n");
        countdownTimer.isTimerEnabled = false;
        //dealer.SetRandomSeed(DateTime.Today.DayOfYear);
        dealer.SetNumLetters(MyPrefs.DEFAULT_LETTERS);
        gameMode = Stats.PREFS_ST_MODE_GOTD;

        gameParameters.gameMode = Stats.PREFS_ST_MODE_GOTD;
        gameParameters.isGameOfTheDay = true;
        gameParameters.numLetters = MyPrefs.DEFAULT_LETTERS;
        //gameParameters.language = MyPrefs.PREFS_LANG_SP;
        NewGame();
    }

    public void NewUntimedGame() {
        print("GameManager.NewUntimedGame \n");
        countdownTimer.isTimerEnabled = false;
        dealer.SetNumLetters(MyPrefs.DEFAULT_LETTERS);
        gameMode = Stats.PREFS_ST_MODE_UNTIMED_50;
        NewGame();
    }

    public void NewTimedGame() {
        print("GameManager.NewTimedGame \n");
        countdownTimer.isTimerEnabled = true;
        countdownTimer.SetCountdown(4);
        dealer.SetNumLetters(0);
        gameMode = Stats.PREFS_ST_MODE_TIMED_4;
        NewGame();
    }

    public void NewCustomGamePrefs() {
        print("GameManager.NewCustomGamePrefs \n");
        Toast.Dismiss();
        prefsContainer.SetActive(true);
        gameObject.SetActive(false);
    }

    public void NewCustomGamePlay() {
        print("GameManager.NewCustomGamePlay \n");
        prefsContainer.SetActive(false);
        gameObject.SetActive(true);
        countdownTimer.isTimerEnabled = MyPrefs.IsTimer();
        countdownTimer.SetCountdown(MyPrefs.GetTimerDuration());
        if (!MyPrefs.IsTimer())
            dealer.SetNumLetters(MyPrefs.GetNumLetters());
        else
            dealer.SetNumLetters(0);
        gameMode = Stats.PREFS_ST_MODE_CUSTOM;

        gameParameters.language = MyPrefs.GetLanguage();
        gameParameters.isGameOfTheDay = MyPrefs.IsGameOfTheDay();
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
        print("GameManager.NewGame \n");
        InactivateOtherCanvases();

        scoreManager.Initialize();
        validatorManager.Initialize();
        countdownTimer.gameObject.SetActive(true);
        countdownTimer.Initialize();
        updateBoard.gameObject.SetActive(true);
        updateBoard.AwakeIt(); // bcdo fix but be careful
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
        await Spinit();
        gameParameters.isEndGame = true;
        InactivateOtherCanvases();
        countdownTimer.EndTimer();
        scoreManager.End();
        Toast.Dismiss();

        endGameContainer.SetActive(true);
        stats.UpdateStats(gameMode);
        print("GameManager.EndGame complete\n");
    }

    public void ExitApplication() {
        Debug.Log("GameManager.ExitApplication");
        Application.Quit();
    }

    public bool IsTimed() {
        return true.Equals(countdownTimer.isTimerEnabled);
    }
}