using EasyUI.Toast;
using UnityEngine;

public class EndGamePanelLayout : MonoBehaviour {
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject endGameButtons;
    private bool isPortrait = true;

    private void Update() {
        if (Screen.height >= Screen.width && !isPortrait) {
            DisplayPortrait();
        }
        else if (Screen.height < Screen.width && isPortrait) {
            DisplayLandscape();
        }
    }

    private void DisplayPortrait() {
        print("EndGamePanelLayout.DisplayPortrait \n");
        isPortrait = true;
        // reparent helpLayout to left panel
        endGameButtons.transform.SetParent(leftPanel.transform, false);
        rightPanel.SetActive(false);
        UpdateBoard.toastPosition = ToastPosition.BottomCenter;
    }


    private void DisplayLandscape() {
        print("EndGamePanelLayout.DisplayLandscape w=" + Screen.width + " h=" + Screen.height + "\n");
        isPortrait = false;
        rightPanel.SetActive(true);
        // reparent helpLayout to right panel
        endGameButtons.transform.SetParent(rightPanel.transform, false);
        UpdateBoard.toastPosition = ToastPosition.BottomRight;
    }
}