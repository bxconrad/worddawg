using Random = System.Random;

public class UpdateBoardHuman : UpdateBoardTwo {
    private static readonly Random rnd = new();

    protected override void UpdateButtonsInteractable(bool isInteractable) {
        print("~UpdateBoardHuman.UpdateButtonsInteractable  do nothing\n");
    }

    protected override string GetOpponentName() {
        return "Humanoid";
    }

    // player order is random
    protected override void GetPlayerOrder() {
        var r = UnityEngine.Random.Range(0, 2);
        print($"~UpdateBoardHuman.GetPlayerOrder r {r}\n");
        if (r == 0) {
            var player1 = new Player(GetOpponentName(), wordGrid1, scoreGrid1);
            var player2 = new Player(gameParameters.userName, wordGrid2, scoreGrid2);
            players = new[] { player1, player2 };
        }
        else {
            var player1 = new Player(gameParameters.userName, wordGrid1, scoreGrid1);
            var player2 = new Player(GetOpponentName(), wordGrid2, scoreGrid2);
            players = new[] { player1, player2 };
        }
    }
}