using EasyUI.Toast;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    [SerializeField] private TransformShaker transformShaker;
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private LogoImage logoImage;
    [SerializeField] private LogoImage logoImage2;
    [SerializeField] private AudioSource audioSource;
    public TMP_Text scoreText;

    private readonly Color toastColor = new(0, .5f, 0, 1);
    private AudioClip howl;
    private AudioClip labBark;
    private int letterScore;
    private Transform[] logoImages;
    private AudioClip woof;

    public void Awake() {
        howl = Resources.Load("dogHowlingAtMoon") as AudioClip;
        woof = Resources.Load("dogWoof") as AudioClip;
        labBark = Resources.Load("labradorBarkingShort") as AudioClip;
        logoImages = new[] { logoImage.transform, logoImage2.transform };
    }


    public void End() {
        scoreText.text = "";
    }

    public void UpdateScoreText(Player player) {
        scoreText.text = player.currentWord != null
            ? player.currentScore.ToString().PadRight(5) + player.currentWord.currentWordHistory.score
            : "0     0";
    }

    public void ShowToastMessage(Player player) {
        var wordContents = player.currentWord.GetCurrentContents();
        var wordScore = player.currentWord.currentWordHistory.score;
        var msg = "";
        var toastTime = 15f;
        Toast.Dismiss();
        if (player.currentWord.currentWordHistory.isDogBonusWord) {
            print("ScoreManager.SendToastMessage howl ");
            audioSource.PlayOneShot(howl);
            msg = "Arooo! Special Word Dawg Bonus for " + wordContents + "!!!\n";
            _ = transformShaker.ABeginRandomSpins(logoImages, .3f, 4);

            print("ScoreManager.SendToastMessage IsDogBonusWord ");
        }

        // If entire rack is used
        if (wordContents.Length - player.currentWord.GetPreviousContents().Length >= gameParameters.numRackLetters) {
            print("ScoreManager.SendToastMessage 100 bonus ");
            audioSource.PlayOneShot(howl);
            msg += "100 Point Bonus for using all letters!!! Great Job!";
            _ = transformShaker.ABeginRandomSpins(logoImages, .3f, 4);
        }

        if (wordScore > 100 && msg.Equals("")) {
            toastTime = 2f;

            if (wordScore > 200) {
                audioSource.PlayOneShot(labBark);
            }
            else {
                audioSource.PlayOneShot(woof);
            }

            msg = wordScore + " points! " + ComplimentHandler.instance.GetRandomCompliment();
            _ = transformShaker.ABeginRandomSpins(logoImages, .3f, 2);
        }

        if (msg.Equals("")) {
            if (player.numWords < 3 && player.numChangedWords == 0)
                msg = "See if you can modify " + wordContents + ". Select " + wordContents +
                      " from the list of words. You must use ALL the letters in " + wordContents +
                      " plus at least ONE letter from the rack.";
            else if (player.numChangedWords == 1 && player.currentWord.GetPreviousContents().Length > 0)
                msg = "Congratulations! You turned " + player.currentWord.GetPreviousContents() + " into " +
                      wordContents +
                      ". And you scored " +
                      wordScore + " points.\n\n Well done!";
        }

        if (wordScore > 10 && msg.Equals("")) {
            toastTime = 2f;
            msg = ComplimentHandler.instance.GetRandomCompliment();
        }

        print("ScoreManager.ShowToastMessage msg " + msg + "\n");

        if (!msg.Equals("")) Toast.Show(msg, toastTime, toastColor, GameHelper.GetToastPosition());
    }
}