namespace KriegspielTicTacToe.Model;

public record TicTacToePlayAction(
    int BoardIndex,
    int Col,
    int Row,
    Player Player
) {
	internal void DoActionCollision(TicTacToeState gameState) {
        if (GetBoard(gameState).IsDone) {
            return;
        }
        var space = GetSpace(gameState);
		space.Mark = "█";
        foreach(var player in gameState.PlayManager.Players) {
            space.MakeKnownToPlayer(player);    
        }
	}

    internal Board GetBoard(TicTacToeState gameState)
        => gameState.GetBoardByIndex(BoardIndex);

    internal Space GetSpace(TicTacToeState gameState)
        => GetBoard(gameState).Spaces[Col, Row];

	public bool IsActionCollision(TicTacToePlayAction otherAction)
    => BoardIndex == otherAction.BoardIndex
        && Col == otherAction.Col
        && Row == otherAction.Row
        && Player != otherAction.Player;

	public void DoAction(TicTacToeState gameState)
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
