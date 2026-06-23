using System.Text.Json.Serialization;

namespace KriegspielTicTacToe.Model.TicTacToe;

public record TicTacToePlayAction(
    int BoardIndex,
    int Col,
    int Row,
    Player Player
) : PlayAction<TicTacToePlayAction, TicTacToeState> {
	public override void DoActionCollision(TicTacToeState gameState) {
        if (GetBoard(gameState).IsDone) {
            return;
        }
        var space = GetSpace(gameState);
		space.Mark = "█";
        foreach(var player in gameState.PlayManager.Players) {
            space.MakeKnownToPlayer(player);    
        }
	}

    protected Board GetBoard(TicTacToeState gameState)
        => gameState.GetBoardByIndex(BoardIndex);

    protected Space GetSpace(TicTacToeState gameState)
        => GetBoard(gameState).Spaces[Col, Row];

	public override bool IsActionCollision(TicTacToePlayAction otherAction)
    => BoardIndex == otherAction.BoardIndex
        && Col == otherAction.Col
        && Row == otherAction.Row
        && Player != otherAction.Player;

	public override void DoAction(TicTacToeState gameState)
	{
		if (GetBoard(gameState).IsDone) {
            return;
        }
        var space = GetSpace(gameState);

        if (space.Mark == null) {
            space.Mark = Player.Mark;
        }
            
        space.MakeKnownToPlayer(Player);
	}
}
