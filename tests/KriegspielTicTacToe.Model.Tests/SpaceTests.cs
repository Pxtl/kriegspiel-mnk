namespace KriegspielTicTacToe.Model.Tests;

using FluentAssertions;
using Xunit;

public class SpaceTests {
    [Fact]
    public void Constructor_Default_MarkCharIsNull() {
        var space = new Space();
        space.MarkChar.Should().BeNull();
    }

    [Fact]
    public void Constructor_Default_KnownToPlayersSet_IsEmpty() {
        var space = new Space();
        space.KnownToPlayersSet.Should().BeEmpty();
    }

    [Fact]
    public void ToString_NoPlayerShowsMarkChar() {
        var space = new Space();
        space.MarkChar = 'X';

        space.ToString(null).Should().Be("X");
    }

    [Fact]
    public void ToString_KnownToPlayerShowsMarkChar() {
        var space = new Space();
        space.MarkChar = 'X';
        space.MakeKnownToPlayer('O');

        space.ToString('O').Should().Be("X");
    }

    [Fact]
    public void ToString_UneknownToPlayerShowsSpace() {
        var space = new Space();
        space.MarkChar = 'X';

        space.ToString('O').Should().Be(" ");
    }

    [Fact]
    public void MakeKnownToPlayer_AddsPlayerToSet() {
        var space = new Space();
        space.KnownToPlayersSet.Should().BeEmpty();

        space.MakeKnownToPlayer('O');
        space.KnownToPlayersSet.Should().Contain('O');
    }

    [Fact]
    public void IsKnownToPlayer_ReturnsTrue_ForKnownPlayer() {
        var space = new Space();
        space.MarkChar = 'X';
        space.MakeKnownToPlayer('O');

        space.IsKnownToPlayer('O').Should().BeTrue();
    }

    [Fact]
    public void IsKnownToPlayer_ReturnsFalse_ForUnknownPlayer() {
        var space = new Space();
        space.MarkChar = 'X';

        space.IsKnownToPlayer('O').Should().BeFalse();
        space.IsKnownToPlayer('X').Should().BeFalse();
    }
}