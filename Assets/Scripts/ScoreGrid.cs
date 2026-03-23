using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreGrid : MonoBehaviour {
    [SerializeField] public TMP_Text name;
    [SerializeField] public TMP_Text totalScore;
    [SerializeField] public Image image;

    public void Initialize() {
        name.text = "";
        totalScore.text = "";
    }
}