namespace KriegspielTicTacToe.Model.Template;

/// <summary>
/// A game template includes all the metadata needed to describe the type of a
/// game, but not the actual list of players or active state.
/// </summary>
public abstract record GameTemplate()
: IGameTemplate {
	public string? CommandName { get; init; }
	public string? Description { get; init; }
	public IEnumerable<int> LegalPlayerCounts { get; init; } = Enumerable.Range(2, 32).ToArray();
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
	public PlayManagerFactory PlayManagerFactory { get; init; } = RoundRobinPlayManagerFactory.Instance;

	public abstract IReadOnlyList<Board> CreateBoards();

	public abstract void InitializeGame(IGameState gameState);
}