namespace KriegspielTicTacToe.Model;

public abstract record GameTemplate<TScoring>() : IGameTemplate
where TScoring : GameScoring {
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
	public PlayManagerFactory PlayManagerFactory { get; init; } = RoundRobinPlayManagerFactory.Instance;

	public abstract IReadOnlyList<Board> ConstructBoards();
}