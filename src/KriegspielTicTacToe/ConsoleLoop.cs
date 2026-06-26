using OneOf;
using OneOf.Types;
using PxtlCa.SystemCollectionsExtensions;

namespace KriegspielTicTacToe;

/// <summary>
/// Game logic implementation.
/// </summary>
internal static class ConsoleLoop {
    public static void RunGame(
        FileInfo sharedStateFilePath,
        GameState state,
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

                    //wait loop.
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
                    DoPlayerTurnLoop(ref state, currentPlayer, sharedStateFilePath.FullName, out bool currentPlayerIsDoneTurn, out bool hasViewChanged);

                    if (currentPlayerIsDoneTurn) {
                        if (hasViewChanged) {
                            Console.Out.WriteLine(
                                BoardRenderer.DrawBoards(state.GetView(currentPlayer), maxRenderWidth: Console.BufferWidth)
                            );
                        }

                        //execute round-end stuff.
                        if (state.PlayManager.IsRoundOver) {
                            var hasRoundStateChanged = false;
                            using (var stateStorage = new StateStorage(sharedStateFilePath.FullName)) {
                                state = stateStorage.State;
                                state.PlayManager.EndRound(out hasRoundStateChanged);
                            }
                            if (hasRoundStateChanged) {
                                InputUtility.PauseAndPressAnyKey("Round over.");
                                Console.Clear();
                                Console.Out.WriteLine(state.GameStateText);
                                Console.Out.WriteLine("Executing synchronous moves.");
                            }
                        }
                        if (!state.IsGameOver) {
                            joinAsPlayer.Switch(
                                player => { },
                                localHotseatGame => {
                                    InputUtility.PauseAndPressAnyKey();
                                    Console.Clear();
                                }
                            );
                        } else {
                            Console.Out.WriteLine(state.GameStateText);
                            Console.Out.WriteLine(BoardRenderer.DrawBoards(state.GetView(null), maxRenderWidth: Console.BufferWidth));
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

    private static void DoPlayerTurnLoop(ref GameState state, Player currentPlayer, string sharedStateFilePath, out bool currentPlayerIsDoneTurn, out bool isViewChanged) {
        IPlayActionResult? playActionResult = null;
        while (playActionResult == null || !playActionResult.IsTurnDone) {
            Console.Out.WriteLine(state.GameStateText);
            Console.Out.WriteLine($"Player {currentPlayer.Mark}, take your turn.");

            var gameView = state.GetView(currentPlayer);
            Console.Out.WriteLine(
                BoardRenderer.DrawBoards(gameView, maxRenderWidth: Console.BufferWidth)
            );
            var spaceCommand = InputUtility.ReadCommandInputWithAddedStandardPlayerCommands(
                "Press numeric key(s) to play a space, or 'r' to resign, or 'q' to save game and quit.",
                gameView.SpaceNames
            );
            using (var stateStorage = new StateStorage(sharedStateFilePath)) {
                state = stateStorage.State;
                spaceCommand.Switch(
                    result => {
                        gameView = stateStorage.State.GetView(currentPlayer);
                        if("r".Equals(result.Value, StringComparison.OrdinalIgnoreCase)) {
                            playActionResult = gameView.ResignPlayer();
                        } else if ("q".Equals(result.Value, StringComparison.OrdinalIgnoreCase)) {
                            Quit();
                        } else {
                            var playerAction = MNKAction.Create(stateStorage.State, result.Value);
                            playActionResult = gameView.Attempt(playerAction);
                        }
                    },
                    unknown => {
                        playActionResult = null;
                    }
                );
            }
        }
        isViewChanged = playActionResult.IsViewChanged;
        currentPlayerIsDoneTurn = playActionResult.IsTurnDone;
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

            if ("q".Equals(commandResult, StringComparison.OrdinalIgnoreCase)) {
                Quit();
            } else {
                return new Result<Player>(commandToPlayer[commandResult]);
            }
        }
    }

    internal static void DoBoardSelectorLoop(string sharedStateFilePath, GameState state, Player currentPlayer, out sbyte boardIndex) {
        if (state.SingleActiveBoardIndex.HasValue) {
            boardIndex = state.SingleActiveBoardIndex.Value;
        } else {
            BoardView? selectedBoard = null;
            while(selectedBoard == null) {
                var gameView = state.GetView(currentPlayer);
                //player picks a board.
                Console.Out.WriteLine(
                    BoardRenderer.DrawBoards(gameView, maxRenderWidth: Console.BufferWidth)
                );
                var availableBoardCommands = gameView.BoardNames;
                //TODO: Just make the 1st char of the command the board number.
                var boardCommand = InputUtility.ReadCommandInputWithAddedStandardPlayerCommands(
                    "Press numeric key(s) to pick a board, 'r' to resign, or 'q' to save game and quit.",
                    availableBoardCommands
                );
                boardCommand.Switch(
                    result => {
                        if("r".Equals(result.Value, StringComparison.OrdinalIgnoreCase)) {
                            using (var stateStorage = new StateStorage(sharedStateFilePath)) {
                                gameView = stateStorage.State.GetView(currentPlayer);
                                gameView.ResignPlayer();
                            }
                        } else if ("q".Equals(result.Value, StringComparison.OrdinalIgnoreCase)) {
                            Quit();
                        } else {
                            gameView.SelectBoard(result.Value).Switch(
                                notFound => {
                                    Console.WriteLine($"That is not a valid board.  Please pick an incomplete board.");
                                },
                                boardIsDone => {
                                    Console.WriteLine($"That board is already complete.");
                                },
                                boardViewResult => {
                                    selectedBoard = boardViewResult.Value;
                                }
                            );
                        }
                    },
                    unknown => {
                        selectedBoard = null;
                    }
                );
            }
            boardIndex = selectedBoard.BoardIndex;
        } 
    }

    private static void Quit() {
        Console.WriteLine("Quitting.  Use 'load' to resume later.");
        Environment.Exit(0);        
    }
}

public struct LocalHotseatGame { }

public struct GameIsOver { }
