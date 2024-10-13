using UnityEngine;
using Random = System.Random;

public class ComplimentHandler {
    private static readonly Random rnd = new();
    public static ComplimentHandler instance = new();
    private string[] complimentArray;

    private ComplimentHandler() {
        InitializeCompliments();
    }

    public void InitializeCompliments() {
        //Debug("ComplimentHandler.InitializeCompliments\n");
        var textFile = Resources.Load("compliments") as TextAsset;
        complimentArray = textFile.text.Split('\n');
        foreach (var cc in complimentArray) {
            var r = rnd.Next(complimentArray.Length);
            var compliment = complimentArray[r];
            // Debug("ComplimentHandler.InitializeCompliments compliment " + compliment + " " + r + "\n");
        }
        // Debug("ComplimentHandler.InitializeCompliments complimentArray " + complimentArray.Length + "\n");
    }

    public string GetRandomCompliment() {
        //  Debug("ComplimentHandler.GetRandomCompliment\n");
        var r = rnd.Next(complimentArray.Length);
        var compliment = complimentArray[r];
        return compliment;
    }
}