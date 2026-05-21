namespace KriegspielTicTacToe.Model;

using OneOf;
using OneOf.Types;

/// <summary>
/// Object that models the full state of the game.  Is serialized into a json file so all players can share reading it.
/// </summary>
public record TicTacToeState {
    #region constructors
    /// <summary>
    /// This constructor is only used by the serializer, never use it.
    /// </summary>
    public TicTacToeState() { 
        Boards = [];
        PlayManager = new PlayManager();
    }

    /// <summary>
    /// Construct a new gamestate object.  Note that there is no protection at this level against impossible values.
    /// </summary>
    public TicTacToeState(
        char[] players,
        IEnumerable<BoardBuilder> boardBuilders,
        bool isRandomPlayerOrder,
        bool isSynchronousMode
    ) {
        if(isRandomPlayerOrder) { 
            Random.Shared.Shuffle(players); 
        }
        PlayManager = new PlayManager
        {
            Players = players.ToList(),
            IsSynchronousMode = isSynchronousMode
        };
        Boards = boardBuilders.Select(b => new Board(b)).ToList();
    }
    #endregion

    #region main data properties
    public PlayManager PlayManager {get;init;}
    public IReadOnlyList<Board> Boards {get;init;}

    #endregion

    #region methods   
    public Board GetBoardByCode(int boardCode)
        => Boards[boardCode-1];

    public OneOf<NotFound, BoardIsDone, Result<int>> SelectBoard(int boardCode)
        => (boardCode <= 0 || boardCode > Boards.Count)
            ? new NotFound()
            : GetBoardByCode(boardCode).IsDone
            ? new BoardIsDone()
            : new Result<int>(boardCode - 1);

    /// <summary>
    /// Play a space by its space code.
    /// </summary>
    public OneOf<Success, Result<char>, AlreadyPlayed, NotFound> PlaySpace(int boardIndex, int spaceCode) {
        var board = Boards[boardIndex];
        if (board.TryGetCoordinatesFromSpaceIndexCode(spaceCode, out var col, out var row)) {
            return PlaySpace(boardIndex, col, row)
                .Match<OneOf<Success, Result<char>, AlreadyPlayed, NotFound>>( //have to provide return-type when going from OneOf to OneOf
                    success => success,
                    result => result,
                    alreadyPlayed => alreadyPlayed
                );
        } else {
            return new NotFound();
        }
    }
    
    /// <summary>
    /// Play a space by its coordinates.
    /// </summary>
    public OneOf<Success, Result<char>, AlreadyPlayed> PlaySpace(int boardIndex, int col, int row) {
        var board = Boards[boardIndex];
        var space = board.Spaces[col, row];
        if (space.MarkChar == PlayManager.CurrentTurnPlayer) {
            return new AlreadyPlayed();
        } else {
            space.MakeKnownToPlayer(PlayManager.CurrentTurnPlayer);
            var foundMark = space.MarkChar;
            if (foundMark.HasValue) {
                return new Result<char>(foundMark.Value);
            } else {
                space.MarkChar = PlayManager.CurrentTurnPlayer;
                return new Success();
            }
        }
    }
    #endregion

    #region helper properties
    /// <summary>
    /// Returns a list of the active board indices. 0-based.
    /// </summary>
    [JsonIgnore()]
    public IEnumerable<int> ActiveBoardIndices {get {
        for(int i = 0; i < Boards.Count; i+=1) {
            if(!Boards[i].IsDone) {
                yield return i; 
            }
        }
    }}
    
    /// <summary>
    /// Returns null if there are 0 or multiple active boards. Board Index if there's 1.
    /// </summary>
    [JsonIgnore()]
    public int? SingleActiveBoardIndex {get {
        var firstElements = ActiveBoardIndices.Take(2).ToArray();
        return (firstElements.Length == 1)
            ? firstElements.Single()
            : null;
    }}

    /// <summary>
    /// Returns true if the game has ended, whether by tie or by winner.
    /// </summary>
    [JsonIgnore()]
    public bool IsGameOver
        => Boards.All(b => b.IsDone) || PlayManager.ActivePlayers.Count() == 1;
    
    /// <summary>
    /// Provides a short text summary of the current game-state. Particularly useful when the game is over.
    /// </summary>
    [JsonIgnore()]
    public string GameStateText
        => IsGameOver 
        ? (Winner.HasValue 
            ? $"Player '{Winner.Value}' wins!." 
            : "Tie game."
        ) 
        : PlayManager.GameStateText;
    
    [JsonIgnore()]
    /// <summary>
    /// Get the winner of the game.  Returns null if nobody has won yet or the
    /// game was a tie.  Note this is a heavy calculation and is not cached, but
    /// computers are fast.  TODO: Optimization.
    /// </summary>
    public char? Winner {
        get {
            if(!IsGameOver) {
                return null;
            }
            
            if(PlayManager.ActivePlayers.Count() == 1) {
                return PlayManager.ActivePlayers.Single();
            }
            var highestScore = ScoreCard.HighestScore;

            return highestScore.HasValue 
                ? highestScore.Value.Player 
                : null; // no winner found
        }
    }
    
    [JsonIgnore()]
    public ScoreCard ScoreCard 
        => Boards.Aggregate(
            new ScoreCard(),
            (prod, next) => prod + next.ScoreCard
            );
    #endregion
}

/// <summary>
/// Empty result struct for OneOf, used when a player tries to play on a space they already played.
/// </summary>
public struct AlreadyPlayed;

/// <summary>
/// Empty result struct for OneOf, used when the player tries to select a board that is done.
/// </summary>
public struct BoardIsDone;
