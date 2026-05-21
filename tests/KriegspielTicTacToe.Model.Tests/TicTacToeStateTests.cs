namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using Xunit;

public class TicTacToeStateTests {
    [Fact]
    public void Constructor_EmptyBoards() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        state.Boards.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithBoardsCreatesProperState() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3), new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        state.Boards.Count.Should().Be(2);
    }

    [Fact]
    public void CurrentTurnPlayer_IndexZero() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        state.PlayManager.CurrentTurnPlayerIndex.Should().Be(0);
        state.PlayManager.CurrentTurnPlayer.Should().Be('X');
    }

    [Fact]
    public void NextTurn_AdvancesTurn() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        
        state.PlayManager.NextTurn();
        state.PlayManager.CurrentTurnPlayerIndex.Should().Be(1);
        state.PlayManager.CurrentTurnPlayer.Should().Be('O');
    }

    [Fact]
    public void ActivePlayers_ShowsAll() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        
        state.PlayManager.ActivePlayers.Should().ContainInOrder('X', 'O');
    }

    [Fact]
    public void RoundComplete_RequiresTwoTurns() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        
        state.PlayManager.RoundIndex.Should().Be(0);
        state.PlayManager.NumberOfActivePlayers.Should().Be(2);
        state.PlayManager.IsRoundComplete.Should().BeFalse();
        
        state.PlayManager.NextTurn();
        state.PlayManager.RoundIndex.Should().Be(0);

        state.PlayManager.NextTurn();
        state.PlayManager.RoundIndex.Should().Be(1);
        state.PlayManager.IsRoundComplete.Should().BeTrue();
    }

    [Fact]
    public void RoundComplete_OnePlayerResigned() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        state.PlayManager.ResignPlayer('X');
        
        state.PlayManager.NumberOfActivePlayers.Should().Be(1);
              
        state.PlayManager.CurrentTurnPlayerIndex.Should().Be(0);
        state.PlayManager.RoundIndex.Should().Be(0);
        state.PlayManager.IsRoundComplete.Should().BeFalse();

        state.PlayManager.NextTurn();

        state.PlayManager.CurrentTurnPlayerIndex.Should().Be(0);
        state.PlayManager.RoundIndex.Should().Be(1);
        state.PlayManager.IsRoundComplete.Should().BeTrue();
    }

    [Fact]
    public void RoundComplete_ThreePlayers() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        
        state.PlayManager.NumberOfActivePlayers.Should().Be(3);
        
        state.PlayManager.NextTurn();
        state.PlayManager.CurrentTurnPlayerIndex.Should().Be(1);
        state.PlayManager.IsRoundComplete.Should().BeFalse();
        
        state.PlayManager.NextTurn();
        state.PlayManager.CurrentTurnPlayerIndex.Should().Be(2);
        state.PlayManager.IsRoundComplete.Should().BeFalse();
        
        state.PlayManager.NextTurn();
        state.PlayManager.CurrentTurnPlayerIndex.Should().Be(0);
        state.PlayManager.RoundIndex.Should().Be(1);
        state.PlayManager.IsRoundComplete.Should().BeTrue();
    }

    [Fact]
    public void Constructor_3Players_CorrectActiveCount() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        
        state.PlayManager.CurrentTurnPlayer.Should().Be('A');
        state.PlayManager.CurrentTurnPlayerIndex.Should().Be(0);
    }

    [Fact]
    public void Constructor_RandomPlayer_Shuffles() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: true,
            isSynchronousMode: false
        );
        // TODO: This test should loop enough times to show that all outcomes are
        // possible, to the point that failing the test is statistically
        var firstPlayer = state.PlayManager.CurrentTurnPlayer;
        var expectedPlayers = new[] { 'A', 'B', 'C' };
        expectedPlayers.Contains(firstPlayer).Should().BeTrue();
    }

    [Fact]
    public void RefreshCurrentPlayerTurnIndex_ResignsPlayer_Wraps() {
        var state = new TicTacToeState(
            ['A', 'B', 'C'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        
        state.PlayManager.ResignPlayer('A');
        state.PlayManager.CurrentTurnPlayer.Should().Be('B');
        
        state.PlayManager.ResignPlayer('B');
        state.PlayManager.CurrentTurnPlayer.Should().Be('C');
    }
}
