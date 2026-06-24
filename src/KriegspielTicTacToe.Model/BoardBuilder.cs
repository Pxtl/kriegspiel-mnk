namespace KriegspielTicTacToe.Model;

/// <summary>
/// Parameters to create a board, including the scoring settings for the board.
/// </summary>
public record BoardBuilder(sbyte Width, sbyte Height, GameRuleset Ruleset = null!) {
    public GameRuleset Ruleset = Ruleset ?? GameRuleset.Empty;
};
