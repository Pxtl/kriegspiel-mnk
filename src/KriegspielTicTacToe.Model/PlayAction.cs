namespace KriegspielTicTacToe.Model;

public abstract record PlayAction<TPlayAction, TState>
where TPlayAction : PlayAction<TPlayAction, TState>
where TState : IGameState { 
    public abstract void DoAction(TState gameState);
    public abstract bool IsActionCollision(TPlayAction otherAction);
    public abstract void DoActionCollision(TState gameState);
}
