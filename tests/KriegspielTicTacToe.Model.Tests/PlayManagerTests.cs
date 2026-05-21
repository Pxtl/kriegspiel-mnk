namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using Xunit;

public class PlayManagerTests {
    [Fact]
    public void Round_RoundIndexStartsAtZero() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        state.PlayManager.RoundIndex.Should().Be(0);
    }

    [Fact]
    public void EndTurn_AdvancesTurn() {
        var state = new TicTacToeState(
            ['X', 'O'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: false
        );
        state.PlayManager.EndTurn('X', out _);
        state.PlayManager.ActivePlayers.Should().ContainInOrder('O');
    }

    [Fact]
    public void RoundComplete_TwoPlayers() {
        var state = new TicTacToeState(
            ['A', 'B'],
            [new BoardBuilder(3, 3)],
            isRandomPlayerOrder: false,
            isSynchronousMode: true
        );
        state.PlayManager.EndTurn('A', out _);
        state.PlayManager.EndTurn('B', out _);
        state.PlayManager.IsRoundOver.Should().BeTrue();
        state.PlayManager.GameStateText.Should().Be("Synchronized play.");
    }
}
