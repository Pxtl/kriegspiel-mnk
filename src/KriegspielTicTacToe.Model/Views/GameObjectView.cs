namespace KriegspielTicTacToe.Model.Views;

//TODO: Should these *contain* their data so they can be serialized without leaking the underlying data to the client?

/// <summary>
/// Abstract base class for a single player's views of Game Objects (or no
/// player for omniscient spectator).
/// </summary>
/// <typeparam name="T">The Game Object type that will be wrapped.</typeparam>
public abstract record GameObjectView<T> {
    /// <summary>
    /// Construct a new view.  
    /// </summary>
    /// <param name="GameState">The gamestate to wrap.</param>
    /// <param name="Player">The player's view of that gamestate.  Null for omniscient spectator.</param>
    public GameObjectView(T value, Player? player) {
        Value = value;
        Player = player;
    }

    #region Data Properties
    [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
    public Player? Player {get; init;}
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    protected T Value { get; init; }
    #endregion
}
