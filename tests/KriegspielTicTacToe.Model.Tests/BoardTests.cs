namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using Xunit;

public class BoardTests {
    #region board size
    [Fact]
    public void Width_Default() {
        var board = new Board();
        board.ColumnCount.Should().Be(1);
    }

    [Fact]
    public void Width_3x3() {
        var board = new Board(3, 3);
        board.ColumnCount.Should().Be(3);
    }

    [Fact]
    public void Width_100() {
        var board = new Board(100, 10);
        board.ColumnCount.Should().Be(100);
    }

    [Fact]
    public void Height_Default() {
        var board = new Board();
        board.RowCount.Should().Be(1);
    }

    [Fact]
    public void Height_3x3() {
        var board = new Board(3, 3);
        board.RowCount.Should().Be(3);
    }

    [Fact]
    public void Height_10() {
        var board = new Board(100, 10);
        board.RowCount.Should().Be(10);
    }

    #endregion

    #region SpaceIndexCodeLength
    [Fact]
    public void SpaceIndexCodeLength_3x3Board_Returns1() {
        var board = new Board(3, 3);
        board.SpaceIndexCodeLength.Should().Be(1);
    }

    [Fact]
    public void SpaceIndexCodeLength_100x10Board_Returns4() {
        var board = new Board(100, 10);
        board.SpaceIndexCodeLength.Should().Be(4);
    }
    #endregion

    #region GetBoardAsEnumerable
    [Fact]
    public void BoardAsEnumerable_ReturnsAllSpaces_ExpectedCount() {
        var board = new Board(3, 3);
        var spaces = board.BoardAsEnumerable().ToList();
        spaces.Count.Should().Be(9);
    }

    [Fact]
    public void BoardAsEnumerable_ReturnsAllSpaces_100x10() {
        var board = new Board(100, 10);
        var spaces = board.BoardAsEnumerable().ToList();
        spaces.Count.Should().Be(1000);
    }
    #endregion

    #region GetSpaceIndexCode
    [Fact]
    public void GetSpaceIndexCode_TopicCorner_Returns9() {
        // top-left corner (row 0, col 2) in 3x3
        var board = new Board(3, 3);
        board.GetSpaceIndexCode(2, 0).Should().Be(9);
    }

    [Fact]
    public void GetSpaceIndexCode_RightOfTopicCorner_Returns6() {
        // 3x3 board: (row 1, col 2)
        var board = new Board(3, 3);
        board.GetSpaceIndexCode(2, 1).Should().Be(6);
    }
    #endregion

    #region TryGetCoordinatesFromSpaceIndexCode
    [Fact]
    public void TryGetCoordinatesFromSpaceIndexCode_Valid() {
        var board = new Board(3, 3);
        var ok = board.TryGetCoordinatesFromSpaceIndexCode(1, out var col, out var row);
        ok.Should().BeTrue();
        col.Should().Be(0);
        row.Should().Be(2);
    }

    [Fact]
    public void TryGetCoordinatesFromSpaceIndexCode_Invalid() {
        var board = new Board(3, 3);
        var ok = board.TryGetCoordinatesFromSpaceIndexCode(99, out _, out _);
        ok.Should().BeFalse();
    }
    #endregion

    #region SpaceIndexCodeLength_Calc
    [Fact]
    public void SpaceIndexCodeLength_CalculatesCorrectly() {
        var board = new Board(3, 3);
        var spaceCount = board.Spaces.GetLength(0) * board.Spaces.GetLength(1);
        board.SpaceIndexCodeLength.Should().Be( (int)Math.Floor(Math.Log10(spaceCount)) + 1);
    }

    [Fact]
    public void SpaceIndexCodeLength_LargerBoard() {
        var board = new Board(100, 10);
        var spaceCount = board.Spaces.GetLength(0) * board.Spaces.GetLength(1);
        board.SpaceIndexCodeLength.Should().Be( (int)Math.Floor(Math.Log10(spaceCount)) + 1);
    }
    #endregion
}
