using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePanelLayout : MonoBehaviour {
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private ScrollRect scrollRect;
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
        print("UpdatePanelLayout.DisplayPortrait \n");
        isPortrait = true;
        // reparent wordList to left panel
        scrollRect.transform.SetParent(leftPanel.transform, false);
        rightPanel.SetActive(false);
        UpdateBoard.toastPosition = ToastPosition.BottomCenter;
    }


    private void DisplayLandscape() {
        print("UpdatePanelLayout.DisplayLandscape w=" + Screen.width + " h=" + Screen.height + "\n");
        isPortrait = false;
        rightPanel.SetActive(true);
        // reparent wordList to right panel
        scrollRect.transform.SetParent(rightPanel.transform, false);
        UpdateBoard.toastPosition = ToastPosition.BottomRight;
    }
}