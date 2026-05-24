namespace KriegspielTicTacToe.Model;

/// <summary>
/// Represents a player in the game. Stores their marker character.
/// </summary>
public record Player(char Value) {
    /// <summary>
    /// Create a Player from a 1-character string.
    /// </summary>
    public static Player FromString(string? value) =>
        char.TryParse(value, out var c) ? new Player(c) : default;
    
    /// <summary>
    /// Create a Player from a char.
    /// </summary>
    public static Player FromChar(char? value) => new Player((char?)value);
    
    /// <summary>
    /// Implicit conversion from 1-character string to Player.
    /// </summary>
    public static implicit operator Player(string? value) => FromString(value);
    
    /// <summary>
    /// Implicit conversion from char to Player.
    /// </summary>
    public static implicit operator Player(char? value) => new Player((char?)value);
    
    /// <summary>
    /// Implicit conversion from Player to string.
    /// </summary>
    public static implicit operator string(Player player) => player.Value.ToString() ?? string.Empty;
    
    /// <summary>
    /// Implicit conversion from Player to char.
    /// </summary>
    public static implicit operator char(Player player) => player.Value;
}
