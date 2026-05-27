namespace KriegspielTicTacToe.Model;

public record TicTacToePlayAction(
    int BoardIndex,
    int Col,
    int Row,
    Player Player
)
{}
