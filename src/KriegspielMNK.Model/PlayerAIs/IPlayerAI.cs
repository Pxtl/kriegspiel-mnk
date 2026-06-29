using KriegspielMNK.Model.Template;
using KriegspielMNK.Model.Views;

namespace KriegspielMNK.Model.PlayerAIs;

public interface IPlayerAI {
	void Attempt(GameView gameView, IEnumerable<GameActionFactory> actionFactories);
	public string Description { get; }
}
