namespace KriegspielTicTacToe.Model;

/// <summary>
/// Parameters to create a board, including the scoring settings for the board.
/// </summary>
public record BoardBuilder(sbyte Width, sbyte Height, GameScoring Scoring = null!) {
    public GameScoring Scoring = Scoring ?? GameScoring.Empty;
};
