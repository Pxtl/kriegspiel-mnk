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
        var board = new Board(3, 3, new MNKRuleset());
        board.ColumnCount.Should().Be(3);
    }

    [Fact]
    public void Width_100() {
        var board = new Board(100, 10, new MNKRuleset());
        board.ColumnCount.Should().Be(100);
    }

    [Fact]
    public void Height_Default() {
        var board = new Board();
        board.RowCount.Should().Be(1);
    }

    [Fact]
    public void Height_3x3() {
        var board = new Board(3, 3, new MNKRuleset());
        board.RowCount.Should().Be(3);
    }

    [Fact]
    public void Height_10() {
        var board = new Board(100, 10, new MNKRuleset());
        board.RowCount.Should().Be(10);
    }
    #endregion

    #region SpaceNameLength
    [Fact]
    public void SpaceNameLength_3x3Board_Returns1() {
        var board = new Board(3, 3, new MNKRuleset());
        board.SpaceNameLength.Should().Be(1);
    }

    [Fact]
    public void SpaceNameLength_26x10Board_ReturnsCorrect() {
        var board = new Board(26, 10, new MNKRuleset());
        var spaceCount = board.SpaceCount;
        board.SpaceNameLength.Should().Be(3); //2 digit s for 1-10, 1 digit for letter.
    }
    #endregion

    #region GetBoardAsEnumerable
    [Fact]
    public void BoardAsEnumerable_ReturnsAllSpaces_ExpectedCount() {
        var board = new Board(3, 3, new MNKRuleset());
        var spaces = board.BoardAsSpaceViewEnumerable().ToList();
        spaces.Count.Should().Be(9);
    }

    [Fact]
    public void BoardAsEnumerable_ReturnsAllSpaces_26x10() {
        var board = new Board(26, 10, new MNKRuleset());
        var spaces = board.BoardAsSpaceViewEnumerable().ToList();
        spaces.Count.Should().Be(260);
    }
    #endregion

    #region GetSpaceName
    [Fact]
    public void GetSpaceName_3x3TopRightSpace_AsExpected() {
        // top-right corner (row 0, col 2) in 3x3
        // board matches layout of numpad, 1 is bottom left.
        var board = new Board(3, 3, new MNKRuleset());
        var code = board.GetSpaceName(2, 0);
        code.Should().Be("9");
    }

    [Fact]
    public void GetSpaceName_4x4BottomLeftSpace_AsExpected() {
        // 4x4 board: (row 3, col 0), uses letter-number format, bottom left space
        var board = new Board(4, 4, new MNKRuleset());
        var code = board.GetSpaceName(0, 3);
        code.Should().Be("A1");
    }
    #endregion

    #region TryGetCoordinatesFromSpaceName
    [Fact]
    public void TryGetCoordinatesFrom3x3SpaceName_AsExpected() {
        var board = new Board(3, 3, new MNKRuleset());
        var ok = board.TryGetCoordinatesFromSpaceName("1", out var col, out var row);
        ok.Should().BeTrue();
        col.Should().Be(0);
        row.Should().Be(2);
    }

    [Fact]
    public void TryGetCoordinatesFrom3x3SpaceName_Invalid() {
        var board = new Board(3, 3, new MNKRuleset());
        var ok = board.TryGetCoordinatesFromSpaceName("99", out _, out _);
        ok.Should().BeFalse();
    }

    [Fact]
    public void TryGetCoordinatesFrom4x4BottomLeftSpace_AsExpected() {
        // 4x4 board: (row 3, col 0), uses letter-number format, bottom left space
        var board = new Board(4, 4, new MNKRuleset());
        var ok = board.TryGetCoordinatesFromSpaceName("A1", out var col, out var row);
        ok.Should().BeTrue();
        col.Should().Be(0);
        row.Should().Be(3);
    }


    #endregion

    #region MakeKnownToPlayer

    [Fact]
    public void MakeKnownToPlayer_MarksToPlayer_IsKnown() {
        var board = new Board(3, 3, new MNKRuleset());
        board.Spaces[0, 0].Mark = "X";
        board.Spaces[0, 0].MakeKnownToPlayer("X");

        board.Spaces[0, 0].KnownToPlayersSet.Should().Contain("X");
    }

    [Fact]
    public void MakeKnownToPlayer_MarksToAnotherPlayer_IsKnown() {
        var board = new Board(3, 3, new MNKRuleset());
        board.Spaces[0, 0].Mark = "X";
        board.Spaces[0, 0].MakeKnownToPlayer("O");

        board.Spaces[0, 0].KnownToPlayersSet.Should().Contain("O");
    }
    #endregion
}
