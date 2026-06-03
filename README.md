# kriegspiel-tictactoe

A simple command-line implementation of Zach Weinersmith's proposed game 'Kriegspiel Tic Tac Toe'.

see https://mastodon.social/@ZachWeinersmith/111890121393299096

Basically, the idea is that it's blind tic-tac-toe where you can only see your
opponent's spaces if you hit the same space.

I (Pxtl) got carried away gold-plating, so there are 3 notable features:

1. Arbitrary square board size.
2. Arbitrary player count. Players must be unique single-character names.
3. Both hotseat *and* file-based network multiplayer.

# Install and Run

This program is implemented in C# using dotnet core 10.

to install dotnet core 10 SDK on Windows:

> `winget install dotnet-sdk-10`

but possibly dotnet runtime 10 will be sufficient.

> `winget install dotnet-runtime-10`

to run it in basic gameplay (traditional tic-tac-toe but Kriegspiel, hotseat
mode).

> `dotnet run KriegspielTicTacToe`

to see other options for play, run

> `dotnet run KriegspielTicTacToe --?`

note that dotnet-script will intercept all other variations like `-h` or
`--help` or `-?`, WIP.  Alternately you can supply any garbage like 

> `dotnet run KriegspielTicTacToe --justshowmethehelp`

and it will insult you but show you the help.

```
Description:
  This is a simple command-line implementation of Zach Weinersmith's proposed game 'Kriegspiel Tic Tac Toe'

Usage:
  KriegspielTicTacToe [options]

Options:
  -f, --file <file>        Path to the json file where gamestate is stored.  Will be resumed automatically if you kill
                           the game (ctrl-C).  Use a fileshare for network 
                           multiplayer. [default: /home/pxtl/.config/KriegspielTicTacToe.json]
  -F, --force              Force a new game instead of loading the game at the gamestate file.  Will replace gamestate 
                           file.
  -p, --players <players>  Players mark characters.  Provide them space-separated, eg '-p A B C X Y Z' for a 6-player
                           game. [default: X|O]
  -r, --random             Randomize player order.
  -z, --size <size>        Board size.  Default is 3x3. [default: 3]
  -b, --boards <boards>    Number of boards. [default: 3]
  -j, --join <join>        Join as given player char mark. Must match a mark in players list. Hotseat mode if not
                           provided.
  -y, --synchronous        Moves do not execute until all players in a round have taken a turn.  If two players move to
                           the same square, that square becomes an impasse marker visible to all.
  -?, -h, --help           Show help and usage information
  --version                Show version information
```

So, to start a simple 3-player hotseat game between Alice, Bob, and Carol on a 4x4 screen, the command would be

> `dotnet run KriegspielTicTacToe -p A B C --size 4`

Conversely, to start a multiplayer game with the default rules (2 players X and
O on a 3x3 board) on a fileshare named `\\kosmos\storage` with random
start-player, the command would be 

> `dotnet run KriegspielTicTacToe -f \\kosmos\storage\temp\ksttt.json -j X -r`

And then your friend (on another computer with similar access to `\\kosmos\storage\`) can join with 

> `dotnet run KriegspielTicTacToe -f \\kosmos\storage\temp\ksttt.json -j O`

any game-rule options you pass in when joining an existing game like size,
players, or randomization will be ignored.

# Synchronous Mode

Use the `--synchronous` flag to enable synchronous mode.

In synchronous mode:
- All moves in a round are buffered until every player in the round takes their turn
- If two players move to the same square in the same round, it becomes an impasse marker (I)
- Impasse markers are visible to all players, cannot be used to win, and block all players from that square

To enable synchronous mode:

```bash
# Start a synchronous 2-player game
dotnet run KriegspielTicTacToe --synchronous

# Start a synchronous 3-player game
dotnet run KriegspielTicTacToe --synchronous -p A B C
```

See [Synchronous Mode Usage](doc/sync-mode-usage.md)

# Contributing

See [CONTRIB.md](CONTRIB.md)

# Bugs

## If I do something crazy like have 3 players all running as player X, or join an online game in hotseat mode, the game is broken and bad

Don't do that.

## Other Bugs

Infinite.  I have no idea.  Half of this was a one-evening hackathon.  The other half was experimenting with a pretty weak local-hosted AI.
