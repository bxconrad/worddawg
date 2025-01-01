using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour {
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameParameters gameParameters;
    private float countdown;

    public void Start() {
        gameManager.gameObject.SetActive(true);
        enabled = false;
        SetText("");
    }


    private void Update() {
        //print("CountdownTimer.Update " + isActiveAndEnabled + "\n");
        if (countdown <= 1 && isActiveAndEnabled) {
            enabled = false;
            EndGame();
            return;
        }

        countdown -= Time.deltaTime;
        SetText(NiceTime(countdown));
    }

    private string NiceTime(float seconds) {
        var min = Mathf.FloorToInt(seconds / 60F);
        var sec = Mathf.FloorToInt(seconds - min * 60);
        var niceTime = string.Format("{0:0}:{1:00}", min, sec);
        return niceTime;
    }

    public void SetText(string aName) {
        countdownText.text = aName;
    }

    public void Initialize() {
        print("CountdownTimer.Initialize \n");
        SetText("");
        countdown = gameParameters.numSeconds;
    }

    public void EndTimer() {
        enabled = false;
        SetText("");
    }

    private void EndGame() {
        EndTimer();
        _ = gameManager.EndGame(); //bcdo use event
    }
}