namespace KriegspielTicTacToe.Model.TicTacToe;

public record TicTacToeView : GameView<TicTacToeState, TicTacToeTemplate, TicTacToeScoring, TicTacToePlayAction> {
    public TicTacToeView (Player? player, TicTacToeState gameState)
    : base(player, gameState) {
        
    }
}
