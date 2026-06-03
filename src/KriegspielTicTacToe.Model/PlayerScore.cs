namespace KriegspielTicTacToe.Model;

/// <summary>
/// Score for a single player
/// </summary>
public record struct PlayerScore(Player Player, int Score) {
    public static implicit operator ScoreCard(PlayerScore p) => new ScoreCard(p);   
}
