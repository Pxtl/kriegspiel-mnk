namespace KriegspielTicTacToe;

using KriegspielTicTacToe.Model;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Draws the full board based on the given gamestate, from the perspective of
/// the given player.
/// </summary>
public static class BoardRenderer
{
    private static StringBuilder sb = new StringBuilder();

    public static string DrawBoards(TicTacToeState state, Player player, int? activeBoardIndex,
        int cursorX = 0, int maxWidth = int.MaxValue)
    {
        // default to console window width if int.MaxValue provided
        maxWidth = maxWidth == int.MaxValue ? Console.WindowWidth : maxWidth;

        bool doShowBoardCode = state.Boards.Count > 1;
        var maxRowCount = state.Boards.Max(b => b.RowCount);
        var sb = new StringBuilder();

        var nextDrawnBoardIndex = 0;
        var boardWidth = GetBoardWidth(state.Boards[nextDrawnBoardIndex]);
        var totalRowWidth = cursorX + boardWidth;
        
        while (nextDrawnBoardIndex < state.Boards.Count)
        {
            //wrap check for top border
            if(cursorX > 0 && totalRowWidth > maxWidth)
            {
                cursorX = 0;
                totalRowWidth = boardWidth;
            }
            DrawBorderRow(state, nextDrawnBoardIndex, "┌", "┬", "┐", "───", doShowBoardCode, maxWidth, ref cursorX, sb);
            
            for(var row = 0; row < maxRowCount; row += 1)
            {
                if(row > 0)
                {
                    DrawBorderRow(state, nextDrawnBoardIndex, "├", "┼", "┤", "───", false, maxWidth, ref cursorX, sb);
                }
                DrawBoardSpacesRow(state, nextDrawnBoardIndex, player, "│", activeBoardIndex, row, boardWidth, maxWidth, ref cursorX, sb);
            }
            nextDrawnBoardIndex = DrawBorderRow(state, nextDrawnBoardIndex, "└", "┴", "┘", "───", false, maxWidth, ref cursorX, sb);
            cursorX += 1; //newline counted as one char
        }

        sb.AppendLine();
        return sb.ToString();
    }

    public static int GetBoardWidth(Board board)
        => board.ColumnCount * 4 + 3;

    /// <summary>
    /// Helper function to draw a border row of the board.
    /// Wraps to newline when maxWidth is exceeded.
    /// </summary>
    private static int DrawBorderRow(
        TicTacToeState state,
        int startBoardIndex,
        string startBarString, 
        string midBarString, 
        string endBarString, 
        string spanString, 
        bool showBoardCode,
        int maxWidth,
        ref int cursorX,
        StringBuilder sb)
    {
        for (var boardIndex = startBoardIndex; boardIndex < state.Boards.Count; boardIndex += 1)
        {
            var board = state.Boards[boardIndex];
            
            //wrap check - break if cursor would exceed maxWidth
            if(cursorX > 0 && (cursorX + GetBoardWidth(board) > maxWidth))
            {
                cursorX = 0;
                break;
            }

            sb.Append(showBoardCode
                ? (board.IsDone
                    ? " ✓" //board is done so just show a checkmark.
                    : $" {boardIndex + 1}" //key-index to choose it
                ) : "  " //blank space
            );

            sb.Append($"{startBarString}{spanString}");
            
            for(var col = 0; col < board.ColumnCount - 1; col += 1)
            {
                sb.Append($"{midBarString}{spanString}");
            }
            sb.Append(endBarString);
            cursorX += GetBoardWidth(board);
        }
        sb.AppendLine();
        return state.Boards.Count;
    }

    /// <summary>
    /// Draw a row of board spaces with window wrapping.
    /// Wraps to newline when maxWidth is exceeded.
    /// </summary>
    private static int DrawBoardSpacesRow(
        TicTacToeState state, 
        int startBoardIndex,
        Player player,
        string borderBarString,
        int? activeBoardIndex,
        int rowIndex,
        int boardWidth,
        int maxWidth,
        ref int cursorX,
        StringBuilder sb)
    {
        for (int boardIndex = startBoardIndex; boardIndex < state.Boards.Count; boardIndex += 1)
        {
            var board = state.Boards[boardIndex];
            
            //wrap check - break if cursor would exceed maxWidth
            if(cursorX > 0 && (cursorX + boardWidth > maxWidth))
            {
                cursorX = 0;
                break;
            }

            sb.Append("  ");

            for(var col = 0; col < board.ColumnCount; col += 1)
            {
                var body = ModelToKeyUtility.GetSpaceString(state, player, boardIndex, activeBoardIndex, col, rowIndex);
                body = body.PadLeft(2);
                body = body.PadRight(3);
                sb.Append($"{borderBarString}{body}");
                cursorX += body.Length;
            }
            sb.Append(borderBarString);
            cursorX += borderBarString.Length;
        }
        sb.AppendLine();
        return state.Boards.Count;
    }

    /// <summary>
    /// Render helper method for string output.
    /// </summary>
    public static string Render(TicTacToeState state, Player player, int? activeBoardIndex)
        => DrawBoards(state, player, activeBoardIndex);
}
