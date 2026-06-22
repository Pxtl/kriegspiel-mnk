namespace KriegspielTicTacToe.Model.Tests;

public class TicTacToeStateTests {
    [Fact]
    public void Constructor_EmptyBoards() {
        var state = new TicTacToeState(
            (new[] { 'X', 'O' }).ToPlayersArray(),
            new GameType {
                BoardBuilders = Array.Empty<BoardBuilder>(),
                IsSynchronousMode = false
            },
            isRandomPlayerOrder: false
        );
        state.Boards.Should().BeEmpty();
        state.PlayManager.Players.Should().Contain(new Player("X")).And.Subject.Should().Contain(new Player("O"));
        state.PlayManager.ActivePlayers.Should().Contain(new Player("X")).And.Subject.Should().Contain(new Player("O"));
    }
}
