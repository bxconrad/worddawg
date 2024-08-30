using System;
using EasyUI.Toast;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("UI")] [SerializeField] private GameObject endGameContainer;
    [SerializeField] private GameObject prefsContainer;
    [SerializeField] private UpdateBoard updateBoard;
    [SerializeField] private CountdownTimer countdownTimer;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private LetterBag letterBag;
    [SerializeField] private Stats stats;
    [SerializeField] private HelpDisplay helpDisplay;

    private string gameMode;

    public void Start() {
        print("GameManager.Start " + endGameContainer + "\n");

        gameObject.SetActive(true);
        InactivateOtherCanvases();
        Toast.Dismiss();
        // end components
        countdownTimer.EndTimer();
        scoreManager.End();
        letterBag.End();
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
        letterBag.SetRandomSeed(DateTime.Today.DayOfYear);
        letterBag.SetNumLetters(MyPrefs.DEFAULT_LETTERS);
        gameMode = Stats.PREFS_ST_MODE_GOTD;
        NewGame();
    }

    public void NewUntimedGame() {
        print("GameManager.NewUntimedGame \n");
        countdownTimer.isTimerEnabled = false;
        letterBag.SetNumLetters(MyPrefs.DEFAULT_LETTERS);
        gameMode = Stats.PREFS_ST_MODE_UNTIMED_50;
        NewGame();
    }

    public void NewTimedGame() {
        print("GameManager.NewTimedGame \n");
        countdownTimer.isTimerEnabled = true;
        countdownTimer.SetCountdown(4);
        letterBag.SetNumLetters(600);
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
        letterBag.SetNumLetters(MyPrefs.GetNumLetters());
        gameMode = Stats.PREFS_ST_MODE_CUSTOM;

        NewGame();
    }

    public void RepeatGame() {
        print("GameManager.RepeatGame \n");
        if (Stats.PREFS_ST_MODE_CUSTOM.Equals(gameMode)) {
            NewCustomGamePrefs();
        }
        else if (Stats.PREFS_ST_MODE_GOTD.Equals(gameMode)) {
            NewGameOfTheDay();
        }
        else if (Stats.PREFS_ST_MODE_TIMED_4.Equals(gameMode)) {
            NewTimedGame();
        }
        else if (Stats.PREFS_ST_MODE_UNTIMED_50.Equals(gameMode)) {
            NewUntimedGame();
        }
        else {
            print("GameManager.RepeatGame Unknown gameMode " + gameMode);
        }
    }

    private void NewGame() {
        print("GameManager.NewGame \n");
        InactivateOtherCanvases();

        scoreManager.Initialize();
        countdownTimer.gameObject.SetActive(true);
        countdownTimer.Initialize();
        updateBoard.gameObject.SetActive(true);
        updateBoard.AwakeIt(); // bcdo fix but be careful
        updateBoard.NewGame();
        gameObject.SetActive(false);
    }

    public void EndGame() {
        print("GameManager.EndGame\n");
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
}