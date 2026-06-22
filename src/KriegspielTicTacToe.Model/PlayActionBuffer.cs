namespace KriegspielTicTacToe.Model;

public class PlayActionBuffer {
    public List<TicTacToePlayAction> Actions {get;private set;} = [];

    public void Add(TicTacToePlayAction action) {
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
    public TicTacToeState? GameState { get; internal set; }
}
