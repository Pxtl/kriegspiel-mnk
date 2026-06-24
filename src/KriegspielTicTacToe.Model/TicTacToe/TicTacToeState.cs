// using OneOf;
// using OneOf.Types;

// namespace KriegspielTicTacToe.Model.TicTacToe;

// public record TicTacToeState : GameState<TicTacToeState, TicTacToeTemplate, TicTacToePlayAction> {
//     [Obsolete]
//     public TicTacToeState() : base() { }

//     public TicTacToeState(
//         Player[] players,
//         TicTacToeTemplate gameTemplate,
//         bool isRandomPlayerOrder
//     ) : base(players, gameTemplate, isRandomPlayerOrder) { }

//     public OneOf<NotFound, ActionQueuedSuccessfully, Result<Player>, AlreadyPlayed> PlaySpace(
//         sbyte boardIndex,
//         string spaceName,
//         Player player
//     ) => Enqueue(new TicTacToePlayAction(boardIndex, int.Parse(spaceName), player));
        
//     private OneOf<NotFound, ActionQueuedSuccessfully, Result<Player>, AlreadyPlayed> PlaySpace(
//         sbyte boardIndex,
//         int spaceNameAsInt,
//         Player player
//     ) {
//         if (spaceNameAsInt <= 0) {
//             return new NotFound();
//         }
//         var board = Boards[boardIndex];
//         if (board.TryGetCoordinatesFromSpaceNameAsInt(spaceNameAsInt, out var col, out var row)) {
//             return Enqueue(new TicTacToePlayAction(boardIndex, col, row, player));
//         }
//         return new NotFound();
//     }

//     public OneOf<NotFound, ActionQueuedSuccessfully, Result<Player>, AlreadyPlayed> PlaySpace(
//         sbyte boardIndex,
//         sbyte col,
//         sbyte row,
//         Player player
//     ) {
//         if (!PlayManager.CanTakeTurn(player)) {
//             throw new InvalidOperationException($"Player '{player}' cannot take their turn.");
//         }
        
//         var space = Boards[boardIndex].Spaces[col, row];
//         if (space.IsKnownToPlayer(player)) {
//             return new AlreadyPlayed();
//         } else if (space.Mark != null) {
//             space.MakeKnownToPlayer(player);
//             return new Result<Player>(space.Mark);
//         } else {
//             PlayActionBuffer.Add(new TicTacToePlayAction(boardIndex, col, row, player));
//             return new ActionQueuedSuccessfully();
//         }
//     }

// }
// public struct AlreadyPlayed;
