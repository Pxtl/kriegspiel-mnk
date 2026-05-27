namespace KriegspielTicTacToe.Model;

/// <summary>
/// Extension method for Contains on IEnumerable&lt;Player&gt;.
/// </summary>
public static class ContainsExtension {
    public static bool Contains<Player>(this IEnumerable<Player> players, Player player) => 
        players.Any(p => p!.Equals(player));
}
