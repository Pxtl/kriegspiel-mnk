
using KriegspielTicTacToe.Model.Views;

namespace KriegspielTicTacToe.Model;

//TODO: Delete, unused.
/// <summary>
/// Non-generic interface for <see cref="Board"/> 
/// </summary>
public interface IBoard {
	Space[,] Spaces { get; init; }
	ScoreCard ScoreCard { get; }
	sbyte ColumnCount { get; }
	sbyte RowCount { get; }
	int SpaceCount { get; }
	bool IsFull { get; }
	bool IsDone { get; }

	IEnumerable<SpaceView> AsSpaceViewEnumerable(Player player);
	IEnumerable<SpaceView> AsSpaceViewEnumerable();
}
