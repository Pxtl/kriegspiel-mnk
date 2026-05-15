namespace KriegspielTicTacToe.Model;

public class PlayManager
{
    #region members
    public IReadOnlyList<char> Players {get;init;} = new List<char>();

    private int _currentTurnPlayerIndex = 0;
    /// <summary>
    /// index within list of *active* players - does not include resigned players.
    /// </summary>
    public int CurrentTurnPlayerIndex {
        get { return _currentTurnPlayerIndex; }
        set {
            _currentTurnPlayerIndex = value;
            RefreshCurrentPlayerTurnIndex(); 
        }
    }

    private int _roundIndex = 0;
    public int RoundIndex {
        get { return _roundIndex; }
        set { _roundIndex = value; }
    }

    public bool IsSynchronousMode {get; init;} = false;
    
    public HashSet<char> ResignedPlayersSet {get;init;} = new HashSet<char>();
    #endregion

    #region methods
    /// <summary>
    /// Advance to the next player's turn.  Do not call this if the current
    /// player has resigned.
    /// </summary>
    public void NextTurn() {
        CurrentTurnPlayerIndex += 1;
        if (CurrentTurnPlayerIndex == 0)
        {
            EndRound();
        }
    }

    public void EndRound()
    {
        CurrentTurnPlayerIndex = 0;
        if (IsSynchronousMode)
        {
            //TODO: Execute synchronous mode turn buffer
        }
        RoundIndex += 1;
    }

    /// <summary>
    /// It's possible that the current player turn can exceed the number of
    /// players.  In that event, wrap around.
    /// </summary>
    public void RefreshCurrentPlayerTurnIndex()
        => _currentTurnPlayerIndex = CurrentTurnPlayerIndex % ActivePlayers.Count();
    
    /// <summary>
    /// Test if the given player has resigned.
    /// </summary>
    public bool IsResignedPlayer(char player)
        => ResignedPlayersSet.Contains(player);

    /// <summary>
    /// Mark the given player as resigned. If it is the current player's turn,
    /// do *not* call NextTurn.
    /// </summary>
    public void ResignPlayer(char player) {
        ResignedPlayersSet.Add(player);
        RefreshCurrentPlayerTurnIndex();
    }

    /// <summary>
    /// True if the given player is able to take a turn.
    /// </summary>
    public bool CanTakeTurn(char? player)
        => player == CurrentTurnPlayer;
    #endregion

    #region helper properties
    [JsonIgnore()]
    public int NumberOfActivePlayers
        => Players
        .Where(p => !ResignedPlayersSet.Contains(p))
        .Count();

    
    /// <summary>
    /// Get all of the current active players.  Order is consistent.
    /// </summary>
    [JsonIgnore()]
    public IEnumerable<char> ActivePlayers
        => Players.Except(ResignedPlayersSet);

    /// <summary>
    /// Get the mark-char of the current-turn player.
    /// </summary>
    [JsonIgnore()]
    public char CurrentTurnPlayer 
        => ActivePlayers.ElementAt(CurrentTurnPlayerIndex);
    
    [JsonIgnore()]
    public bool IsRoundComplete 
        => (CurrentTurnPlayerIndex == 0) && (RoundIndex > 0);

    [JsonIgnore()]
    public string GameStateText
        =>  $"Player '{CurrentTurnPlayer}' turn.";


    #endregion
}