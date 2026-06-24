using System.Runtime.Serialization;
using OneOf;
using OneOf.Types;

namespace KriegspielTicTacToe.Model;

public abstract record GameState<TState, TTemplate, TScoring, TAction> : IGameState
where TState : GameState<TState, TTemplate, TScoring, TAction>
where TTemplate : GameTemplate<TScoring>
where TScoring : GameScoring 
where TAction : PlayAction<TAction, TState> {
    #region Constructors
    public GameState() { 
        // will probably get removed when members are initialized.
        PlayManager = new RoundRobinPlayManager([]);
        Boards = [];
    }

    public GameState(
        Player[] players,
        TTemplate gameType,
        bool isRandomPlayerOrder
    ) {
        if(isRandomPlayerOrder) {
            Random.Shared.Shuffle(players);
        }

        PlayManager = gameType.PlayManagerFactory.Create(players);
        Boards = gameType.ConstructBoards();
        Initialize();
    }
    #endregion

    #region Data Properties
    [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
    public virtual PlayManager PlayManager { get; init; }

    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.None)]
    public virtual IReadOnlyList<Board> Boards { get; init; }

    [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
    public PlayActionBuffer<TAction, TState> PlayActionBuffer { get; init; } = new PlayActionBuffer<TAction, TState>();
    #endregion

    #region Initializer
    public virtual void Initialize() {
        PlayActionBuffer.GameState = (TState)this;
        PlayManager.PlayActionBuffer = PlayActionBuffer;
    }

    [OnDeserialized]
    public void Initialize(StreamingContext context) {
        Initialize();
    }
    #endregion

    [JsonIgnore()]
    public abstract ScoreCard ScoreCard { get; }

    public OneOf<NotFound, Result<int>> GetBoardIndexByName(string boardName) {
        var boardNameAsInt = int.Parse(boardName);
        var boardIndex = boardNameAsInt - 1;
        return (boardIndex >= 0 && boardIndex < Boards.Count) 
            ? new Result<int>(boardIndex)
            : new NotFound();
    }

    public IEnumerable<string> BoardNames {
        get {
            for(var i = 1; i <= Boards.Count; i += 1) {
                yield return i.ToString();
            }
        }
    }
        
    public Board GetBoardByIndex(int boardIndex)
    => Boards[boardIndex];

    [JsonIgnore()]
    public IEnumerable<int> ActiveBoardIndices {get {
        for(int i = 0; i < Boards.Count; i+=1) {
            if(!Boards[i].IsDone) {
                yield return i; 
            }
        }
    }}
    
    [JsonIgnore()]
    public int? SingleActiveBoardIndex {get {
        var firstElements = ActiveBoardIndices.Take(2).ToArray();
        return (firstElements.Length == 1) ? firstElements.Single() : null;
    }}

    [JsonIgnore()]
    public string GameStateText
        => IsGameOver 
        ? (Winner == null
            ? "Game over. Tie game."
            : $"Game over. {Winner} wins."
        )
        : (PlayManager.GameStateText 
            + Environment.NewLine
            + ResignedPlayersText
        );

    public string ResignedPlayersText
        => PlayManager.ResignedPlayersSet.Count > 0
        ? $"Resigned players: {string.Join(", ", PlayManager.ResignedPlayersSet.OrderBy(p => p.Mark))}"
        : "";
    
    [JsonIgnore()]
    public Player? Winner {
        get {
            if(!IsGameOver) return null;
            if(PlayManager.ActivePlayers.Count() == 1) return PlayManager.ActivePlayers.Single();
            return null;
        }
    }

    [JsonIgnore()]
    public bool IsGameOver
    => Boards.All(b => b.IsDone) || PlayManager.ActivePlayers.Count() == 1;
}
