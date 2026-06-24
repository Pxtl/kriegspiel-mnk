using OneOf;
using OneOf.Types;

namespace KriegspielTicTacToe.Model.TicTacToe;

public record TicTacToeState : GameState<TicTacToeState, TicTacToeTemplate, TicTacToeScoring, TicTacToePlayAction> {
    [Obsolete]
    public TicTacToeState() : base() { }

    public TicTacToeState(
        Player[] players,
        TicTacToeTemplate gameTemplate,
        bool isRandomPlayerOrder
    ) : base(players, gameTemplate, isRandomPlayerOrder) { }

    public OneOf<NotFound, BoardIsDone, Result<int>> SelectBoard(string boardName)
        => GetBoardIndexByName(boardName).Match(
            notFound => new NotFound(),
            indexResult => GetBoardByIndex(indexResult.Value).IsDone
                ? OneOf<NotFound, BoardIsDone, Result<int>>.FromT1(new BoardIsDone())
                : new Result<int>(indexResult.Value)
        );

    public OneOf<NotFound, ActionQueuedSuccessfully, Result<Player>, AlreadyPlayed> PlaySpace(
        int boardIndex,
        string spaceName,
        Player player
    ) => PlaySpace(boardIndex, int.Parse(spaceName), player);
        
    private OneOf<NotFound, ActionQueuedSuccessfully, Result<Player>, AlreadyPlayed> PlaySpace(
        int boardIndex,
        int spaceNameAsInt,
        Player player
    ) {
        if (spaceNameAsInt <= 0) {
            return new NotFound();
        }
        var board = Boards[boardIndex];
        if (board.TryGetCoordinatesFromSpaceNameAsInt(spaceNameAsInt, out var col, out var row)) {
            return PlaySpace(boardIndex, col, row, player);
        }
        return new NotFound();
    }

    public OneOf<NotFound, ActionQueuedSuccessfully, Result<Player>, AlreadyPlayed> PlaySpace(
        int boardIndex,
        int col,
        int row,
        Player player
    ) {
        if (!PlayManager.CanTakeTurn(player)) {
            throw new InvalidOperationException($"Player '{player}' cannot take their turn.");
        }
        
        var space = Boards[boardIndex].Spaces[col, row];
        if (space.IsKnownToPlayer(player)) {
            return new AlreadyPlayed();
        } else if (space.Mark != null) {
            space.MakeKnownToPlayer(player);
            return new Result<Player>(space.Mark);
        } else {
            PlayActionBuffer.Add(new TicTacToePlayAction(boardIndex, col, row, player));
            return new ActionQueuedSuccessfully();
        }
    }
    
    [JsonIgnore()]
    public override ScoreCard ScoreCard 
        //make sure all players are in the scorecard even those with 0.
        => new ScoreCard(PlayManager.ActivePlayers.Select(p => new PlayerScore(p, 0))) 
            // can't use ienumerable.sum on non-numeric objects, operators don't work that way.
            + Boards.Select(b => b.ScoreCard).SumScoreCards();
}
public struct AlreadyPlayed;
public struct BoardIsDone;
public struct ActionQueuedSuccessfully;
