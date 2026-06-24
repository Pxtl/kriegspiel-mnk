using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using KriegspielTicTacToe.Model.Views;
using OneOf;

namespace KriegspielTicTacToe.Model;

/// <summary>
/// JSON-serializable model object for a single tic-tac-toe board.
/// </summary>
public record Board
: IBoard {
    #region constructors
    /// <summary>
    /// Default constructor creates a useless board.  Never uses this without
    /// replacing <see cref="Ruleset"> and <see cref="Spaces"/> members.
    /// </summary>
    public Board()
    : this(1, 1, GameRuleset.Empty) { }

    public Board(BoardBuilder builder)
    : this(builder.Width, builder.Height, builder.Ruleset) { }

    public Board(sbyte width, sbyte height)
    : this(width, height, null) { }

    public Board(sbyte width, sbyte height, GameRuleset? ruleset) {
        Ruleset = ruleset ?? GameRuleset.Empty;
        Spaces = new Space[width, height];
        for (sbyte col = 0; col < ColumnCount; col += 1) {
            for (sbyte row = 0; row < RowCount; row += 1) {
                Spaces[col, row] = new Space();
            }
        }
    }

    public Board(GameRuleset ruleset)
    : this() {
        Ruleset = ruleset;
    }
    #endregion

    #region main data properties
    [Required]
    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.None, TypeNameHandling = TypeNameHandling.None)] //non-polymorphic
    public Space[,] Spaces { get; init; }
    [Required]
    public GameRuleset Ruleset { get; init; }
    #endregion

    #region Methods
    /// <summary>
    /// For the given space on the board, generate the space's name.
    /// </summary>
    public int GetSpaceNameAsInt(sbyte col, sbyte row)
    {
        //aims for basic 3x3, but larger if needed
        //7 8 9
        //4 5 6
        //1 2 3
        return Spaces.GetLength(1) * (Spaces.GetLength(0) - 1) //top-left
            + col
            - row * Spaces.GetLength(0)
            + 1; //1-based
    }

    /// <summary>
    /// For the given space index code, find the coordinates.  Uses a "Try"
    /// signature so that it shall return false if the given spaceindex is not
    /// on the board at all.
    /// </summary>
    public bool TryGetCoordinatesFromSpaceNameAsInt(int spaceName, out sbyte resultCol, out sbyte resultRow)
    {
        //brute-force search
        //todo: smarter algo
        for (sbyte col = 0; col < ColumnCount; col += 1) {
            for (sbyte row = 0; row < RowCount; row += 1) {
                if (GetSpaceNameAsInt(col, row) == spaceName) {
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
    public IEnumerable<SpaceView> BoardAsEnumerable(Player? player) {
        for (sbyte col = 0; col < Spaces.GetLength(0); col += 1) {
            for (sbyte row = 0; row < Spaces.GetLength(1); row += 1) {
                yield return new SpaceView(Spaces[col, row], player, col, row);
            }
        }
    }

    public IEnumerable<SpaceView> BoardAsEnumerable() => BoardAsEnumerable(player: null);

    [JsonIgnore()]
    public IEnumerable<string> SpaceNames
    => BoardAsEnumerable()
        .Select(s => GetSpaceNameAsInt(s.Col, s.Row).ToString());
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
    /// Returns true if the game has ended in a tie.
    /// </summary>
    [JsonIgnore()]
    public bool IsFull
    => BoardAsEnumerable().All(s => s.Mark != null);

    [JsonIgnore()]
    public virtual bool IsDone
    => IsFull || Ruleset.IsDone(this);
    #endregion

    public bool IsSpaceInsideOfBoard((sbyte Col, sbyte Row) pos, (sbyte Col, sbyte Row) boardSize)
    => (pos.Col < boardSize.Col)
        && (pos.Row < boardSize.Row)
        && (pos.Col >= 0)
        && (pos.Row >= 0);
}
