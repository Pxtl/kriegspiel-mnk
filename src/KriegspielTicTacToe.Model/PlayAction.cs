using System.ComponentModel.DataAnnotations;
using OneOf;
using OneOf.Types;

namespace KriegspielTicTacToe.Model;

public abstract record PlayAction {
    #region constructors
    [Obsolete("Default constructor is for use with deserialing.  If used manually, ensure that all required members are set.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
	public PlayAction() { }
#pragma warning restore CS8618
	public PlayAction(Player player) {
        Player = player;
    }
    #endregion
    #region data members
    [Required]
    [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
    public Player Player {get;set;}
    #endregion
    public abstract OneOf<IsLegalToQueue, NewlyLearned, AlreadyPlayed> Attempt(IGameState gameState);
    public abstract void DoAction(IGameState gameState);
    public abstract bool IsActionCollision(PlayAction otherAction);
    public abstract void DoActionCollision(IGameState gameState);
}
public record struct IsLegalToQueue;
public record struct AlreadyPlayed;
public record struct NewlyLearned(string Mark);