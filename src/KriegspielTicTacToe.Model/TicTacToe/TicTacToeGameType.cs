namespace KriegspielTicTacToe.Model.TicTacToe;

/// <summary>
/// Represents a game type configuration including board builders and play mode settings.
/// </summary>
public record TicTacToeGameType
: GameType<TicTacToeBoard> {
    public TicTacToeGameType(
        IEnumerable<TicTacToeBoardBuilder> boardBuilders,
        bool isSynchronousMode
    )
    : base() {
        BoardBuilders = boardBuilders;
        PlayManagerFactory = isSynchronousMode 
            ? SynchronizedPlayManagerFactory.Instance
            : RoundRobinPlayManagerFactory.Instance;
    }

    public IEnumerable<TicTacToeBoardBuilder> BoardBuilders {get; init;}

    public override IReadOnlyList<TicTacToeBoard> ConstructBoards()
    => BoardBuilders.Select(b => new TicTacToeBoard(b)).ToList();
}
