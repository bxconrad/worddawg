using System;
using System.Collections.Generic;
using System.Linq;

public class Word {
    private readonly DateTime created;
    private readonly List<WordHistory> histories = new();


    public Word(string contents) {
        CreateWord(contents);
        created = DateTime.Now;
    }

    public WordHistory currentWord { get; set; }

    public static IEqualityComparer<Word> CreatedComparer { get; } = new CreatedEqualityComparer();

    public int GetNumModified() {
        return histories.Count();
    }

    public string GetCurrentContents() {
        return currentWord.contents;
    }

    public void CreateWord(string contents) {
        var history = new WordHistory(contents);
        histories.Add(history);
        currentWord = history;
        Console.WriteLine(history.ToString());
    }

    public bool IsNewWord() {
        return histories.Count == 1;
    }

    public override string ToString() {
        return $" {nameof(currentWord)}: {currentWord}, {nameof(created)}: {created}, NumHistories: {histories.Count},";
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
        public Player player;
        public int score;

        /*
         * A word history includes the content/string, time created, score, player.
         */
        public WordHistory(string contents) : this(contents, -1) {
        }

        private WordHistory(string contents, int score) {
            this.contents = contents;
            created = DateTime.Now;
            player = Player.GET_DUMMY_PLAYER();
        }

        public override string ToString() {
            return $"Contents: {contents}, Created: {created}, Player: {player}, Score: {score}";
        }
        // public Player player
    }
}