using System;
using System.Collections.Generic;
using System.Linq;

public class Word {
    private readonly DateTime created;
    private readonly List<WordHistory> histories = new();
    public string contents;


    public Word(string contents, Player player, int score) {
        CreateWord(contents, player, score);
        created = DateTime.Now;
    }

    public WordHistory currentWordHistory { get; set; }

    public static IEqualityComparer<Word> CreatedComparer { get; } = new CreatedEqualityComparer();

    public int GetNumModified() {
        return histories.Count();
    }

    public string GetCurrentContents() {
        return currentWordHistory.contents;
    }

    public string GetPreviousContents() {
        return GetPreviousHistory() != null ? GetPreviousHistory().contents : "";
    }

    public string GetPreviousDisplayContents() {
        return GetPreviousContents().Replace("#", "QU").Replace("*", "LL");
    }


    public WordHistory GetPreviousHistory() {
        if (histories.Count >= 2) {
            return histories[^2];
        }

        return null;
    }

    public string GetDisplayContents() {
        return GetCurrentContents().Replace("#", "QU").Replace("*", "LL");
    }

    public bool IsStolenWord() {
        var history = GetPreviousHistory();
        return history == null ? false : history.player == currentWordHistory.player;
    }

    public void CreateWord(string contents, Player player, int score) {
        this.contents = contents;
        var history = new WordHistory(contents, player, score);
        histories.Add(history);
        currentWordHistory = history;
        player.currentScore += score; // bcdo don't do this here. have updateScore on Player
        Console.WriteLine(history.ToString());
    }

    public bool IsNewWord() {
        return histories.Count == 1;
    }

    public override string ToString() {
        return
            $" {nameof(currentWordHistory)}: {currentWordHistory}, {nameof(created)}: {created}, NumHistories: {histories.Count},";
    }

    private sealed class CreatedEqualityComparer : IEqualityComparer<Word> {
        public bool Equals(Word x, Word y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.created.Equals(y.created);
        }

        public int GetHashCode(Word obj) {
            return obj.created.GetHashCode();
        }
    }

    public class WordHistory {
        public string contents;
        public DateTime created;
        public bool isDogBonusWord;
        public Player player;
        public int score;

        /*
         * A word history includes the content/string, time created, score, player.
         */
        public WordHistory(string contents, Player player, int score) {
            this.contents = contents;
            created = DateTime.Now;
            this.player = player;
            this.score = score;
            isDogBonusWord = false;
        }

        public override string ToString() {
            return
                $"{nameof(contents)}: {contents}, {nameof(created)}: {created}, {nameof(player)}: {player}, {nameof(score)}: {score}";
        }
    }
}