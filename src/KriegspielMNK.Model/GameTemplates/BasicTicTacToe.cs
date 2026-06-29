using KriegspielMNK.Model.Template;
using KriegspielMNK.Model.MNKGame;

namespace KriegspielMNK.Model;

public static partial class GameTemplates {
    public static GameTemplate BasicTicTacToe {get;} = new MNKTemplate(
        "basic-tictactoe",
        "Basic simple tic-tac-toe.",
        [2], //playercount.
        [
            new BoardBuilder(3, 3, new MNKBoardRuleset(IsBoardDoneWhenScored: true))
        ],
        
        isKriegspiel: false,
        isSynchronousMode: false
    );
}