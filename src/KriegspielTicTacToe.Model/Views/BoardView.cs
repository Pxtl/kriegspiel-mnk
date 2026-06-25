namespace KriegspielTicTacToe.Model.Views;

public record BoardView
: GameObjectView<Board> {
    public BoardView(Board board, Player? player, sbyte boardIndex)
    : base(board, player) {
        BoardIndex = boardIndex;
    }
    #region data properties
    public sbyte BoardIndex { get; init; }
    #endregion

    #region calculated properties
	public bool IsDone => Value.IsDone;
	public int SpaceNameLength => Value.SpaceNameLength;
    public sbyte RowCount => Value.RowCount;
    public sbyte ColumnCount => Value.ColumnCount;
    #endregion

    public string GetSpaceName(sbyte col, sbyte row)
    => Value.GetSpaceName(col, row);

	public SpaceView GetSpaceView(sbyte col, sbyte row)
    => new SpaceView(Value.Spaces[col, row], Player, col, row);
}
