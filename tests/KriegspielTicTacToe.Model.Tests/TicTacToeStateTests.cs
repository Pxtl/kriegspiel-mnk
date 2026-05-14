namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using Xunit;

public class TicTacToeStateTests {
    [Fact]
    public void Constructor_EmptyBoards() {
        var state = new TicTacToeState(
            ['X', 'O'],
            false,
            []
        );
        state.Boards.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithBoardsCreatesProperState() {
        var state = new TicTacToeState(
            ['X', 'O'],
            false,
            [new BoardBuilder(3, 3), new BoardBuilder(3, 3)]
        );
        state.Boards.Count.Should().Be(2);
    }

    [Fact]
    public void CurrentTurnPlayer_IndexZero() {
        var state = new TicTacToeState(
            ['X', 'O'],
            false,
            [new BoardBuilder(3, 3)]
        );
        state.CurrentTurnPlayerIndex.Should().Be(0);
        state.CurrentTurnPlayer.Should().Be('X');
    }

    [Fact]
    public void NextTurn_AdvancesTurn() {
        var state = new TicTacToeState(
            ['X', 'O'],
            false,
            [new BoardBuilder(3, 3)]
        );
        
        state.NextTurn();
        state.CurrentTurnPlayerIndex.Should().Be(1);
        state.CurrentTurnPlayer.Should().Be('O');
    }

    [Fact]
    public void ActivePlayers_ShowsAll() {
        var state = new TicTacToeState(
            ['X', 'O'],
            false,
            [new BoardBuilder(3, 3)]
        );
        
        state.ActivePlayers.Should().ContainInOrder('X', 'O');
    }

    [Fact]
    public void ScoreCard_CalculatesWinningBoard_XWins() {
        var board = new Board(3, 3);
        
        board.Spaces[0, 0].MarkChar = 'X';
        board.Spaces[0, 1].MarkChar = 'X';
        board.Spaces[0, 2].MarkChar = 'X';
        
        board.ScoreCard.HighestScore.Should().NotBeNull();
    }

    [Fact]
    public void ScoreCard_CalculatesWinningBoard_OWins() {
        var board = new Board(3, 3);
        
        board.Spaces[0, 0].MarkChar = 'O';
        board.Spaces[1, 0].MarkChar = 'O';
        board.Spaces[2, 0].MarkChar = 'O';
        
        board.ScoreCard.HighestScore.Should().NotBeNull();
    }

    [Fact]
    public void Board_MarksToPlayer_MakesKnown() {
        var board = new Board(3, 3);
        board.Spaces[0, 0].MarkChar = 'X';
        
        board.Spaces[0, 0].MakeKnownToPlayer('X');
        
        board.Spaces[0, 0].KnownToPlayersSet.Should().Contain('X');
    }

    [Fact]
    public void Board_MarksToAnotherPlayer_MakeKnown() {
        var board = new Board(3, 3);
        board.Spaces[0, 0].MarkChar = 'X';
        
        board.Spaces[0, 0].MakeKnownToPlayer('O');
        
        board.Spaces[0, 0].KnownToPlayersSet.Should().Contain('O');
    }
}