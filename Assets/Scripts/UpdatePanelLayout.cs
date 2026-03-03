using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePanelLayout : MonoBehaviour {
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject wordGridVerticalLayout;
    private bool isFirst = true;
    private bool isPortrait = true;
    private bool isUpdateWordGrid = true;

    private void Update() {
        //UpdateWWordGrid must be done on subsequent update DisplayPortrait/Landscape to get correct updated widths
        if (isUpdateWordGrid) {
            //    UpdateWordGrid();
        }

        isUpdateWordGrid = true;
        if (isFirst || (Screen.height >= Screen.width && !isPortrait)) {
            DisplayPortrait();
            isFirst = false;
        }
        else if (isFirst || (Screen.height < Screen.width && isPortrait)) {
            DisplayLandscape();
            isFirst = false;
        }
        else {
            isUpdateWordGrid = false;
        }
    }

    private void DisplayPortrait() {
        print("UpdatePanelLayout.DisplayPortrait \n"); // + leftPanel.GetComponent<RectTransform>().rect.width + "\n");
        isPortrait = true;
        // reparent wordList to left panel
        rightPanel.SetActive(false);
        // scrollRect.transform.SetParent(leftPanel.transform, false);
        wordGridVerticalLayout.transform.SetParent(leftPanel.transform, false);
        UpdateBoard.toastPosition = ToastPosition.BottomCenter;
    }

    // viewPortImage was not resetting width. 
    // set controloChildSize.width=true on RightUpdatePanel
    private void UpdateWordGrid() {
        var parent = isPortrait ? leftPanel : rightPanel;

        var width = parent.GetComponent<RectTransform>().rect.width;
        var newWidth = width * .9 / 2;

        var gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        var newSize = new Vector2((float)newWidth, gridLayoutGroup.cellSize.y);
        print("UpdatePanelLayout.UpdateWordGrid " +
              " isPportrait=" + isPortrait +
              " newWidth=" + newWidth +
              " screenWidthw=" + Screen.width +
              " leftPanel " + leftPanel.GetComponent<RectTransform>().rect.width +
              " rightPanel " + rightPanel.GetComponent<RectTransform>().rect.width + "\n");
        gridLayoutGroup.cellSize = newSize;
    }

    private void DisplayLandscape() {
        print("UpdatePanelLayout.DisplayLandscape w=" + Screen.width + " h=" + Screen.height + "\n");
        isPortrait = false;
        rightPanel.SetActive(true);
        // reparent wordList to right panel
        // scrollRect.transform.SetParent(rightPanel.transform, false);
        wordGridVerticalLayout.transform.SetParent(rightPanel.transform, false);

        UpdateBoard.toastPosition = ToastPosition.BottomRight;
    }
}