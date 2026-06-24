namespace KriegspielTicTacToe.Model;

public abstract record GameView<TState, TTemplate, TScoring, TAction>
where TState : GameState<TState, TTemplate, TScoring, TAction>
where TTemplate : GameTemplate<TScoring>
where TScoring : GameScoring
where TAction : PlayAction<TAction, TState> {
    public GameView (Player? player, GameState<TState, TTemplate, TScoring, TAction> gameState) {
        Player = player;
        GameState = gameState;
    }
    public Player? Player {get; init;}
    public GameState<TState, TTemplate, TScoring, TAction> GameState { get; init; }

    public IReadOnlyList<Board> Boards => GameState.Boards;
    public bool CanTakeTurn => GameState.PlayManager.CanTakeTurn(Player);
    public bool IsGameOver => GameState.IsGameOver;
}