using System.ComponentModel.DataAnnotations;
using OneOf;
using OneOf.Types;

namespace KriegspielTicTacToe.Model.MNKGame;

/// <summary>
/// A play action for an MNK game such as tic tac toe.  <see href="https://en.wikipedia.org/wiki/M,n,k-game">WP: MNK Game</see>
/// </summary>
public record MNKPlayAction
: PlayAction {
    [Obsolete("Default constructor is only used for deserialization.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public MNKPlayAction() : base() { }
#pragma warning restore CS8618
	public MNKPlayAction(
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
    => otherAction is MNKPlayAction otherTicTacToeAction 
        ? BoardIndex == otherTicTacToeAction.BoardIndex
            && Col == otherTicTacToeAction.Col
            && Row == otherTicTacToeAction.Row
            && Player != otherTicTacToeAction.Player
        : throw new InvalidOperationException("Cannot compare different action types.");

	public override OneOf<IsLegalToQueue, NewlyLearned, AlreadyPlayed> Attempt(IGameState gameState) {
        var space = gameState.Boards[BoardIndex].Spaces[Col, Row];
        if (space.Mark == null) {
            space.MakeKnownToPlayer(Player);
            return new IsLegalToQueue();
        } else if (space.IsKnownToPlayer(Player)) {
            return new AlreadyPlayed();
        } else {
            space.MakeKnownToPlayer(Player);
            return new NewlyLearned(space.Mark);
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

    public static MNKPlayAction Create(
        IGameState gameState,
        sbyte boardIndex,
        string spaceName,
        Player player
    ) {
        var board = gameState.Boards[boardIndex];
        if (board.TryGetCoordinatesFromSpaceName(spaceName, out var col, out var row)) {
            return new MNKPlayAction(boardIndex, col, row, player);
        } else {
            throw new KeyNotFoundException("That is not a valid space name for this board.");
        }
    }
}
