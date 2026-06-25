
using KriegspielTicTacToe.Model.Views;

namespace KriegspielTicTacToe.Model;

//TODO: Delete, unused.
/// <summary>
/// Non-generic interface for <see cref="Board"/> 
/// </summary>
public interface IBoard {
	Space[,] Spaces { get; init; }
	IEnumerable<string> SpaceNames { get; }
	ScoreCard ScoreCard { get; }
	sbyte ColumnCount { get; }
	sbyte RowCount { get; }
	int SpaceCount { get; }
	int SpaceNameLength { get; }
	bool IsFull { get; }
	bool IsDone { get; }

	IEnumerable<SpaceView> BoardAsSpaceViewEnumerable(Player player);
	IEnumerable<SpaceView> BoardAsSpaceViewEnumerable();
	string GetSpaceName(sbyte col, sbyte row);
	bool TryGetCoordinatesFromSpaceName(string spaceName, out sbyte resultCol, out sbyte resultRow);
}
