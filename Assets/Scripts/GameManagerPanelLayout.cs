using UnityEngine;

public class GameManagerPanelLayout : MonoBehaviour {
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject helpLayout;
    private bool isFirst = true;
    private bool isPortrait = true;

    private void Update() {
        // isFirst is needed to force correct display if starting in landscape. (?)
        if (isFirst || (Screen.height >= Screen.width && !isPortrait)) {
            isFirst = false;
            DisplayPortrait();
        }
        else if (Screen.height < Screen.width && isPortrait) {
            DisplayLandscape();
        }
    }

    private void DisplayPortrait() {
        print("GameManagerPanelLayout.DisplayPortrait \n");
        isPortrait = true;
        // reparent helpLayout to left panel
        helpLayout.transform.SetParent(leftPanel.transform, false);
        rightPanel.SetActive(false);
    }


    private void DisplayLandscape() {
        print("GameManagerPanelLayout.DisplayLandscape w=" + Screen.width + " h=" + Screen.height + "\n");
        isPortrait = false;
        rightPanel.SetActive(true);
        // reparent helpLayout to right panel
        helpLayout.transform.SetParent(rightPanel.transform, false);
    }
}