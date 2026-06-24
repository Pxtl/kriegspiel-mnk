namespace KriegspielTicTacToe.Model;

public interface IGameTemplate {
    PlayManagerFactory PlayManagerFactory { get;}
    
    IReadOnlyList<Board> ConstructBoards();
}
