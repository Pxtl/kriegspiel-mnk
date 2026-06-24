namespace KriegspielTicTacToe.Model.TicTacToe;

/// <summary>
/// Represents a game type configuration including board builders and play mode settings.
/// </summary>
public record TicTacToeTemplate
: GameTemplate<TicTacToeScoring> {
    public TicTacToeTemplate(
        IEnumerable<BoardBuilder> boardBuilders,
        bool isSynchronousMode
    )
    : base() {
        BoardBuilders = boardBuilders;
        PlayManagerFactory = isSynchronousMode 
            ? SynchronizedPlayManagerFactory.Instance
            : RoundRobinPlayManagerFactory.Instance;
    }

    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    public IEnumerable<BoardBuilder> BoardBuilders {get; init;}

    public override IReadOnlyList<Board> ConstructBoards()
    => BoardBuilders.Select(b => new Board(b)).ToList();
}
