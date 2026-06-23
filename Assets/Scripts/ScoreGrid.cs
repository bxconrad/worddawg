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

    public void DisplayActiveTurn() {
        image.color = Color.white;
        name.color = Color.black;
        name.fontStyle = FontStyles.Bold;
        // name.fontStyle = FontStyles.UpperCase;
        totalScore.color = Color.black;
        totalScore.fontStyle = FontStyles.Bold;
    }

    public void DisplayInactiveTurn() {
        var myColor = new Color(.5f, .5f, .5f, 1.0f);
        image.color = Color.black;
        name.color = myColor; // Color.tan;
        name.fontStyle = FontStyles.Normal;

        totalScore.color = myColor; //
        totalScore.fontStyle = FontStyles.Normal;
    }
}