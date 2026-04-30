public class ResultMatch {
    public string GeneratedWord { get; set; }
    public Word SourceObject { get; set; }

    public override string ToString() {
        return $"GeneratedWord: {GeneratedWord}, SourceObject: {SourceObject}";
    }
}