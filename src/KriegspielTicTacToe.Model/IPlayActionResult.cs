namespace KriegspielTicTacToe.Model;

public interface IPlayActionResult {
    /// <summary>
    /// True if the UI should re-render the board(s).
    /// </summary>
    bool IsViewChanged { get; }
    /// <summary>
    /// True if the player's turn is over.
    /// </summary>
    bool IsTurnDone { get; }
}

public record struct Resigned(Player Player)
: IPlayActionResult {
    public bool IsViewChanged => true;
    public bool IsTurnDone => true;
    public string ResultText => $"Player {Player} is resigning.";
}
public record struct Enqueued(bool IsViewChanged, string SpaceName) 
: IPlayActionResult {
    public bool IsTurnDone => true;
    public string ResultText => $"Played space {SpaceName}.";
}
public record struct AlreadyPlayed(Player Player)
: IPlayActionResult {
    public bool IsViewChanged => false;
    public bool IsTurnDone => false;
    public string ResultText => $"Invalid space, that space is already known to player {Player}.";
};
public record struct NewlyLearned(string Mark)
: IPlayActionResult{
    public bool IsViewChanged => true;
    public bool IsTurnDone => true;
    public string ResultText => $"Space already filled: '{Mark}'.";
};