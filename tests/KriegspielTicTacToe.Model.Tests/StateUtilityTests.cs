namespace KriegspielTicTacToe.Model.Tests;

public class StateUtilityTests {
    [Fact]
    public void SerializeAndDeserialize_Board() {
        var expectedBoard = new Board(4, 4, new TicTacToeRuleset(3, true));
        var boardString = StateStorage.StateToString(expectedBoard);
        var actualBoard = StateStorage.StringToState<Board>(boardString);
        actualBoard.Should().BeEquivalentTo(expectedBoard);
    }

    [Fact]
    public void SerializeAndDeserialize_BlankGameState() {
        var boardBuilder3x3 = TicTacToeRuleset.CreateBoardBuilder(3, 3);
        IGameState expectedState = new GameState<TicTacToePlayAction>(
            new char[] { 'X', 'O' }.ToPlayersArray(),
            new TicTacToeTemplate([boardBuilder3x3, boardBuilder3x3, boardBuilder3x3], isSynchronousMode: false),
            isRandomPlayerOrder: false
        );
        var stateString = StateStorage.StateToString(expectedState);
        var actualState = StateStorage.StringToState<IGameState>(stateString);
        actualState.Should().BeOfType(typeof(GameState<TicTacToePlayAction>));
        actualState.Should().BeEquivalentTo(expectedState);
    }

    [Fact]
    public void SerializeAndDeserialize_SynchronousGameState() {
        var boardBuilder3x3 = TicTacToeRuleset.CreateBoardBuilder(3, 3);
        var players = new char[] { 'X', 'O' }.ToPlayersArray();
        var playerX = players[0];
        var playerO = players[1];
        var expectedState = new GameState<TicTacToePlayAction>(
            players,
            new TicTacToeTemplate([boardBuilder3x3, boardBuilder3x3, boardBuilder3x3], isSynchronousMode: true),
            isRandomPlayerOrder: false
        );

        //round 1 (collision)
        expectedState.Enqueue(new TicTacToePlayAction(0, 1, 1, playerX));
        expectedState.PlayManager.EndTurn(playerX, out _);
        expectedState.Enqueue(new TicTacToePlayAction(0, 1, 1, playerO));
        expectedState.PlayManager.EndTurn(playerO, out _);
        expectedState.PlayManager.EndRound(out _);

        //round 2 (2 separate moves)
        expectedState.Enqueue(new TicTacToePlayAction(0, 0, 0, playerX));
        expectedState.PlayManager.EndTurn(playerX, out _);
        expectedState.Enqueue(new TicTacToePlayAction(0, 2, 2, playerO));
        expectedState.PlayManager.EndTurn(playerO, out _);
        expectedState.PlayManager.EndRound(out _);

        //round 3 (player O discovers player X)
        expectedState.Enqueue(new TicTacToePlayAction(0, 2, 0, playerX));
        expectedState.PlayManager.EndTurn(playerX, out _);
        expectedState.Enqueue(new TicTacToePlayAction(0, 0, 0, playerO));
        expectedState.PlayManager.EndTurn(playerO, out _);
        expectedState.PlayManager.EndRound(out _);

        //round 4 (incomplete)
        expectedState.Enqueue(new TicTacToePlayAction(0, 1, 0, playerX));
        expectedState.PlayManager.EndTurn(playerX, out _);

        IGameState untypedExpectedState = expectedState;

        var stateString = StateStorage.StateToString<IGameState>(expectedState);
        var untypedActualState = StateStorage.StringToState<IGameState>(stateString);

        untypedActualState.Should().BeOfType(typeof(GameState<TicTacToePlayAction>));
        var actualState = (GameState<TicTacToePlayAction>)untypedActualState;

        untypedActualState.Boards.Should().BeEquivalentTo(untypedExpectedState.Boards);
        untypedActualState.PlayManager.Should().BeEquivalentTo(untypedExpectedState.PlayManager);
        actualState.ActionQueue.Should().BeEquivalentTo(expectedState.ActionQueue);
        actualState.Should().BeEquivalentTo(expectedState);
    }
}