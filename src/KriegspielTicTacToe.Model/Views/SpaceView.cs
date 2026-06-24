namespace KriegspielTicTacToe.Model.Views;

public record SpaceView
: GameObjectView<Space> {
    public SpaceView(Space space, Player? player, sbyte col, sbyte row)
    : base(space, player) {
        Col = col;
        Row = row;
    }
    #region data properties
    public sbyte Col { get; init; }
    public sbyte Row { get; init; }
    #endregion

	public string? Mark 
    => Value.IsKnownToPlayer(Player)
        ? Value.Mark
        : null;
}
