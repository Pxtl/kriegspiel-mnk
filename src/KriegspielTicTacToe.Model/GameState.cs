using System.Runtime.Serialization;
using KriegspielTicTacToe.Model.Template;
using KriegspielTicTacToe.Model.Views;

namespace KriegspielTicTacToe.Model;

public record GameState<TAction> : IGameState
where TAction : PlayAction {
    #region Constructors
    public GameState() { 
        // unusable default values will probably get removed when members are
        // initialized.
        PlayManager = new RoundRobinPlayManager([]);
        Boards = [];
    }

    public GameState(
        Player[] players,
        IGameTemplate gameTemplate,
        bool isRandomPlayerOrder
    ) {
        if(isRandomPlayerOrder) {
            Random.Shared.Shuffle(players);
        }

        PlayManager = gameTemplate.PlayManagerFactory.Create(players);
        Boards = gameTemplate.CreateBoards();
        Initialize();

        if (Boards.Count > 1 && Boards.Any(b => b.RowCount > 9)) {
            throw new ApplicationException(
                "Cannot start game. Current board-renderer does not support boards that are taller than 9 spaces in multi-board games."
            );
        }

        if (!gameTemplate.LegalPlayerCounts.Contains(players.Length)) {
            throw new ApplicationException(
                "Cannot start game. This game only supports the following player-counts: "
                    + string.Join(", ", gameTemplate.LegalPlayerCounts)
                    + Environment.NewLine
                    + $"You have provided {players.Length} player(s)."
            );
        }

        gameTemplate.InitializeGame(this);
    }
    #endregion

    #region Data Properties
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    public virtual PlayManager PlayManager { get; init; }

    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.None)]
    public virtual IReadOnlyList<Board> Boards { get; init; }

    [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
    public PlayActionQueue<TAction> ActionQueue { get; init; } = new PlayActionQueue<TAction>();
    #endregion

    #region Initializer
    public virtual void Initialize() {
        ActionQueue.GameState = this;
        PlayManager.ActionQueue = ActionQueue;
    }

    [OnDeserialized]
    public void Initialize(StreamingContext context) {
        Initialize();
    }
    #endregion

    public GameView GetView(Player? player)
    => new(this, player);

    #region GameState
    /// <summary>
    /// Scorecard for all active (non-resigned) players
    /// </summary>
    [JsonIgnore()]
    public ScoreCard ScoreCard 
    => AllPlayersScoreCard.FilterByPlayers(PlayManager.ActivePlayers);

    
    [JsonIgnore()]
    public ScoreCard AllPlayersScoreCard
    => PlayManager.Players.BlankPlayersScoreCard()  //make sure all active players are in the scorecard even those with 0.
        + Boards.Select(b => b.ScoreCard).SumScoreCards();


    [JsonIgnore()]
    public virtual IEnumerable<Player> Winners { get {
        if(!IsGameOver) {
            return [];
        }
        if(PlayManager.ActivePlayers.Count() == 1) {
            return PlayManager.ActivePlayers;
        }
        else {
            return ScoreCard.Highest.PlayerScores.Select(s => s.Player);
        }
    }}

    [JsonIgnore()]
    public bool IsGameOver
    => Boards.All(b => b.IsDone) || PlayManager.ActivePlayers.Count() == 1;


    [JsonIgnore()]
    public string GameStateText
    => IsGameOver 
        ? (Winners.Count() == 0
            ? "Game over. Nobody wins."
            : $"Game over. {string.Join(" and ", Winners)} win(s)."
        )
        : (PlayManager.GameStateText 
            + Environment.NewLine
            + ResignedPlayersText
        );

    [JsonIgnore()]
    public string ResignedPlayersText
    => PlayManager.ResignedPlayersSet.Count > 0
        ? $"Resigned players: {string.Join(", ", PlayManager.ResignedPlayersSet.OrderBy(p => p.Mark))}"
        : "";
    #endregion

    #region board management

    [JsonIgnore()]
    public IEnumerable<sbyte> ActiveBoardIndices { get {
        for(sbyte i = 0; i < Boards.Count; i+=1) {
            if(!Boards[i].IsDone) {
                yield return i; 
            }
        }
    }}
    
    /// <summary>
    /// If there is only one active board, return its index.  Otherwise, return
    /// null.
    /// </summary>
    [JsonIgnore()]
    public sbyte? SingleActiveBoardIndex { get {
        var firstElements = ActiveBoardIndices.Take(2).ToArray();
        return (firstElements.Length == 1) ? firstElements.Single() : null;
    }}

    public Board GetBoardByIndex(sbyte boardIndex)
    => Boards[boardIndex];

	public void Enqueue(TAction playAction) {
		ActionQueue.Add(playAction);
	}
	#endregion
}

public struct BoardIsDone;
