namespace KriegspielTicTacToe.Model;

/// <summary>
/// Parameter and logic for scoring a board. Default implementation gives nobody
/// any points ever, you must override Score to get actual useful gameplay.
/// </summary>
public record GameRuleset {
	public static GameRuleset Empty { get; } = new GameRuleset();
	public virtual bool IsDone(Board board)
	=> false;

	public virtual ScoreCard Score(Board board)
		=> ScoreCard.Empty;
};
