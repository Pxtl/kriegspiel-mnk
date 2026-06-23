namespace KriegspielTicTacToe.Model;

public abstract record GameState<TState, TGame, TBoard, TAction> : IGameState
where TState : GameState<TState, TGame, TBoard, TAction>
where TGame : GameType<TBoard>
where TBoard : Board 
where TAction : PlayAction<TAction, TState> {
    public GameState(
        Player[] players,
        TGame gameType,
        bool isRandomPlayerOrder
    ) {
        if(isRandomPlayerOrder) {
            Random.Shared.Shuffle(players);
        }

        PlayManager = gameType.PlayManagerFactory.Create(players);
        Boards = gameType.ConstructBoards();
        Initialize();
    }

    public virtual PlayManager PlayManager {get;init;}
    public virtual IReadOnlyList<TBoard> Boards {get;init;}
    IReadOnlyList<Board> IGameState.Boards => Boards;
    public virtual PlayActionBuffer<TAction, TState> PlayActionBuffer {get; init;} = new PlayActionBuffer<TAction, TState>();
    public void Initialize() {
        PlayActionBuffer.GameState = (TState)this;
        PlayManager.PlayActionBuffer = PlayActionBuffer;
    }
}
