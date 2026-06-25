using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using KriegspielTicTacToe.Model.Views;
using OneOf;

namespace KriegspielTicTacToe.Model;

/// <summary>
/// JSON-serializable model object for a single tic-tac-toe board. Columns are
/// left-to-right, rows are top-to-bottom.
/// </summary>
public record Board
: IBoard {
    #region constructors
    /// <summary>
    /// Default constructor creates a useless board.  Never uses this without
    /// replacing <see cref="Ruleset"> and <see cref="Spaces"/> members.
    /// </summary>
    public Board()
    : this(1, 1, BoardRuleset.Empty) { }

    public Board(Template.BoardBuilder builder)
    : this(builder.Width, builder.Height, builder.Ruleset) { }

    public Board(sbyte width, sbyte height)
    : this(width, height, null) { }

    public Board(sbyte width, sbyte height, BoardRuleset? ruleset) {
        Ruleset = ruleset ?? BoardRuleset.Empty;
        Spaces = new Space[width, height];
        for (sbyte col = 0; col < ColumnCount; col += 1) {
            for (sbyte row = 0; row < RowCount; row += 1) {
                Spaces[col, row] = new Space();
            }
        }
    }

    public Board(BoardRuleset ruleset)
    : this() {
        Ruleset = ruleset;
    }
    #endregion

    #region main data properties
    [Required]
    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.None, TypeNameHandling = TypeNameHandling.None)] //non-polymorphic
    public Space[,] Spaces { get; init; }
    [Required]
    public BoardRuleset Ruleset { get; init; }
    #endregion

    #region Methods
    /// <summary>
    /// For the given space on the board, generate the space's name.
    /// </summary>
    private int GetSpaceNameAsInt(sbyte col, sbyte row) {
        //aims for basic 3x3, but larger if needed
        //7 8 9
        //4 5 6
        //1 2 3
        return Spaces.GetLength(1) * (Spaces.GetLength(0) - 1) //top-left
            + col
            - row * Spaces.GetLength(0)
            + 1; //1-based
    }

    public string GetSpaceName(sbyte col, sbyte row)
    => GetSpaceNameAsInt(col, row)
        .ToString(new string('0', SpaceNameLength)); //zero-pad;

    /// <summary>
    /// For the given space index code, find the coordinates.  Uses a "Try"
    /// signature so that it shall return false if the given spaceindex is not
    /// on the board at all.
    /// </summary>
    public bool TryGetCoordinatesFromSpaceName(string spaceName, out sbyte resultCol, out sbyte resultRow) {
        //brute-force search
        //todo: smarter algo
        for (sbyte col = 0; col < ColumnCount; col += 1) {
            for (sbyte row = 0; row < RowCount; row += 1) {
                if (GetSpaceName(col, row).Equals(spaceName, StringComparison.OrdinalIgnoreCase)) {
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
    /// Get all of the spaces on the board as a big enumerable that you can
    /// foreach across.
    /// </summary>
    public IEnumerable<SpaceView> BoardAsSpaceViewEnumerable(Player? player) {
        for (sbyte col = 0; col < Spaces.GetLength(0); col += 1) {
            for (sbyte row = 0; row < Spaces.GetLength(1); row += 1) {
                yield return new SpaceView(Spaces[col, row], player, col, row);
            }
        }
    }

    public IEnumerable<Space> BoardAsSpaceEnumerable() {
        for (sbyte col = 0; col < Spaces.GetLength(0); col += 1) {
            for (sbyte row = 0; row < Spaces.GetLength(1); row += 1) {
                yield return Spaces[col, row];
            }
        }
    }

    public IEnumerable<SpaceView> BoardAsSpaceViewEnumerable()
    => BoardAsSpaceViewEnumerable(player: null);

    [JsonIgnore()]
    public IEnumerable<string> SpaceNames
    => BoardAsSpaceViewEnumerable()
        .Select(s => GetSpaceName(s.Col, s.Row)); //zero-pad.
    #endregion

    #region abstract and virtual properties
    [JsonIgnore()]
    public ScoreCard ScoreCard => Ruleset.Score(this);
    #endregion

    #region helper properties
    [JsonIgnore()]
    public sbyte ColumnCount
    => (sbyte)Spaces.GetLength(0);

    [JsonIgnore()]
    public sbyte RowCount
    => (sbyte)Spaces.GetLength(1);

    /// <summary>
    /// Get how many spaces are on the board.
    /// </summary>
    [JsonIgnore()]
    public int SpaceCount
    => Spaces.GetLength(0) * Spaces.GetLength(1);

    /// <summary>
    /// Get how many digits the users will have to type in to type in a
    /// space-name.
    /// </summary>
    [JsonIgnore()]
    public int SpaceNameLength
    => SpaceCount.ToString().Length;

    /// <summary>
    /// Returns true if the board is full.
    /// </summary>
    [JsonIgnore()]
    public bool IsFull
    => BoardAsSpaceEnumerable().All(s => s.Mark != null);

    /// <summary>
    /// Returns true if the board is done and locked from further play.
    /// </summary>
    [JsonIgnore()]
    public virtual bool IsDone
    => IsFull || Ruleset.IsDone(this);
    #endregion

    /// <summary>
    /// Returns true if a space is within an arbitrarily-sized board.
    /// </summary>
    public bool IsSpaceInsideOfBoard((sbyte Col, sbyte Row) pos, (sbyte Col, sbyte Row) boardSize)
    => (pos.Col < boardSize.Col)
        && (pos.Row < boardSize.Row)
        && (pos.Col >= 0)
        && (pos.Row >= 0);
}
