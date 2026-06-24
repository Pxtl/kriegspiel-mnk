namespace KriegspielTicTacToe.Model;

/// <summary>
/// Non-generic interface for <see cref="GameState{TState, TTemplate, TScoring, TAction}"/> 
/// </summary>
public interface IGameState {
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    PlayManager PlayManager {get;}
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    IReadOnlyList<Board> Boards {get;}
}
