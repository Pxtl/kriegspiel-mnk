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
        => ModelToCommandNameUtility.GetBoardIndexByName(boardName, Value.Boards.Count).Match(
            notFound => new NotFound(),
            indexResult => Value.Boards[indexResult.Value].IsDone
                ? OneOf<NotFound, BoardIsDone, Result<sbyte>>.FromT1(new BoardIsDone())
                : new Result<sbyte>(indexResult.Value)
        );
    #endregion


    #region private helpers
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

    public BoardView GetBoardViewByName(string boardName) {
        if (BoardsCount == 1) {
            return Boards.Single();
        }
        var boardIndex = ModelToCommandNameUtility.GetBoardIndexByName(boardName, BoardsCount).Match(
            notFound => throw new ArgumentException($"That is not a valid board: '{boardName}", nameof(boardName)),
            result => result.Value
        );
        return GetBoardViewByIndex(boardIndex);
    }
    #endregion

    #region space names

    [JsonIgnore()]
    public IEnumerable<string> SpaceNames
    => Boards.SelectMany(b => b
        .AsSpaceViewEnumerable()
        .Select(s => GetSpaceName(b.BoardName, s.Col, s.Row))
    ); //zero-pad.
        
    /// <summary>
    /// For the given space on the board, generate the space's name.
    /// </summary>
    private int GetSpaceNameAsInt(BoardView board, sbyte col, sbyte row) {
        //aims for basic 3x3, but larger if needed
        //7 8 9
        //4 5 6
        //1 2 3
        return board.RowCount * (board.ColumnCount - 1) //top-left
            + col
            - row * board.ColumnCount
            + 1; //1-based
    }

    private bool IsSpaceNamingNumpadLayout(BoardView board) => board.SpaceCount < 10;

    public string GetSpaceName(BoardView board, sbyte col, sbyte row)
    => (BoardsCount > 1 ? board.BoardName : "") //board name component
        + 
        (IsSpaceNamingNumpadLayout(board) //space name component
            ? GetSpaceNameAsInt(board, col, row).ToString()
            : (
                // letter component.  Can be only length 1 because max board size is 26.
                ((char)('A' + col)).ToString()
                // number component, zero-padded to SpaceNameLength without the letter component.
                + (board.RowCount - row).ToString(new string('0', SpaceNameLength(board) - 1)) 
            )
        );

    public string GetSpaceName(string boardName, sbyte col, sbyte row) {
        var board = GetBoardViewByName(boardName);
        return GetSpaceName(board, col, row);
    }

    /// <summary>
    /// For the given space index code, find the coordinates.  Uses a "Try"
    /// signature so that it shall return false if the given spaceindex is not
    /// on the board at all.
    /// </summary>
    public bool TryGetCoordinatesFromSpaceName(string spaceName, out string boardName, out sbyte resultCol, out sbyte resultRow) {
        boardName = "1";
        if(BoardsCount > 1) {
            boardName = spaceName.Substring(0, 1);
        }
        var board = GetBoardViewByName(boardName);

        //brute-force search
        //todo: smarter algo
        for (sbyte col = 0; col < board.ColumnCount; col += 1) {
            for (sbyte row = 0; row < board.RowCount; row += 1) {
                if (GetSpaceName(boardName, col, row).Equals(spaceName, StringComparison.OrdinalIgnoreCase)) {
                    resultCol = col;
                    resultRow = row;
                    return true;
                }
            }
        }
        // if not found
        resultCol = resultRow = -1;
        return false;
    }
    /// <summary>
    /// Get how many chars the users will have to type in to type in a
    /// space-name.
    /// </summary>
    public int SpaceNameLength(BoardView board)
    => IsSpaceNamingNumpadLayout(board)
        ? 1
        : (int)Math.Log10(board.RowCount) + 2; //(int)Math.Log10(RowCount) is number of digits - 1.  Add 2, 1 for digits, 1 for letter.

    #endregion
}