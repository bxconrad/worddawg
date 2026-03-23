public class BotFactory {
    /* BotLevels
           0 - Add 1 letter. Return lowest score word
           1 - Add 1 letter. Return random score word
           2 - Add 2 letters. Return random score word
           3 - Add 3 letters. Return high score word
           4 - Add 99 letters. Return high score word.
           */
    public static BrucesBotAbstract Create(int botLevel, TrieDictionary dictionary, ScoreCalculator scoreCalculator) {
        BrucesBotAbstract bot = null;
        if (botLevel == 0) {
            bot = new BrucesBot0();
        }
        else if (botLevel == 1) {
            bot = new BrucesBot1();
        }
        else if (botLevel == 2) {
            bot = new BrucesBot2();
        }
        else if (botLevel == 3) {
            bot = new BrucesBot3();
        }
        else if (botLevel == 4) {
            bot = new BrucesBot4();
        }

        bot.trieDictionary = dictionary;
        bot.scoreCalculator = scoreCalculator;
        return bot;
    }
}