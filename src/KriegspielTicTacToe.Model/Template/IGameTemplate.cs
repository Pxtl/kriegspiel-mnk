namespace KriegspielTicTacToe.Model.Template;

public interface IGameTemplate {
    string? CommandName { get; }
	string? Description { get; }
	IEnumerable<int> LegalPlayerCounts { get; }
    PlayManagerFactory PlayManagerFactory { get;}
    
    IReadOnlyList<Board> CreateBoards();
	void InitializeGame (GameState gameState);
}
