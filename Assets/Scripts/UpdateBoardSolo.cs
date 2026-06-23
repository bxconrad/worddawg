using EasyUI.Toast;
using UnityEngine;

public class UpdateBoardSolo : UpdateBoardAbstract {
    protected override void NewGameInitializePlayers() {
        // For One Player Version
        ActivatePanels(false);
        var player1 = new Player(gameParameters.userName, wordGrid1, scoreGrid1);
        players = new[] { player1 };

        Toast.Show(
            "To start the game, create a word by clicking on the letters in the rack. After that, you can create or modify new words.",
            15f, Color.magenta, GameHelper.GetToastPosition());
    }

    protected override void NextTurn() {
        // do nothing for solo game
    }
}