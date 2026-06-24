namespace KriegspielTicTacToe.Model.Tests;

public class BoardTests {
    #region board size
    [Fact]
    public void Width_Default() {
        var board = new Board() {};
        board.ColumnCount.Should().Be(1);
    }

    [Fact]
    public void Width_3x3() {
        var board = new Board(3, 3, new TicTacToeRuleset());
        board.ColumnCount.Should().Be(3);
    }

    [Fact]
    public void Width_100() {
        var board = new Board(100, 10, new TicTacToeRuleset());
        board.ColumnCount.Should().Be(100);
    }

    [Fact]
    public void Height_Default() {
        var board = new Board();
        board.RowCount.Should().Be(1);
    }

    [Fact]
    public void Height_3x3() {
        var board = new Board(3, 3, new TicTacToeRuleset());
        board.RowCount.Should().Be(3);
    }

    [Fact]
    public void Height_10() {
        var board = new Board(100, 10, new TicTacToeRuleset());
        board.RowCount.Should().Be(10);
    }
    #endregion

    #region SpaceNameLength
    [Fact]
    public void SpaceNameLength_3x3Board_Returns1() {
        var board = new Board(3, 3, new TicTacToeRuleset());
        board.SpaceNameLength.Should().Be(1);
    }

    [Fact]
    public void SpaceNameLength_100x10Board_ReturnsCorrect() {
        var board = new Board(100, 10, new TicTacToeRuleset());
        var spaceCount = board.SpaceCount;
        board.SpaceNameLength.Should().Be((int)Math.Floor(Math.Log10(spaceCount)) + 1);
    }
    #endregion

    #region GetBoardAsEnumerable
    [Fact]
    public void BoardAsEnumerable_ReturnsAllSpaces_ExpectedCount() {
        var board = new Board(3, 3, new TicTacToeRuleset());
        var spaces = board.BoardAsEnumerable().ToList();
        spaces.Count.Should().Be(9);
    }

    [Fact]
    public void BoardAsEnumerable_ReturnsAllSpaces_100x10() {
        var board = new Board(100, 10, new TicTacToeRuleset());
        var spaces = board.BoardAsEnumerable().ToList();
        spaces.Count.Should().Be(1000);
    }
    #endregion

    #region GetSpaceName
    [Fact]
    public void GetSpaceName_TopicCorner_ReturnsPositive() {
        // top-left corner (row 0, col 2) in 3x3
        var board = new Board(3, 3, new TicTacToeRuleset());
        var code = board.GetSpaceNameAsInt(2, 0);
        code.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetSpaceName_RightOfTopicCorner_ReturnsPositive() {
        // 3x3 board: (row 1, col 2)
        var board = new Board(3, 3, new TicTacToeRuleset());
        var code = board.GetSpaceNameAsInt(2, 1);
        code.Should().BeGreaterThan(0);
    }
    #endregion

    #region TryGetCoordinatesFromSpaceName
    [Fact]
    public void TryGetCoordinatesFromSpaceName_Valid() {
        var board = new Board(3, 3, new TicTacToeRuleset());
        var ok = board.TryGetCoordinatesFromSpaceNameAsInt(1, out var col, out var row);
        ok.Should().BeTrue();
        col.Should().BeLessThan(board.ColumnCount);
        row.Should().BeLessThan(board.RowCount);
    }

    [Fact]
    public void TryGetCoordinatesFromSpaceName_Invalid() {
        var board = new Board(3, 3, new TicTacToeRuleset());
        var ok = board.TryGetCoordinatesFromSpaceNameAsInt(99, out _, out _);
        ok.Should().BeFalse();
    }
    #endregion

    #region MakeKnownToPlayer

    [Fact]
    public void MakeKnownToPlayer_MarksToPlayer_IsKnown() {
        var board = new Board(3, 3, new TicTacToeRuleset());
        board.Spaces[0, 0].Mark = "X";
        board.Spaces[0, 0].MakeKnownToPlayer("X");

        board.Spaces[0, 0].KnownToPlayersSet.Should().Contain("X");
    }

    [Fact]
    public void MakeKnownToPlayer_MarksToAnotherPlayer_IsKnown() {
        var board = new Board(3, 3, new TicTacToeRuleset());
        board.Spaces[0, 0].Mark = "X";
        board.Spaces[0, 0].MakeKnownToPlayer("O");

        board.Spaces[0, 0].KnownToPlayersSet.Should().Contain("O");
    }
    #endregion
}
