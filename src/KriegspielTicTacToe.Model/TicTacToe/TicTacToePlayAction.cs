using System.ComponentModel.DataAnnotations;
using OneOf;
using OneOf.Types;

namespace KriegspielTicTacToe.Model.TicTacToe;

public record TicTacToePlayAction
: PlayAction {
    [Obsolete("Default constructor is only used for deserialization.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public TicTacToePlayAction() : base() { }
#pragma warning restore CS8618
	public TicTacToePlayAction(
        sbyte boardIndex,
        sbyte col,
        sbyte row,
        Player player
    ) : base(player) {
        BoardIndex = boardIndex;
        Col = col;
        Row = row;
    }
    [Required]
    public sbyte BoardIndex {get;init;}
    [Required]
    public sbyte Col {get;init;}
    [Required]
    public sbyte Row {get;init;}

	public override void DoActionCollision(IGameState gameState) {
        if (GetBoard(gameState).IsDone) {
            return;
        }
        var space = GetSpace(gameState);
		space.Mark = "█";
        foreach(var player in gameState.PlayManager.Players) {
            space.MakeKnownToPlayer(player);    
        }
	}

    protected Board GetBoard(IGameState gameState)
        => gameState.Boards[BoardIndex];

    protected Space GetSpace(IGameState gameState)
        => GetBoard(gameState).Spaces[Col, Row];

	public override bool IsActionCollision(PlayAction otherAction)
    => otherAction is TicTacToePlayAction otherTicTacToeAction 
        ? BoardIndex == otherTicTacToeAction.BoardIndex
            && Col == otherTicTacToeAction.Col
            && Row == otherTicTacToeAction.Row
            && Player != otherTicTacToeAction.Player
        : throw new InvalidOperationException("Cannot compare different action types.");

	public override OneOf<IsLegalToQueue, NewlyLearned, AlreadyPlayed> Attempt(IGameState gameState) {
        var space = gameState.Boards[BoardIndex].Spaces[Col, Row];
        if (space.IsKnownToPlayer(Player)) {
            return new AlreadyPlayed();
        } else if (space.Mark != null) {
            space.MakeKnownToPlayer(Player);
            return new NewlyLearned(space.Mark);
        } else {
            return new IsLegalToQueue();
        }
	}

	public override void DoAction(IGameState gameState)
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

    public static TicTacToePlayAction Create(
        IGameState gameState,
        sbyte boardIndex,
        string spaceName,
        Player player
    ) => Create(gameState, boardIndex, int.Parse(spaceName), player);
        
    public static TicTacToePlayAction Create(
        IGameState gameState,
        sbyte boardIndex,
        int spaceNameAsInt,
        Player player
    ) {
        if (spaceNameAsInt <= 0) {
            throw new KeyNotFoundException("That is not a valid space name for this board.");
        }
        var board = gameState.Boards[boardIndex];
        if (board.TryGetCoordinatesFromSpaceNameAsInt(spaceNameAsInt, out var col, out var row)) {
            return new TicTacToePlayAction(boardIndex, col, row, player);
        } else {
            throw new KeyNotFoundException("That is not a valid space name for this board.");
        }
    }
}
