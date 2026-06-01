namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using KriegspielTicTacToe.Model;
using Xunit;

#pragma warning disable IDE0027 // Simplify nested expression
public class BoardRendererTests {
    
   [Fact]
    public void DrawBoards_3x3_ReturnsBoardGridString() {
        var boardBuilder = new BoardBuilder(3, 3);
        var state = new TicTacToeState(new[] { 'X', 'O' }, new[] { boardBuilder }, isRandomPlayerOrder: false, isSynchronousMode: false);
        
        var currentPlayer = state.PlayManager.PlayersAvailableForTurn.First();
        
        var actual = BoardRenderer.DrawBoards(state, currentPlayer, activeBoardIndex: null);
        var expected = @"
  ┌───┬───┬───┐
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  └───┴───┴───┘
  "
            .Substring(1) //skip the leading linebreak needed for legibility
            .TrimEnd();

        actual.TrimEnd().Should().Be(expected);
    }

    [Fact]
    public void DrawBoards_3x3WithOneMove_ReturnsBoardGridString() {
        var boardBuilder = new BoardBuilder(3, 3);
        var state = new TicTacToeState(
            new[] { 'X', 'O' },
            new[] { boardBuilder },
            isRandomPlayerOrder: false,
            isSynchronousMode: false);
        
        var currentPlayer = new Player("X");
        state.PlaySpace(0, 0, 0, currentPlayer);
        state.PlayManager.EndTurn(currentPlayer, out var _);
        var expected = @"
  ┌───┬───┬───┐
  │ X │   │   │
  ├───┼───┼───┤
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  └───┴───┴───┘
  "
            .Substring(1) //skip the leading linebreak needed for legibility
            .TrimEnd();

        var actual = BoardRenderer.DrawBoards(state, currentPlayer, activeBoardIndex: null);    
        actual.TrimEnd().Should().Be(expected);
   }

    [Fact]
    public void DrawBoards_3x3MultipleBoardsWithWrapping_ReturnWrappedBoardGridString() {
        var boardBuilder3x3 = new BoardBuilder(3, 3);
        
        var state = new TicTacToeState(
            new[] { 'X', 'O' },
            new[] { boardBuilder3x3, boardBuilder3x3, boardBuilder3x3 },
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        var currentPlayer = new Player("X");
        // wrap halfway through 3rd board
        var actual = BoardRenderer.DrawBoards(state, currentPlayer, activeBoardIndex: null, maxWidth:42);
        var expected = @"
 1┌───┬───┬───┐ 2┌───┬───┬───┐
  │   │   │   │  │   │   │   │
  ├───┼───┼───┤  ├───┼───┼───┤
  │   │   │   │  │   │   │   │
  ├───┼───┼───┤  ├───┼───┼───┤
  │   │   │   │  │   │   │   │
  └───┴───┴───┘  └───┴───┴───┘
 3┌───┬───┬───┐
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  └───┴───┴───┘
  "
            .Substring(1) //skip the leading linebreak needed for legibility
            .TrimEnd();

        actual.TrimEnd().Should().Be(expected);
    }

        [Fact]
    public void DrawBoards_3x3MultipleBoardsWithNarrowWrapping_ReturnWrappedBoardGridString() {
        var boardBuilder3x3 = new BoardBuilder(3, 3);
        
        var state = new TicTacToeState(
            new[] { 'X', 'O' },
            new[] { boardBuilder3x3, boardBuilder3x3, boardBuilder3x3 },
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );

        var currentPlayer = new Player("X");
        // 0 means wrap as tight as possible.
        var actual = BoardRenderer.DrawBoards(state, currentPlayer, activeBoardIndex: null, maxWidth:0);
        var expected = @"
 1┌───┬───┬───┐
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  └───┴───┴───┘
 2┌───┬───┬───┐
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  └───┴───┴───┘
 3┌───┬───┬───┐
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  ├───┼───┼───┤
  │   │   │   │
  └───┴───┴───┘
  "
            .Substring(1) //skip the leading linebreak needed for legibility
            .TrimEnd();

        actual.TrimEnd().Should().Be(expected);
    }
}
#pragma warning restore IDE0027 // Simplify nested expression
