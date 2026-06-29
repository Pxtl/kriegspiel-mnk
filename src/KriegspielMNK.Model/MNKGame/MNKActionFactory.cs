using KriegspielMNK.Model.Template;

namespace KriegspielMNK.Model.MNKGame;

[ModelSerializable]
public record MNKActionFactory
: GameActionFactoryForSpace {
	public override string Name => "play space";

	public override GameAction Create(sbyte boardIndex, sbyte col, sbyte row)
    => new MNKAction(boardIndex, col, row);
}