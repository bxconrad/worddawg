using EasyUI.Toast;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePanelLayout : MonoBehaviour {
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private Canvas canvas;
    [SerializeField] private ScrollRect scrollRect;
    private CanvasScaler canvasScaler;
    private GridLayoutGroup gridLayout;
    private bool isPortrait;
    private bool isRotating;
    private WordGrid wordGrid;
    private RectTransform wordListScrollRect;


    public void Start() {
        print("UpdatePanelLayout.Start\n");
        wordGrid = GetComponentInChildren<WordGrid>();
        gridLayout = wordGrid.GetComponent<GridLayoutGroup>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        wordListScrollRect = scrollRect.GetComponent<RectTransform>();
        RecalculateWordGridLayout();
    }

    private void Update() {
        if (Screen.height >= Screen.width && !isPortrait) {
            DisplayPortrait();
            RecalculateWordGridLayout();
        }
        else if (Screen.height < Screen.width && isPortrait) {
            DisplayLandscape();
            RecalculateWordGridLayout();
        }
    }

    private void RecalculateWordGridLayout() {
        var rowCount = gridLayout.constraintCount;
        var scale = canvasScaler.referenceResolution;

        Vector3 cellSize = gridLayout.cellSize;
        Vector3 spacing = gridLayout.spacing;

        var childWidth = (scale.x - spacing.x * (rowCount - 1)) / (float)(rowCount * 1.1);
        cellSize.x = childWidth;
        gridLayout.cellSize = cellSize;
    }


    private void DisplayPortrait() {
        print("UpdatePanelLayout.DisplayPortrait \n");
        isPortrait = true;

        // set up wordList in left panel
        // offsetMin is Left/Bottom 
        // offset max is Right*-1/Top 
        wordListScrollRect.offsetMin = new Vector2(0, 120);
        wordListScrollRect.offsetMax = new Vector2(0, -700);
        scrollRect.transform.SetParent(leftPanel.transform, false);

        rightPanel.SetActive(false);
        UpdateBoard.toastPosition = ToastPosition.BottomCenter;
    }


    private void DisplayLandscape() {
        print("UpdatePanelLayout.DisplayLandscape w=" + Screen.width + " h=" + Screen.height + "\n");
        isPortrait = false;
        // offsetMin is Left/Bottom 
        // offset max is Right*-1/Top (right is negative in editor so positive here)
        // To set Bottom same as editor it is (Bottom- offsetMax.b)
        // so if you want bottom 940 and offsetMax.b = -700 set offsetMin.b=-1640

        // Set up right panel
        rightPanel.SetActive(true);
        // set up wordList in right panel
        wordListScrollRect.offsetMin = new Vector2(0, 0);
        wordListScrollRect.offsetMax = new Vector2(0, -150);
        // reparent wordList to right panel
        scrollRect.transform.SetParent(rightPanel.transform, false);
        UpdateBoard.toastPosition = ToastPosition.BottomRight;
    }
}