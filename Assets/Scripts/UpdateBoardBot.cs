using System.Collections.Generic;
using bot;

public class UpdateBoardBot : UpdateBoardTwo {
    private BrucesBotAbstract brucesBot;
    private int saveBotLevel = -1;


    protected override void NewGameInitializePlayers() {
        base.NewGameInitializePlayers();
        players[0].isBot = true;
    }

    protected override void NextTurn() {
        base.NextTurn();
        if (currentPlayer.isBot) {
            CallPlayerBot();
        }
    }

    private void CallPlayerBot() {
        print("~UpdateBoardBot.CallPlayerBot begin \n");
        var words = new List<Word>();
        foreach (var player in players) {
            words.AddRange(player.wordGrid.FindWordObjects());
        }

        var bestResultMatch = brucesBot.FindBestestWord(words, betterRack.GetWord());
        print($"~~~UpdateBoardBot.CallPlayerBot bestResultMatch {bestResultMatch}\n");

        print($"~UpdateBoardBot.CallPlayerBot betterRack {betterRack} \n");
        if (!string.IsNullOrEmpty(bestResultMatch.GeneratedWord)) {
            AutomateWordEntry(bestResultMatch.SourceObject, bestResultMatch.GeneratedWord);
        }
        else {
            print($"~UpdateBoardBot.CallPlayerBot  No word Found. Rack {betterRack.GetWord()} call ReplaceRack \n");
            AutomatedReplaceRack(3.0f);
        }

        print($"~UpdateBoardBot.CallPlayerBot betterRack {betterRack} \n");
    }

    protected override void NewGameBuildDictionaries() {
        print("~UpdateBoardBot.NewGameBuildDictionaries\n");
        if (saveLanguage.Equals(gameParameters.language) && saveBotLevel.Equals(gameParameters.botLevel)) {
            return;
        }

        base.NewGameBuildDictionaries();
        print("~UpdateBoardBot.NewGameBuildDictionaries after base\n");

        // if lang is same then botLevel is different. 
        // for now, we can assume that english is only language with multiple dictionaries.
        // so if language is not english , just use primary dictionary
        if (gameParameters.botLevel != saveBotLevel) {
            print("~UpdateBoardBot.NewGameBuildDictionaries newbotlevel\n");
            var botDictionaryTrie = dictionaryManager.BuildDictionaryForBot(gameParameters);
            brucesBot = BotFactory.Create(gameParameters.botLevel, botDictionaryTrie, scoreCalculator);
            saveBotLevel = gameParameters.botLevel;
        }
    }
}