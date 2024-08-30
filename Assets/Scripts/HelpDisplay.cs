using UnityEngine;

public class HelpDisplay : MonoBehaviour {
    [SerializeField] private GameObject mainHelp;
    [SerializeField] private GameObject gameHelp;
    [SerializeField] private GameObject scoreHelp;
    [SerializeField] private GameObject gestureHelp;

    private bool isGameActive;
    private bool isGestureActive;
    private bool isMainActive;
    private bool isScoreActive;

    public void Initialize() {
        mainHelp.SetActive(false);
        gameHelp.SetActive(false);
        scoreHelp.SetActive(false);
        gestureHelp.SetActive(false);
    }

    public void ShowHelp(string name) {
        Initialize();
        if ("MAIN".Equals(name)) {
            isMainActive = !isMainActive;
            mainHelp.SetActive(isMainActive);
            isGameActive = false;
            isGestureActive = false;
            isScoreActive = false;
        }
        else if ("GAME".Equals(name)) {
            isGameActive = !isGameActive;
            gameHelp.SetActive(isGameActive);
            isMainActive = false;
            isGestureActive = false;
            isScoreActive = false;
        }
        else if ("SCORE".Equals(name)) {
            isScoreActive = !isScoreActive;
            scoreHelp.SetActive(isScoreActive);
            isGameActive = false;
            isGestureActive = false;
            isMainActive = false;
        }
        if ("GESTURE".Equals(name)) {
            isGestureActive = !isGestureActive;
            gestureHelp.SetActive(isGestureActive);
            isGameActive = false;
            isMainActive = false;
            isScoreActive = false;
        }
    }
}