using UnityEngine;

public class Tile : MonoBehaviour {
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