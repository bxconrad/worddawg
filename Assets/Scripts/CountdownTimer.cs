using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour {
    [Header("UI")] [SerializeField] private GameManager gameManager;

    [SerializeField] private TMP_Text countdownText;
    private bool? _isTimerEnabled;
    private float countdown;
    private GameObject statRows;

    public bool? isTimerEnabled {
        get => _isTimerEnabled == null ? MyPrefs.IsTimer() : _isTimerEnabled;
        set {
            print("CountdownTimer.isTimerEnabled {" + value + "}\n");
            _isTimerEnabled = value;
            if (value != null) {
                enabled = (bool)value;
            }
        }
    }

    public void Start() {
        gameManager.gameObject.SetActive(true);
    }


    private void Update() {
        //print("CountdownTimer.Update " + isActiveAndEnabled + "\n");
        if (countdown <= 1 && isActiveAndEnabled) {
            EndGame();
            return;
        }

        countdown -= Time.deltaTime;
        var min = Mathf.FloorToInt(countdown / 60F);
        var sec = Mathf.FloorToInt(countdown - min * 60);
        var niceTime = string.Format("{0:0}:{1:00}", min, sec);

        SetText(niceTime);
    }


    public void SetText(string name) {
        countdownText.text = name;
    }

    public void SetCountdown(int minutes) {
        countdown = minutes * 60;
    }

    public void Initialize() {
        print("CountdownTimer.Initialize \n");
        SetText("");
        if (countdown == 0) {
            countdown = MyPrefs.GetTimerDuration() * 60;
        }
    }

    public void EndTimer() {
        isTimerEnabled = false;
        SetText("");
    }

    private void EndGame() {
        EndTimer();
        gameManager.EndGame(); //bcdo use event
    }
}