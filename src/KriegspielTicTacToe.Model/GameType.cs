namespace KriegspielTicTacToe.Model;

public interface IGameType {
    public PlayManagerFactory PlayManagerFactory { get;}
}

public abstract record GameType<TBoard>() : IGameType
where TBoard : Board {
	public PlayManagerFactory PlayManagerFactory { get; init; } = RoundRobinPlayManagerFactory.Instance;

	public abstract IReadOnlyList<TBoard> ConstructBoards();
}