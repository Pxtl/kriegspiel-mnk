using OneOf;
using OneOf.Types;
using KriegspielTicTacToe.Model.Template;

namespace KriegspielTicTacToe;

/// <summary>
/// Game logic implementation.
/// </summary>
internal static class ConsoleLoop {
    public static void RunGame(
        FileInfo sharedStateFilePath,
        GameState<MNKPlayAction> state,
        OneOf<Player, LocalHotseatGame> joinAsPlayer
    ) {
        StateStorage.SaveState(state, sharedStateFilePath.FullName);

        Console.Out.WriteLine(joinAsPlayer.Match(
            player => $"Joining game-file '{sharedStateFilePath.FullName}' as player '{player}'.",
            localHotseatGame => "Running in hotseat mode."
        ));
        
        Console.Out.WriteLine($"Players are {string.Join(", ", state.PlayManager.Players)}.");

        bool isGameOver = false;
        while (!isGameOver) {
            var currentPlayerChosen = joinAsPlayer.Match(
                player => {
                    if (!state.PlayManager.Players.Contains(player)) {
                        throw new ApplicationException($"Invalid player join, player {player} is not a player in this game.");
                    }
                    bool isDoneWaiting = false;
                    Console.Out.Write("Waiting for your turn.");

                    while (!isDoneWaiting) {
                        state = StateStorage.LoadState(sharedStateFilePath.FullName);
                        if (state.PlayManager.PlayersAvailableForTurn.Contains(player)
                            ||
                            state.IsGameOver
                        ) {
                            isDoneWaiting = true;
                        } else {
                            Console.Out.Write(".");
                            Thread.Sleep(100);
                        }
                    }
                    Console.Out.WriteLine();
                    return state.IsGameOver
                        ? OneOf<Result<Player>, GameIsOver>.FromT1(new GameIsOver())
                        : new Result<Player>(player);
                },
                localHotseatGame => DoPlayerChooserLoop(state.PlayManager)
            );

            currentPlayerChosen.Switch(
                playerResult => {
                    var currentPlayer = playerResult.Value;
                    var currentPlayerIsDoneTurn = false;
                    currentPlayerIsDoneTurn = DoPlayerTurnLoop(state, currentPlayer, sharedStateFilePath.FullName);

                    if (currentPlayerIsDoneTurn) {
                        var hasStateChanged = false;
                        using (var stateStorage = new StateStorage(sharedStateFilePath.FullName)) {
                            state = stateStorage.State;
                            var gameView = state.GetView(currentPlayer);
                            gameView.EndTurn(out hasStateChanged);
                        }

                        if (hasStateChanged) {
                            var gameView = state.GetView(currentPlayer);
                            Console.Out.WriteLine(
                                BoardRenderer.DrawBoards(gameView, activeBoardIndex: null, maxRenderWidth: Console.BufferWidth)
                            );
                        }

                        if (state.PlayManager.IsRoundOver) {
                            var hasRoundStateChanged = false;
                            using (var stateStorage = new StateStorage(sharedStateFilePath.FullName)) {
                                state = stateStorage.State;
                                state.PlayManager.EndRound(out hasRoundStateChanged);
                            }
                            if (hasRoundStateChanged) {
                                InputUtility.PauseAndPressAnyKey();
                                Console.Clear();
                                Console.Out.WriteLine(state.GameStateText);
                                Console.Out.WriteLine("Executing synchronous moves.");
                            }
                        }

                        if (!state.IsGameOver) {
                            joinAsPlayer.Switch(
                                player => { },
                                localHotseatGame =>
                                {
                                    InputUtility.PauseAndPressAnyKey();
                                    Console.Clear();
                                }
                            );
                        } else {
                            Console.Out.WriteLine(state.GameStateText);
                            isGameOver = true;
                        }
                    }
                },
                gameIsOver => {
                    isGameOver = true;
                }
            );                        
        }        
        Thread.Sleep(1000);
        sharedStateFilePath.Delete();
    }

    private static bool DoPlayerTurnLoop(GameState<MNKPlayAction> state, Player currentPlayer, string sharedStateFilePath) {
        var currentPlayerIsDoneTurn = false;
        while (!currentPlayerIsDoneTurn)
        {
            Console.Out.WriteLine(state.GameStateText);
            Console.Out.WriteLine($"Player {currentPlayer.Mark}, take your turn.");
           
            var activeBoardIndex = state.SingleActiveBoardIndex;

            if (!activeBoardIndex.HasValue) {
                var gameView = state.GetView(currentPlayer);
                //player picks a board.
                Console.Out.WriteLine(
                    BoardRenderer.DrawBoards(gameView, activeBoardIndex, maxRenderWidth: Console.BufferWidth)
                );
                var availableBoardCommands = gameView.BoardNames;
                //TODO: Just make the 1st char of the command the board number.
                var boardCommand = InputUtility.ReadCommandInputWithAddedStandardPlayerCommands(
                    "Press numeric key(s) to pick a board, or 'r' to resign.",
                    availableBoardCommands
                );
                boardCommand.Switch(
                    result => {
                        if(result.Value == "r") {
                            currentPlayerIsDoneTurn = true;
                            using (var stateStorage = new StateStorage(sharedStateFilePath)) {
                                gameView = new GameView(stateStorage.State, currentPlayer);
                                gameView.ResignPlayer();
                            }
                        } else {
                            gameView.SelectBoard(result.Value).Switch(
                                notFound => {
                                    currentPlayerIsDoneTurn = false;
                                    Console.WriteLine($"That is not a valid board.  Please pick an incomplete board.");
                                },
                                boardIsDone => {
                                    currentPlayerIsDoneTurn = false;
                                    Console.WriteLine($"That board is already complete.");
                                },
                                boardIndex => {
                                    activeBoardIndex = boardIndex.Value;
                                }
                            );
                        }
                    },
                    unknown => {
                        currentPlayerIsDoneTurn = false;
                    }
                );
            }

            // activeBoardIndex will be null if the user has to retry or the
            // user is resigning. In both cases we skip asking them about
            // which space they wish to play.
            if (activeBoardIndex.HasValue) {
                var boardIndex = activeBoardIndex.Value;
                var boardCode = boardIndex + 1;
                var gameView = state.GetView(currentPlayer);
                Console.Out.WriteLine(
                    BoardRenderer.DrawBoards(gameView, boardIndex, maxRenderWidth: Console.BufferWidth)
                );
                var spaceCommand = InputUtility.ReadCommandInputWithAddedStandardPlayerCommands(
                    "Press numeric key(s) to play a space, or 'r' to resign.",
                    state.Boards[boardIndex].SpaceNames
                );
                spaceCommand.Switch(
                    result => {
                        using (var stateStorage = new StateStorage(sharedStateFilePath)) {
                            state = stateStorage.State;
                            gameView = state.GetView(currentPlayer);
                            if(result.Value == "r") {
                                currentPlayerIsDoneTurn = true;
                                gameView.ResignPlayer();
                            } else {
                                var playAction = MNKPlayAction.Create(state, boardIndex, result.Value, currentPlayer);
                                playAction.Attempt(state).Switch(
                                    isLegalToQueue => {
                                        state.Enqueue(playAction);
                                        currentPlayerIsDoneTurn = true;
                                        Console.WriteLine($"Played on board {boardCode}, space {result.Value}");
                                    }, 
                                    newlyLearned => {
                                        currentPlayerIsDoneTurn = true;
                                        Console.WriteLine($"Space already filled: '{newlyLearned.Mark}'.");
                                    },
                                    alreadyPlayed => {
                                        currentPlayerIsDoneTurn = false;
                                        Console.Out.WriteLine($"Invalid space, that space is already known to player {currentPlayer}");
                                    }
                                );
                            }
                        }
                    },
                    unknown => {
                        currentPlayerIsDoneTurn = false;
                    }
                );
            }
        }

        return currentPlayerIsDoneTurn;
    }

    internal static OneOf<Result<Player>, GameIsOver> DoPlayerChooserLoop(PlayManager playManager) {
        // Use ModelToKeyUtility for clean, testable key mapping
        var playerToCommand = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(playManager.PlayersAvailableForTurn);

        var commandToPlayer = playerToCommand
            .ToOrderedDictionary(
                p => p.Value,
                p => p.Key,
                StringComparer.OrdinalIgnoreCase
            );

        while (true) {
            if (playManager.PlayersAvailableForTurn.Count() == 1) {
                var currentPlayer = playManager.PlayersAvailableForTurn.Single();
                InputUtility.PauseAndPressAnyKey(prompt: $"Player {currentPlayer} ready?");
                Console.WriteLine();
                return new Result<Player>(currentPlayer);
            }
            if (!playManager.PlayersAvailableForTurn.Any()) {
                throw new InvalidOperationException("No players are available to take a turn.");
            }

            Console.Out.WriteLine(playManager.GameStateText);

            // Display all available players with alternate key hints for non-typeable marks
            var playerDisplayList = playManager.PlayersAvailableForTurn
                .Select(p => {
                    var altKey = playerToCommand[p];
                    var keyDisplay = altKey.Equals(p.Mark, StringComparison.OrdinalIgnoreCase)
                        ? ""
                        : $" ({altKey})";
                    return $"Player {p.Mark}{keyDisplay}";
                });

            var prompt = "Who will take the next turn? Press the player's key to take their turn (or press 'q' to quit the game for everyone)."
                + Environment.NewLine
                + string.Join(" ", playerDisplayList);
            var validCommands = ((IEnumerable<string>)["q"]).Concat(commandToPlayer.Keys);
            var commandResult = InputUtility.ReadCommandInputLoop(prompt, validCommands);

            if (commandResult == "q") {
                Console.Out.WriteLine("Quitting.");
                return new GameIsOver();
            } else {
                return new Result<Player>(commandToPlayer[commandResult]);
            }
        }
    }
}

public struct LocalHotseatGame { }

public struct GameIsOver { }
