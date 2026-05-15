namespace KriegspielTicTacToe;

using KriegspielTicTacToe.Model;
using System.IO;

/// <summary>
/// Game logic implementation.
/// </summary>
internal static class GameLogic {
    public static void RunGame(
        FileInfo sharedStateFilePath,
        bool doForceNewGame,
        char[] players,
        IEnumerable<BoardBuilder> boardBuilders,
        char? joinAsPlayer,
        bool isRandomPlayerOrder,
        bool isSynchronousMode) 
    {
        TicTacToeState state;
        if(sharedStateFilePath.Exists && !doForceNewGame) {
            state = StateUtility.LoadState(sharedStateFilePath.FullName);
            Console.Out.WriteLine($"Loaded saved game!");
        } else {
            state = new TicTacToeState(players, boardBuilders, 
                isRandomPlayerOrder: isRandomPlayerOrder,
                isSynchronousMode: isSynchronousMode
            );
            Console.Out.WriteLine("Starting new game!");
            StateUtility.SaveState(state, sharedStateFilePath.FullName);
        }
        var playManager = state.PlayManager;

        if(joinAsPlayer.HasValue) {
            Console.Out.WriteLine($"Joining game-file '{sharedStateFilePath.FullName}' as player '{joinAsPlayer.Value}'.");
        } else {
            Console.Out.WriteLine("Running in hotseat mode.");
        }
        
        Console.Out.WriteLine($"Players are {string.Join(", ", playManager.Players)}.");
        Console.Out.WriteLine(isSynchronousMode 
            ? "Synchronous mode: all moves in a round are buffered until end of round." 
            : ""
        );
        
        bool isDone = false;
        var doNextTurn = true;
        while(!isDone) {
            if(doNextTurn) {
                if(joinAsPlayer.HasValue) {
                    if(!playManager.Players.Contains(joinAsPlayer.Value)) {
                        throw new ApplicationException($"Invalid player join, player {joinAsPlayer.Value} is not a player in this game.");
                    }
                    bool isDoneWaiting = false;
                    Console.Out.Write("Waiting for your turn.");

                    while(!isDoneWaiting) {
                        state = StateUtility.LoadState(sharedStateFilePath.FullName);
                        if(joinAsPlayer.Value == playManager.CurrentTurnPlayer || state.IsGameOver) {
                            isDoneWaiting = true;
                        } else {
                            Console.Out.Write(".");
                            Thread.Sleep(100);
                        }
                    }
                    Console.Out.WriteLine();
                    if(state.IsGameOver) {
                        isDone = true;
                    } else {
                        Console.Out.WriteLine($"Player {joinAsPlayer.Value} it is your turn.");
                    }
                } else {
                    Console.Out.WriteLine($"Player {playManager.CurrentTurnPlayer} ready? Press any key to continue...");
                    Console.ReadKey(intercept: true);
                    Console.WriteLine();
                }
            }
            var playerToRender = playManager.CurrentTurnPlayer;
            
            if (!isDone) {
                var activeBoardIndex = state.SingleActiveBoardIndex;
                if (!activeBoardIndex.HasValue) {
                    BoardRenderer.DrawBoards(state, playerToRender, activeBoardIndex);
                    var boardCommand = InputUtility.ReadCommandKeys("Press numeric key(s) to pick a board, or 'r' to resign.", 1);
                    boardCommand.Switch (
                        charCode => {
                            doNextTurn = true;
                            playManager.ResignPlayer(playManager.CurrentTurnPlayer);
                        },
                        boardCode => state.SelectBoard(boardCode).Switch (
                            notFound => {
                                doNextTurn = false;
                                Console.WriteLine($"That is not a valid board.  Please pick an incomplete board.");
                            },
                            boardIsDone => {
                                doNextTurn = false;
                                Console.WriteLine($"That board is already complete.");
                            },
                            result => {
                                activeBoardIndex = result.Value;
                            }
                        ),
                        unknown => {
                            doNextTurn = false;
                        }
                    );
                }

                if(activeBoardIndex.HasValue) {
                    var boardIndex = activeBoardIndex.Value;
                    BoardRenderer.DrawBoards(state, playerToRender, boardIndex);
                    var spaceCommand = InputUtility.ReadCommandKeys("Press numeric key(s) to play a space, or 'r' to resign.", state.Boards[boardIndex].SpaceIndexCodeLength);
                    spaceCommand.Switch (
                        charCode => {
                            doNextTurn = true;
                            playManager.ResignPlayer(playManager.CurrentTurnPlayer);
                        },
                        spaceCode => {
                            state.PlaySpace(boardIndex, spaceCode).Switch(
                                success => {
                                    doNextTurn = true;
                                    Console.WriteLine($"Played on board {boardIndex + 1}, space {spaceCode}.");
                                }, result => {
                                    doNextTurn = true;
                                    Console.WriteLine($"Space already taken by player '{result.Value}'.");
                                }, alreadyPlayed => {
                                    doNextTurn = false;
                                    Console.Out.WriteLine($"Invalid square, that square is already known to player {playManager.CurrentTurnPlayer}");
                                }, notFound => {
                                    doNextTurn = false;
                                    Console.WriteLine("Invalid space.");
                                }
                            );
                        },
                        unknown => {
                            doNextTurn = false;
                        }
                    );
                }
            }
            
            if (doNextTurn) {
                if (!playManager.IsResignedPlayer(playerToRender)) {
                    playManager.NextTurn();
                }

                if (!joinAsPlayer.HasValue || playerToRender == joinAsPlayer.Value) {
                    StateUtility.SaveState(state, sharedStateFilePath.FullName);                  
                }

                BoardRenderer.DrawBoards(state, playerToRender, activeBoardIndex:null);
                
                if (!state.IsGameOver) {
                    if (!joinAsPlayer.HasValue) {
                        Console.Out.WriteLine($"Press any key to continue...");
                        Console.ReadKey(intercept: true);
                        Console.Clear();
                    }
                } else {
                    Console.Out.WriteLine(state.GameStateText);
                    isDone = true;
                }
            }
        }
        
        Console.Out.WriteLine("Game over.");
        Thread.Sleep(1000);
        sharedStateFilePath.Delete();
    }
}
