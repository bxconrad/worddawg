using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour {
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text countdownText;

    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private AudioSource audioSource;
    private AudioClip clockTick;
    private float countdown;
    private AudioClip howl;
    private bool isSound;

    public void Awake() {
        howl = Resources.Load("dogHowlingAtMoon") as AudioClip;
        clockTick = Resources.Load("clockTick") as AudioClip;
        Initialize();
    }


    private void Update() {
        //print("CountdownTimer.Update " + isActiveAndEnabled + "\n");
        if (countdown <= 1 && isActiveAndEnabled) {
            enabled = false;
            audioSource.Stop();
            // bcHack. audio would not play in EndGame so we do it here.
            audioSource.PlayOneShot(howl);
            EndGame();
            return;
        }

        if (countdown < 16 && !isSound && isActiveAndEnabled) {
            print("CountdownTimer.Update play sound  mute? " + audioSource.mute + " \n");
            isSound = true;
            audioSource.PlayOneShot(clockTick);
        }

        countdown -= Time.deltaTime;
        SetText(NiceTime(countdown));
    }

    public void Startx() {
        gameManager.gameObject.SetActive(true);
        print("CountdownTimer.Start gmActive " + gameManager.gameObject.activeInHierarchy + " \n");
        enabled = false;
        SetText("");
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
        // countdown = 5;
        isSound = false;
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