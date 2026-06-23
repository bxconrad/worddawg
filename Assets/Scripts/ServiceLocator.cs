using UnityEngine;
using UnityEngine.UI;

public class ServiceLocator : MonoBehaviour {
    public static ServiceLocator instance;

    [SerializeField] public GameObject updateButtons;
    [SerializeField] public ToastMaster toastMaster;
    [SerializeField] public ScrollRect scrollRect;
    [SerializeField] public GameObject dummyPanel;
    [SerializeField] public GameObject logoImage2;
    [SerializeField] public TransformShaker transformShaker;
    [SerializeField] public Dealer dealer;
    [SerializeField] public GameParameters gameParameters;

    [SerializeField] public WordGrid wordGrid1;
    [SerializeField] public WordGrid wordGrid2;
    [SerializeField] public ScoreGrid scoreGrid1;
    [SerializeField] public ScoreGrid scoreGrid2;
    [SerializeField] public GameObject wordGridScrollPanel2;
    [SerializeField] public CanvasGroup scorePanelCanvasGroup;
    [SerializeField] public GameObject scorePanel;
    [SerializeField] public InputWord inputWord;
    [SerializeField] public SelectedWord selectedWord;
    [SerializeField] public GameObject timeScorePanel;
    [SerializeField] public UpdateBoardButtonHandler replaceRackButton;
    [SerializeField] public DictionaryManager dictionaryManager;
    public UpdateBoardAbstract updateBoard;

    private void Awake() {
        print("~ServiceLocator.Awake\n");
        instance = this;
    }
}