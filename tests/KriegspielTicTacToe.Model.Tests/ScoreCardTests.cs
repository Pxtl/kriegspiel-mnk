namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using Xunit;

public class ScoreCardTests {
    [Fact]
    public void Constructor_Empty_ReturnsEmptyScores() {
        var scoreCard = new ScoreCard();
        scoreCard.HighestScore.Should().BeNull();
    }

    [Fact]
    public void Constructor_SingleScore() {
        var scoreCard = new ScoreCard('X', 5);
        scoreCard.HighestScore.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_MultipleScores() {
        var scoreCard = new ScoreCard(new[] { 
            new PlayerScore('X', 3), 
            new PlayerScore('O', 2) 
        });
        
        scoreCard.HighestScore.Should().NotBeNull();
    }

    [Fact]
    public void OperatorPlus() {
        var a = new ScoreCard('X', 3);
        var b = new ScoreCard('O', 2);
        
        var result = a + b;
        result.HighestScore.Should().NotBeNull();
    }
}
