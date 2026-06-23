namespace KriegspielTicTacToe.Model.TicTacToe;

public record TicTacToeBoardBuilder(sbyte Width, sbyte Height, sbyte? ScoringLength = null, bool IsBoardDoneWhenScored = false) 
: BoardBuilder(Width, Height);
