namespace KriegspielTicTacToe.Model;

using System.IO;
using OneOf;


/// <summary>
/// JSON-serializable model object for a single tic-tac-toe board.
/// </summary>
public abstract record Board {
    #region constructors
    public Board() {}
    
    #endregion

    #region main data properties
    public Space[,] Spaces {get;set;} = new Space[1,1]{{new Space()}}; //default value is dummy board, never use.
    #endregion

    #region Methods
    /// <summary>
    /// For the given space on the board, generate the space's name.
    /// </summary>
    public int GetSpaceNameAsInt(int col, int row) {
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
    public bool TryGetCoordinatesFromSpaceNameAsInt(int spaceName, out int resultCol, out int resultRow) {
        //brute-force search
        //todo: smarter algo
        for (var col = 0; col < Spaces.GetLength(0); col+=1) {
            for (var row = 0; row < Spaces.GetLength(1); row+=1) {
                if(GetSpaceNameAsInt(col, row) == spaceName) {
                    resultCol = col;
                    resultRow = row;
                    return true;
                }
            }
        }
        resultCol = resultRow = -1;
        return false;
    }
        
    /// <summary>
    /// Get all of the spaces on the board as a big enumerable that you can
    /// foreach across.
    /// </summary>
    public IEnumerable<SpaceEnumerator> BoardAsEnumerable() {
        for(sbyte col = 0; col < Spaces.GetLength(0); col += 1) {
            for(sbyte row = 0; row < Spaces.GetLength(1); row += 1) {
                yield return new SpaceEnumerator(col, row, Spaces[col, row]);
            }
        }
    }

    public IEnumerable<string> SpaceNames
        => BoardAsEnumerable()
            .Select(s => GetSpaceNameAsInt(s.Col, s.Row).ToString());
    #endregion

    #region abstract and virtual properties
    public abstract ScoreCard ScoreCard { get; }
    #endregion

    #region helper properties
    [JsonIgnore()]
    public int ColumnCount
        => Spaces.GetLength(0);

    [JsonIgnore()]    
    public int RowCount
        => Spaces.GetLength(1);
        
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
        => BoardAsEnumerable().All(s => s.Space.Mark != null);

    [JsonIgnore()]
    public virtual bool IsDone
        => IsFull;

    public static bool IsSpaceInsideOfBoard((sbyte Col, sbyte Row) pos, (sbyte Col, sbyte Row) boardSize)
        => (pos.Col < boardSize.Col)
            && (pos.Row < boardSize.Row)
            && (pos.Col >= 0)
            && (pos.Row >= 0);
    #endregion
}
