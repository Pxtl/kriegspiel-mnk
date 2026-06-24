namespace KriegspielTicTacToe.Model;

/// <summary>
/// Parameter and logic for scoring a board. Default implementation gives nobody
/// any points ever, you must override Score to get actual useful gameplay.
/// </summary>
public record GameScoring {
    public static GameScoring Empty {get;} = new GameScoring();
    public virtual bool IsDone(Board board)
    => false;

	public virtual ScoreCard Score(Board board)
    => ScoreCard.Empty;
};
