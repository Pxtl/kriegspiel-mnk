namespace KriegspielTicTacToe.Model;

public interface IGameState {
    PlayManager PlayManager {get;}
    IReadOnlyList<Board> Boards {get;}
}
