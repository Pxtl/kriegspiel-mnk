# Synchronous Mode Usage

## Overview

Kriegspiel Tic-Tac-Toe now supports a **synchronous mode** option. In synchronous mode, all players in a round have the opportunity to move, and their moves only become permanent after all players in that round have taken their turn.

## Usage

### Starting a Synchronous Game

```bash
# Using the synchronous flag
dotnet run KriegspielTicTacToe --synchronous -p X O --size 3

# Or using shorthand
dotnet run KriegspielTicTacToe -y -p X O
```

### Starting an Asynchronous (Default) Game

```bash
dotnet run KriegspielTicTacToe -p X O
```

## Behavior

### Synchronous Mode

1. **Buffering**: Moves made by each player in a round are buffered until all players have moved.
2. **Impasse Markers**: If two players move to the same square in the same round, that square becomes an **impasse marker (█)** visible to all players.
3. **Impasse Effects**:
   - The impasse marker cannot be used to complete a winning row
   - All players are blocked from playing that square
   - The marker is visible to all players regardless of whether they've moved there

### Asynchronous Mode (Default)

1. **Immediate Moves**: Moves happen immediately when played.
2. **No Impasses**: Since moves happen immediately, there's no opportunity for conflicts.

## Round Completion

In synchronous mode:
- Each round starts with the first active player
- Players take turns until all active players have moved
- Once the last player has moved, the round completes
- All buffered moves are then finalized
- If conflicts occurred, impasse markers are applied
- The next round begins

## Example Play Session

```
Players are X, O.
Running in synchronous mode.

Board:
---
  
. . .
. . .
. . .

Round in progress. Waiting for other players to move...
Current turn player: X
Press any key when you're ready for next player...

Board after X moves:
---
  
. . .
. . .
X . .

Round in progress. Waiting for other players to move...
Current turn player: O
Press any key when you're ready for next player...

Round complete.
```

## Impasse Handling

When a conflict occurs:

```
Board showing impasse:
---
  
. . .
. . . 
█ . . <-- Impasse marker '█'
```

The impasse marker '█' at [col=1, row=1] means:
- Both X and O moved there this round
- No one "wins" that space
- The space is blocked for future play
- The marker is visible to all players

## Options

### `--synchronous` or `-s`

Enable synchronous mode. When combined with multiple players, this allows all players to make their moves before they become permanent.

## Notes

- Synchronous mode is particularly useful for network multiplayer scenarios
- Saved game files will work with both modes (round data is tracked automatically)