namespace KriegspielTicTacToe.Model;

/// <summary>
/// Non-generic interface for <see cref="Board{TScoring}"/> 
/// </summary>
public interface IBoard {
	Space[,] Spaces { get; init; }
	IEnumerable<string> SpaceNames { get; }
	ScoreCard ScoreCard { get; }
	int ColumnCount { get; }
	int RowCount { get; }
	int SpaceCount { get; }
	int SpaceNameLength { get; }
	bool IsFull { get; }
	bool IsDone { get; }

	IEnumerable<SpaceEnumerator> BoardAsEnumerable();
	int GetSpaceNameAsInt(int col, int row);
	bool TryGetCoordinatesFromSpaceNameAsInt(int spaceName, out int resultCol, out int resultRow);
}
