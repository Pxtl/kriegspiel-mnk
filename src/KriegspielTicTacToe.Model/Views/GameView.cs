using OneOf;
using OneOf.Types;

namespace KriegspielTicTacToe.Model.Views;

public record GameView
: GameObjectView<IGameState> {
    #region Constructors
    public GameView(IGameState gameState, Player? player)
    : base(gameState, player) { }
    #endregion

    #region Calculated Members
    public bool CanTakeTurn => Value.PlayManager.CanTakeTurn(Player);
    public IEnumerable<BoardView> Boards { get {
        for(sbyte i = 0; i < BoardsCount; i+=1) {
            yield return new BoardView(Value.Boards[i], Player, i);
        }
    } }
    

    [JsonIgnore()]
    public bool IsGameOver => Value.IsGameOver;

    [JsonIgnore()]
    public IEnumerable<Player> Winners => Value.Winners;
    
    #endregion

    #region Player Actions
	public void ResignPlayer() {
        if (Player == null) {
            throw new InvalidOperationException($"{nameof(Player)} is null.");
        } else {
		    Value.PlayManager.ResignPlayer(Player);
        }
	}

	public void EndTurn(out bool hasStateChanged) {
        if (Player == null) {
            throw new InvalidOperationException($"{nameof(Player)} is null.");
        } else {
		    Value.PlayManager.EndTurn(Player, out hasStateChanged);
        }
	}

    public OneOf<NotFound, BoardIsDone, Result<sbyte>> SelectBoard(string boardName)
        => GetBoardIndexByName(boardName).Match(
            notFound => new NotFound(),
            indexResult => Value.Boards[indexResult.Value].IsDone
                ? OneOf<NotFound, BoardIsDone, Result<sbyte>>.FromT1(new BoardIsDone())
                : new Result<sbyte>(indexResult.Value)
        );
    #endregion


    #region private helpers
    private OneOf<NotFound, Result<sbyte>> GetBoardIndexByName(string boardName) {
        var boardNameAsSbyte = sbyte.Parse(boardName);
        sbyte boardIndex = boardNameAsSbyte.Minus1();
        return (boardIndex >= 0 && boardIndex < Value.Boards.Count) 
            ? new Result<sbyte>(boardIndex)
            : new NotFound();
    }

    public IEnumerable<string> BoardNames { get {
        for(var i = 1; i <= Value.Boards.Count; i += 1) {
            yield return i.ToString();
        }
    }}
    #endregion

    #region board management
    [JsonIgnore()]
    public sbyte BoardsCount => (sbyte)Value.Boards.Count;

    public BoardView GetBoardViewByIndex(sbyte boardIndex)
    => new BoardView(Value.Boards[boardIndex], Player, boardIndex);
    #endregion
}