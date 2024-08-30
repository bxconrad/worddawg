using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {
    private Image fill;
    private Outline outline;
    private TextMeshProUGUI text;

    public string letter { get; private set; }
    public State state { get; private set; }

    private void Awake() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        fill = GetComponent<Image>();
        outline = GetComponent<Outline>();
        //   print("Tile.Start text " + text + " fill " + fill + "\n");
    }

    public void SetLetter(string inLetter) {
        //print("Tiles.SetLetter text " + text + " inLetter " + inLetter);
        letter = inLetter.ToUpper();
        text.text = letter;
    }

    public void SetLetter(char letter) {
        SetLetter(letter.ToString());
    }

    public void SetState(State state) {
        this.state = state;
        // print("Tile.SetState " + fill + "  outline " + outline + "\n");
        if (fill != null)
            fill.color = state.fillColor;
        else
            print("Tile.SetState *** uh oh the fill is gone \n");
        if (outline != null)
            outline.effectColor = state.outlineColor;
    }

    public bool IsSelected() {
        return state != null && State.SELECTED.Equals(state.name);
    }

    public bool IsUnselected() {
        return state != null && State.UNSELECTED.Equals(state.name);
    }

    public bool IsEmpty() {
        return "".Equals(letter);
    }

    public void Clear() {
        letter = "";
        state = null;
    }

    [Serializable]
    public class State {
        public static string SELECTED = "SELECTED";
        public static string UNSELECTED = "UNSELECTED";

        public static State unselectedState = new(Color.blue, Color.yellow, UNSELECTED);
        public static State selectedState = new(Color.gray, Color.gray, SELECTED);


        public Color fillColor;
        public Color outlineColor;

        public State(Color fillColor, Color outlineColor, string name) {
            this.fillColor = fillColor;
            this.outlineColor = outlineColor;
            this.name = name;
        }

        public string name { get; set; }
    }
}