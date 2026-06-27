using KriegspielTicTacToe.Model.Template;
using KriegspielTicTacToe.Model.Views;

namespace KriegspielTicTacToe.Model.PlayerAIs;

[ModelSerializable]
public class WinnerAI : IPlayerAI {
    public string Description => "Winner, Difficulty 3";

    public void Attempt(GameView gameView, IEnumerable<GameActionFactory> actionFactories)
    {
        int boardRC = 0;   // row count (height in rows)
        int boardCC = 0;   // col count (width in cols)

        foreach (var board in gameView.Boards)
        {
            if (board.ColumnCount != 0)
            {
                boardRC = board.RowCount;
                boardCC = board.ColumnCount;
                break;
            }
        }
        
        // If no valid board, skip to simple action fallback
        if (boardRC == 0) goto fallbackSimpleAction;

        // Calculate center using floor division for any board size
        int centerColInt = ((int)Math.Floor((double)(boardCC - 1) / 2.0));
        int centerRowInt = ((int)Math.Floor((double)(boardRC - 1) / 2.0));

        bool onBoard(int c, int r) => 
            (r >= 0 && r < boardRC && c >= 0 && c < boardCC);

        var spaceActions = actionFactories.OfType<GameActionFactoryForSpace>().ToList();
        
        if (spaceActions.Count == 0) goto fallbackSimpleAction;

        // Vertical: scan up/down from center
        for (int d = -3; d <= 3; d++)
        {
            int targetRow = centerRowInt + d;
            if (onBoard(centerColInt, targetRow))
                gameView.Attempt(spaceActions[0].Create(0, (sbyte)(centerColInt), (sbyte)targetRow));
                break;
        }

        // Horizontal: scan left/right from center  
        for (int d = -3; d <= 3; d++)
        {
            int targetCol = centerColInt + d;
            if (onBoard(targetCol, centerRowInt))
                gameView.Attempt(spaceActions[0].Create(0, (sbyte)targetCol, (sbyte)(centerRowInt)));
                break;
        }

        // Diagonal: up-right through center  
        for (int d = -3; d <= 3; d++)
        {
            int diagCol = centerColInt + d;
            int diagRow = centerRowInt + d;
            if (onBoard(diagCol, diagRow))
                gameView.Attempt(spaceActions[0].Create(0, (sbyte)diagCol, (sbyte)diagRow));
                break;
        }

        // Diagonal: up-left through center  
        for (int d = -3; d <= 3; d++)
        {
            int diagCol2 = centerColInt + d;
            int diagRow2 = centerRowInt - d;
            if (onBoard(diagCol2, diagRow2))
            {
                gameView.Attempt(spaceActions[0].Create(0, (sbyte)diagCol2, (sbyte)diagRow2));
                break;
            }
        }

        // Center itself  
        gameView.Attempt(spaceActions[0].Create(0, (sbyte)(centerColInt), (sbyte)(centerRowInt)));

    fallbackSimpleAction:
        // SpaceNames fallback when calculated positions fail
        foreach (var spaceName in gameView.SpaceNames)
        {
            try 
            {
                if (gameView.TryGetCoordinatesFromSpaceName(spaceName, out sbyte biBox, out sbyte colBox2, out sbyte rowBox)) break;
            }
            catch { continue; }  
        }

        // Final fallback: simple action  
        var factory = actionFactories.OfType<GameActionFactoryForSimple>().FirstOrDefault();
        if (factory != null) gameView.Attempt(factory.Create());
    }
}
