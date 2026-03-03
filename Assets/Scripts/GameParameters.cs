using UnityEngine;

public class GameParameters : MonoBehaviour {
    public int numLetters { get; set; }
    public int numSeconds { get; set; }
    public string language { get; set; }
    public string gameMode { get; set; }
    public string userName { get; set; }
    public bool isTimed { get; set; }

    public bool isGameOfTheDay { get; set; }

    //  public bool isEndGame { get; set; }
    public int numRackLetters { get; set; }

    public int minimumLetters { get; set; }
    public int dealerSeed { get; set; }
    public int botLevel { get; set; }
    public bool isTwoPlayer { get; set; }


    public void Initialize() {
        language = MyPrefs.PREFS_LANG_EN;
        numSeconds = 0;
        numLetters = 0;
        isTimed = false;
        isGameOfTheDay = false;
        // isEndGame = false;
        numRackLetters = MyPrefs.DEFAULT_NUM_RACK_LETTERS;
        dealerSeed = 0;

        botLevel = Settings.GetBotLevel();
        isTwoPlayer = Settings.GetIsTwoPlayer();
        userName = Settings.GetUserName();
        //gameMode = ""; don't init gameMode, need it for same game replay
    }

    public override string ToString() {
        return
            $"{base.ToString()}, {nameof(dealerSeed)}: {dealerSeed}, {nameof(botLevel)}: {botLevel}, {nameof(isTwoPlayer)}: {isTwoPlayer}, {nameof(numLetters)}: {numLetters}, {nameof(numSeconds)}: {numSeconds}, {nameof(language)}: {language}, {nameof(gameMode)}: {gameMode}, {nameof(isTimed)}: {isTimed}, {nameof(isGameOfTheDay)}: {isGameOfTheDay},  {nameof(numRackLetters)}: {numRackLetters}, {nameof(minimumLetters)}: {minimumLetters}";
    }

    //qulogic
}