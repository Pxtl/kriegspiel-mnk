namespace KriegspielTicTacToe.Model;

public interface IPlayActionBuffer {
    void ExecutePendingActions();
}

public class PlayActionBuffer<TPlayAction, TState> : IPlayActionBuffer
where TPlayAction : PlayAction<TPlayAction, TState>
where TState : IGameState {
    public List<TPlayAction> Actions {get;private set;} = [];

    public void Add(TPlayAction action) {
        Actions.Add(action);
    }

    public void Clear() {
        Actions.Clear();
    }

    public void ExecutePendingActions() {
        if (Actions.Count == 0) {
            return;
        }
        if (GameState == null) {
            throw new InvalidOperationException("Must be initialized first.");
        }
               
        foreach (var action in Actions) {            
            if (Actions.Any(otherA => action.IsActionCollision(otherA))) {
                action.DoActionCollision(GameState);
            } else {
                action.DoAction(GameState);
            }
        }

        Clear();
    }

    [JsonIgnore()]
    public TState? GameState { get; internal set; }
}
