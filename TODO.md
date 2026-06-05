# TODO

## Automated Testing

### ModelToCommandNameUtility
Unit tests for ModelToCommandNameUtility.cs

## Game Type Files

### New GameType Model Object
- Add a new Constructor for TicTacToeState that wraps the following constructor:
    ```
    public TicTacToeState(
        Player[] players,
        IEnumerable<BoardBuilder> boardBuilders,
        bool isRandomPlayerOrder,
        bool isSynchronousMode
    ) {...}
    ```
    with
    ```
    public TicTacToeState(
        Player[] players,
        GameType gameType,
        bool isRandomPlayerOrder
    ) {...}
    ```
- Refactor other constructors by moving their components that are part of
  GameType into the GameType constructors.
- Remove the other TicTacToeState constructors so that all calls to
  TicTacToeState constructor use the constructors
    ```
    public TicTacToeState(
        Player[] players,
        GameType gameType,
        bool isRandomPlayerOrder
    ) {...}
    ```
