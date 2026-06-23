namespace KriegspielTicTacToe.Model.TicTacToe;

public record TicTacToeBoard : Board {
    #region Constructors
    public TicTacToeBoard() : base() { }
    public TicTacToeBoard(TicTacToeBoardBuilder builder) : this(builder.Width, builder.Height, builder.ScoringLength) {}
    public TicTacToeBoard(sbyte width, sbyte height) : this(width, height, null) {}
    public TicTacToeBoard(sbyte width, sbyte height, sbyte? scoringLength = null, bool isBoardDoneWhenScored = false) {
        Spaces = new Space[width, height];
        for (var col = 0; col < Spaces.GetLength(0); col+=1) {
            for (var row = 0; row < Spaces.GetLength(1); row+=1) {
                Spaces[col,row] = new Space();
            }
        }

        if (scoringLength.HasValue) {
            HorizontalScoringLength = scoringLength.Value;
            VerticalScoringLength = scoringLength.Value;
            DiagonalScoringLength = scoringLength.Value;
        } else {
            HorizontalScoringLength = width;
            VerticalScoringLength = height;
            DiagonalScoringLength = Math.Min(width, height);
        }

        IsBoardDoneWhenScored = isBoardDoneWhenScored;
    }
    #endregion

    #region Properties
    public sbyte HorizontalScoringLength {get;init;}
    public sbyte VerticalScoringLength {get;init;}
    public sbyte DiagonalScoringLength {get;init;}
    public bool IsBoardDoneWhenScored {get;init;}
    #endregion

    #region Calculated Properties
    [JsonIgnore()]
    public override ScoreCard ScoreCard {
        get {
            var result = new ScoreCard();
            var width = Spaces.GetLength(0);
            var height = Spaces.GetLength(1);
            
            foreach (var spaceEnumerator in BoardAsEnumerable()) {
                string? lineOwnerMark = spaceEnumerator.Space.Mark;
                if(lineOwnerMark != null) {
                    var lineOwnerPlayer = new Player(lineOwnerMark);
                    result += ScoreSpace(lineOwnerPlayer, (spaceEnumerator.Col, spaceEnumerator.Row));
                }
            }
            return result;
        }
    }

    public ScoreCard ScoreSpace(
        Player lineOwnerPlayer,
        (sbyte Col, sbyte Row) pos
    ) => ScoreSpace(lineOwnerPlayer, pos, (1, 0), HorizontalScoringLength)
        + ScoreSpace(lineOwnerPlayer, pos, (0, 1), VerticalScoringLength)
        + ScoreSpace(lineOwnerPlayer, pos, (1, 1), DiagonalScoringLength)
        + ScoreSpace(lineOwnerPlayer, pos, (1, -1), DiagonalScoringLength);

    /// <summary>
    /// Score a given space for the given player and the given direction. Only
    /// counts score for lines that *start* on the space, not ones that continue
    /// on the space.
    /// </summary>
    /// <param name="lineOwnerPlayer"></param>
    /// <param name="lineStartPos"></param>
    /// <param name="delta"></param>
    /// <param name="scoreLen"></param>
    /// <returns></returns>
    public ScoreCard ScoreSpace(
        Player lineOwnerPlayer,
        (sbyte Col, sbyte Row) lineStartPos,
        (sbyte Col, sbyte Row) delta,
        int scoreLen
    ) {
        var boardSize = ((sbyte)Spaces.GetLength(0), (sbyte)Spaces.GetLength(1));
        (sbyte Col, sbyte Row) endPos = ExtrapolatePos(lineStartPos, delta, scoreLen - 1);

        //end point is outside of board.
        if (!IsSpaceInsideOfBoard(endPos, boardSize)) {
            return ScoreCard.Empty;
        }

        (sbyte Col, sbyte Row) beforeStartPos = ExtrapolatePos(lineStartPos, delta, -1);
        if (
            IsSpaceInsideOfBoard(beforeStartPos, boardSize)
            && Spaces[beforeStartPos.Col, beforeStartPos.Row].Mark == lineOwnerPlayer.Mark
        ) {
            // line already started before this space, return false to prevent double-counting.
            return ScoreCard.Empty;
        }

        var lineLength = 0;
        for (sbyte i = 0; IsSpaceInsideOfBoard(ExtrapolatePos(lineStartPos, delta, i), boardSize); i += 1) {
            (sbyte Col, sbyte Row) curPos = ExtrapolatePos(lineStartPos, delta, i);

            if (lineOwnerPlayer.Mark != Spaces[curPos.Col, curPos.Row].Mark) {
                break;
            } else {
                lineLength = i+1;
            }
        }
        var lineScore = lineLength / scoreLen;
        return (lineScore > 0) 
            ? new ScoreCard(lineOwnerPlayer, lineScore)
            : ScoreCard.Empty;
    }

    private static (sbyte Col, sbyte Row) ExtrapolatePos((sbyte Col, sbyte Row) pos, (sbyte Col, sbyte Row) delta, int multiplier)
        => ((sbyte)(pos.Col + delta.Col * multiplier), (sbyte)(pos.Row + delta.Row * multiplier));

    
    public override bool IsDone
        => IsFull 
        || (IsBoardDoneWhenScored && ScoreCard.PlayerScores.Any(s => s.Score > 0));
    #endregion

}
