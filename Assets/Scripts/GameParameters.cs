using UnityEngine;

public class GameParameters : MonoBehaviour {
    public int numLetters { get; set; }
    public int numSeconds { get; set; }
    public string language { get; set; }
    public string gameMode { get; set; }
    public bool isTimed { get; set; }
    public bool isGameOfTheDay { get; set; }
    public bool isEndGame { get; set; }
    public int numRackLetters { get; set; }

    public int minimumLetters { get; set; }


    public void Initialize() {
        language = MyPrefs.PREFS_LANG_EN;
        numSeconds = 0;
        numLetters = 0;
        isTimed = false;
        isGameOfTheDay = false;
        isEndGame = false;
        numRackLetters = MyPrefs.DEFAULT_NUM_RACK_LETTERS;

        //gameMode = ""; don't init gameMode, need it for same game replay
    }

    public override string ToString() {
        return $"{base.ToString()}, numLetters: {numLetters}, numSeconds: {numSeconds}," +
               $" language: {language}, gameMode: {gameMode}, isTimed: {isTimed}, isGameOfTheDay: {isGameOfTheDay}, " +
               $"isEndGame: {isEndGame},  numRackLetters: {numRackLetters}";
    }

    //qulogic
}