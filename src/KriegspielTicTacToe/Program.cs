namespace KriegspielTicTacToe;

using System.CommandLine;

/// <summary>
/// Application entry point.
/// </summary>
class Program {
    public static int Main(string[] args) {
        var rootCommand = new RootCommand("This is a simple command-line implementation of Zach Weinersmith's proposed game 'Kriegspiel Tic Tac Toe'"){
            Options.StateFileOption,
            Options.ForceNewGameOption,
            Options.PlayersOption,
            Options.RandomOption,
            Options.SizeOption,
            Options.BoardsNumberOption,
            Options.JoinAsPlayerOption,
            Options.SynchronousModeOption
        };

        rootCommand.SetAction((parseResult) => {
                var file = parseResult.GetValue(Options.StateFileOption)!;  // has non-null default value.
                var doForceNewGame = parseResult.GetValue(Options.ForceNewGameOption);
                var players = parseResult.GetValue(Options.PlayersOption)!; // has non-null default value.
                var size = parseResult.GetValue(Options.SizeOption);
                var boardsNumber = parseResult.GetValue(Options.BoardsNumberOption);
                var joinAsPlayer = parseResult.GetValue(Options.JoinAsPlayerOption);

                var isRandomPlayer = parseResult.GetValue(Options.RandomOption);
                var isSynchronousMode = parseResult.GetValue(Options.SynchronousModeOption);

                var boardBuilders = new Model.BoardBuilder[boardsNumber!.Value];

                for(var i = 0; i < boardsNumber!; i+=1) {
                    boardBuilders[i] = new Model.BoardBuilder(size!.Value, size!.Value);
                }
                GameLogic.RunGame (
                    file,
                    doForceNewGame,
                    players.Select(p => p.Single()).ToArray(),
                    boardBuilders,
                    (joinAsPlayer ?? "").Cast<char?>().SingleOrDefault(),
                    isRandomPlayerOrder: isRandomPlayer,
                    isSynchronousMode: isSynchronousMode
                );
            }
        );

        return rootCommand.Parse(
            args, 
            new ParserConfiguration() {EnablePosixBundling = true}
        )
            .Invoke();
    }
}
